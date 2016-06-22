using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WPath
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Same as string.Format()
        /// Example: "{0}".FormatWith(value) instead of string.Format("{0}", value)
        /// </summary>
        public static string FormatWith(this string formatedString, params object[] args)
        {
            return string.Format(formatedString, args);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
}