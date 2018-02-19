using System;
using System.Collections.Generic;
using System.Text;

namespace Fbeltrao.AzureFunctionExtensions
{
    /// <summary>
    /// Utility class
    /// </summary>
    internal class Utils
    {
        /// <summary>
        /// Merge values from the specified inputs
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        internal static string MergeValueForProperty(string v1, string v2, string v3)
        {
            if (!string.IsNullOrEmpty(v1))
                return v1;

            if (!string.IsNullOrEmpty(v2))
                return v2;

            return v3 ?? string.Empty;
        }

        /// <summary>
        /// Merge values from the specified inputs
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        internal static string MergeValueForProperty(string v1, string v2)
        {
            if (!string.IsNullOrEmpty(v1))
                return v1;
            
            return v2 ?? string.Empty;
        }


        /// <summary>
        /// Merge values from the specified inputs
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        internal static int MergeValueForPropertyInt(int? v1, int? v2, int defaultValue = 0)
        {
            if (v1.HasValue)
                return v1.Value;

            if (v2.HasValue)
                return v2.Value;
            
            return defaultValue;
        }

        /// <summary>
        /// Merge values from the specified inputs
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        internal static int MergeValueForPropertyInt(int? v1, int? v2, int? v3, int defaultValue = 0)
        {
            if (v1.HasValue)
                return v1.Value;

            if (v2.HasValue)
                return v2.Value;

            if (v3.HasValue)
                return v3.Value;

            return defaultValue;
        }

        /// <summary>
        /// Merge values from nullable types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <returns></returns>
        internal static T? MergeValueForNullableProperty<T>(T? value1, T? value2, T? value3)
            where T : struct
        {
            return value1 ?? (value2 ?? value3);
        }
    }
}
