using System;
using System.Collections.Generic;
using System.Diagnostics;
using EnsureThat.Extensions;
using JetBrains.Annotations;

namespace EnsureThat
{
    public class EquatableArg
    {
        [DebuggerStepThrough]
        public T Is<T>(T value, T expected, [InvokerParameterName] string paramName = Param.DefaultName) where T : IEquatable<T>
        {
            if (!Ensure.IsActive)
                return value;

            if (!value.IsEq(expected))
                throw new ArgumentException(ExceptionMessages.Comp_Is_Failed.Inject(value, expected), paramName);

            return value;
        }

        [DebuggerStepThrough]
        public T Is<T>(T value, T expected, [NotNull] IEqualityComparer<T> comparer, [InvokerParameterName] string paramName = Param.DefaultName)
        {
            if (!Ensure.IsActive)
                return value;

            if (!value.IsEq(expected, comparer))
                throw new ArgumentException(ExceptionMessages.Comp_Is_Failed.Inject(value, expected), paramName);

            return value;
        }

        [DebuggerStepThrough]
        public T IsNot<T>(T value, T expected, [InvokerParameterName] string paramName = Param.DefaultName) where T : IEquatable<T>
        {
            if (!Ensure.IsActive)
                return value;

            if (value.IsEq(expected))
                throw new ArgumentException(ExceptionMessages.Comp_IsNot_Failed.Inject(value, expected), paramName);

            return value;
        }


        [DebuggerStepThrough]
        public T IsNot<T>(T value, T expected, [NotNull] IEqualityComparer<T> comparer, [InvokerParameterName] string paramName = Param.DefaultName)
        {
            if (!Ensure.IsActive)
                return value;

            if (value.IsEq(expected, comparer))
                throw new ArgumentException(ExceptionMessages.Comp_IsNot_Failed.Inject(value, expected), paramName);

            return value;
        }
    }
}