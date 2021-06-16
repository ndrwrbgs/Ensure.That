﻿using System.Collections;
using Xunit;

namespace UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using EnsureThat;

    using FluentAssertions;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    /// <summary>
    /// Full coverage obtained by
    ///
    /// A ⊃ B
    /// B ⊃ C
    /// C ⊃ A
    ///
    /// A: Ensure.That(x).Blah()
    /// B: EnsureArg.Blah(x)
    /// C: Ensure.XType.Blah(x)
    /// </summary> 
    public class ApiParityTests
    {
        private readonly ITestOutputHelper output;

        public ApiParityTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void EnumerateEnsureThat()
        {
            foreach (var method in GetEnsureThatMethods())
            {
                this.output.WriteLine($"{method.methodName}({method.forTypeWithConstraints}, {string.Join(", ", method.args.Select(p => p.ToString()))})");
            }
        }

        [Fact]
        public void EnumerateEnsureArg()
        {
            foreach (var method in GetEnsureArgMethods())
            {
                this.output.WriteLine($"{method.methodName}({method.forTypeWithConstraints}, {string.Join(", ", method.args.Select(p => p.ToString()))})");
            }
        }
        [Fact]
        public void EnumerateEnsure()
        {
            //foreach (var method in GetEnsureMethods())
            //{
            //    this.output.WriteLine($"{method.methodName}({method.forTypeWithConstraints}, {string.Join(", ", method.args.Select(p => p.ToString()))})");
            //}
        }


        [Fact]
        public void EnsureThatImplementedInEnsureArg()
        {
            var ensureThatMethods = GetEnsureThatMethods().ToList();
            var ensureArgMethods = GetEnsureArgMethods().ToList();

            foreach (var ensureThatMethod in ensureThatMethods)
            {
                // Exclusions - found in Ensure.That but not EnsureArg
                if (string.Equals(ensureThatMethod.methodName, "HasItems")
                    && string.Equals(ensureThatMethod.forTypeWithConstraints.ToString(), "System.Collections.ObjectModel.Collection`1[T]"))
                {
                    continue;
                }
                if (string.Equals(ensureThatMethod.methodName, "HasItems")
                    && string.Equals(ensureThatMethod.forTypeWithConstraints.ToString(), "System.Collections.Generic.List`1[T]"))
                {
                    continue;
                }
                if (string.Equals(ensureThatMethod.methodName, "HasItems")
                    && string.Equals(ensureThatMethod.forTypeWithConstraints.ToString(), "System.Collections.Generic.HashSet`1[T]"))
                {
                    continue;
                }
                if (string.Equals(ensureThatMethod.methodName, "HasItems")
                    && string.Equals(ensureThatMethod.forTypeWithConstraints.ToString(), "System.Collections.Generic.Dictionary`2[TKey,TValue]"))
                {
                    continue;
                }
                if (string.Equals(ensureThatMethod.methodName, "ContainsKey")
                    && string.Equals(ensureThatMethod.forTypeWithConstraints.ToString(), "System.Collections.Generic.Dictionary`2[TKey,TValue]"))
                {
                    continue;
                }
                if (string.Equals(ensureThatMethod.methodName, "HasAny")
                    && string.Equals(ensureThatMethod.forTypeWithConstraints.ToString(), "System.Collections.Generic.List`1[T]"))
                {
                    continue;
                }
                if (string.Equals(ensureThatMethod.methodName, "HasAny")
                    && string.Equals(ensureThatMethod.forTypeWithConstraints.ToString(), "System.Collections.ObjectModel.Collection`1[T]"))
                {
                    continue;
                }
                if (string.Equals(ensureThatMethod.methodName, "HasAny")
                    && string.Equals(ensureThatMethod.forTypeWithConstraints.ToString(), "System.Collections.Generic.IDictionary`2[TKey,TValue]"))
                {
                    continue;
                }
                if (string.Equals(ensureThatMethod.methodName, "HasAny")
                    && string.Equals(ensureThatMethod.forTypeWithConstraints.ToString(), "System.Collections.Generic.Dictionary`2[TKey,TValue]"))
                {
                    continue;
                }
                if (string.Equals(ensureThatMethod.methodName, "IsClass")
                    && string.Equals(ensureThatMethod.forTypeWithConstraints.ToString(), "T"))
                {
                    continue;
                }

                ensureArgMethods
                    .Any(
                        ensureArgMethod =>
                        {
                            if (!string.Equals(ensureThatMethod.methodName, ensureArgMethod.methodName))
                            {
                                return false;
                            }

                            // TODO: Better way to compare these
                            if (!string.Equals(ensureThatMethod.forTypeWithConstraints.ToString(), ensureArgMethod.forTypeWithConstraints.ToString()))
                            // == failed
                            // MetadataToken failed when they came from different nesting levels
                            //if (ensureThatMethod.forTypeWithConstraints.MetadataToken != ensureArgMethod.forTypeWithConstraints.MetadataToken)
                            {
                                return false;
                            }

                            // TODO: Other 2 arguments
                            return true;
                        })
                    .Should()
                    .BeTrue(because: $"{ensureThatMethod.methodName}({ensureThatMethod.forTypeWithConstraints}, {string.Join(", ", ensureThatMethod.args.Select(p => p.ToString()))})" + $" is not matched in EnsureArg");
            }
        }

        [Fact]
        public void EnsureArgImplementedInEnsure()
        {
        }

        [Fact]
        public void EnsureImplementedInEnsureThat()
        {

        }

        private static IEnumerable<(Type forTypeWithConstraints, string methodName, ParameterInfo[] args)> GetEnsureArgMethods()
        {
            foreach (var methodInfo in typeof(EnsureArg).GetMethods())
            {
                ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                if (parameterInfos.Length == 0) continue;

                // Skip paramName and optsFn (and the first argument)
                var otherParameters = parameterInfos
                    .Skip(1)
                    .Take(parameterInfos.Length - 3)
                    .ToArray();

                var firstParameter = parameterInfos[0].ParameterType;

                if (firstParameter.IsGenericParameter)
                {
                    var constraintsOnT = firstParameter.GetGenericParameterConstraints();
                    if (constraintsOnT.Length == 1)
                    {
                        yield return (
                            forTypeWithConstraints: constraintsOnT.Single(),
                            methodName: methodInfo.Name,
                            args: otherParameters);
                        continue;
                    }
                }

                yield return (
                    forTypeWithConstraints: firstParameter,
                    methodName: methodInfo.Name,
                    args: otherParameters);
            }
        }

        private static IEnumerable<(Type forTypeWithConstraints, string methodName, ParameterInfo[] args)> GetEnsureThatMethods()
        {
            foreach (MethodInfo methodInfo in typeof(EnsureArg).Assembly
                .ExportedTypes
                .SelectMany(type => type.GetMethods())
                .Where(method => method.IsStatic)
            )
            {
                // Filter to Param<T> methods
                ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                if (parameterInfos.Length == 0) continue;


                var firstParameter = parameterInfos[0].ParameterType;

                if (!firstParameter.IsByRef) continue;

                firstParameter = firstParameter.GetElementType();

                if (!firstParameter.IsGenericType) continue;

                if (firstParameter.GetGenericTypeDefinition() != typeof(Param<>)) // TODO: StringParam and any other Param types
                {
                    continue;
                }

                if (firstParameter.GenericTypeArguments.First().IsGenericParameter)
                {
                    var constraintsOnT = firstParameter.GenericTypeArguments.First().GetGenericParameterConstraints();
                    if (constraintsOnT.Length == 1)
                    {
                        yield return (
                            forTypeWithConstraints: constraintsOnT.Single(),
                            methodName: methodInfo.Name,
                            args: parameterInfos.Skip(1).ToArray());
                        continue;
                    }
                    // TODO: Handle GenericParameterAttributes which don't show up in GetGenericParameterConstraints
                    //if (firstParameter.GenericTypeArguments.First().GenericParameterAttributes == GenericParameterAttributes.)
                }

                yield return (
                    forTypeWithConstraints: firstParameter.GenericTypeArguments[0],
                    methodName: methodInfo.Name,
                    args: parameterInfos.Skip(1).ToArray());
            }

            yield break;
        }
    }
}