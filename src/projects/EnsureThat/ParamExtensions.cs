using System;
using JetBrains.Annotations;

namespace EnsureThat
{
    public static class ParamExtensions
    {
        [Pure]
        public static Param<T> And<T>(this Param<T> param)
        {
            return param;
        }

        [Pure]
        public static Param<T> WithExtraMessageOf<T>(this Param<T> param, string message)
        {
            return new Param<T>(
                param.Name,
                param.Value,
                p => message,
                param.ExceptionFn);
        }

        [Pure]
        public static Param<T> WithExtraMessageOf<T>(this Param<T> param, [NotNull] Func<string> messageFn)
        {
            return new Param<T>(
                param.Name,
                param.Value,
                p => messageFn(),
                param.ExceptionFn);
        }

        [Pure]
        public static Param<T> WithExtraMessageOf<T>(this Param<T> param, Func<Param<T>, string> messageFn)
        {
            return new Param<T>(
                param.Name,
                param.Value,
                messageFn,
                param.ExceptionFn);
        }

        [Pure]
        public static Param<T> WithException<T>(this Param<T> param, Func<Param<T>, Exception> exceptionFn)
        {
            return new Param<T>(
                param.Name,
                param.Value,
                param.ExtraMessageFn,
                exceptionFn);
        }
    }
}