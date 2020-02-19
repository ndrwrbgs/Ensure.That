﻿using JetBrains.Annotations;

namespace EnsureThat
{
    using System;

    public static partial class EnsureArg
    {
        /// <summary>
        /// Confirms that the <paramref name="value"/> is defined in the enum <typeparamref name="T"/>.
        /// </summary>
        public static T IsValidEnum<T>(T value, [InvokerParameterName] string paramName = null, OptsFn optsFn = null) where T : struct, Enum
            => Ensure.Enum.IsValidEnum(value, paramName, optsFn);
    }
}