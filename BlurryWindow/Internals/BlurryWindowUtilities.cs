using System;

namespace BlurryControls.Internals
{
    internal static class BlurryWindowUtilities
    {
        /// <summary>
        /// returns the corresponting enum value if it is part of the enum
        /// </summary>
        /// <typeparam name="T">enum type</typeparam>
        /// <param name="value">name of the demanded enum value</param>
        /// <returns>the anum value of type enum with the corresponding name</returns>
        internal static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}