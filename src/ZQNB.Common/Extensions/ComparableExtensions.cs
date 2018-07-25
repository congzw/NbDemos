using System;

namespace ZQNB.Common.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IComparable{T}"/>.
    /// </summary>
    public static class ComparableExtensions
    {
        /// <summary>
        /// Checks a value is between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value to be checked</param>
        /// <param name="min">Minimum (exclusive) value</param>
        /// <param name="max">Maximum (exclusive) value</param>
        public static bool IsBetweenExclusive<T>(this T value, T min, T max) where T : IComparable<T>
        {
            return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
        }

        /// <summary>
        /// Checks a value is between a minimum and maximum value.
        /// </summary>
        /// <param name="value">The value to be checked</param>
        /// <param name="min">Minimum (inclusive) value</param>
        /// <param name="max">Maximum (inclusive) value</param>
        public static bool IsBetweenInclusive<T>(this T value, T min, T max) where T : IComparable<T>
        {
            return value.CompareTo(min) > 0 && value.CompareTo(max) < 0;
        }

        /// <summary>
        /// 整数范围
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="inclusiveMin">包含最小</param>
        /// <param name="inclusiveMax">包含最大</param>
        /// <returns></returns>
        public static bool IsBetween<T>(this T value, T min, T max, bool inclusiveMin, bool inclusiveMax) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
            {
                return false;
            }

            if (!inclusiveMin && value.CompareTo(min) == 0)
            {
                return false;
            }

            if (value.CompareTo(max) > 0)
            {
                return false;
            }

            if (!inclusiveMax && value.CompareTo(max) == 0)
            {
                return false;
            }

            return true;
        }
    }
}