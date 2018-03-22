using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Globalization;

using UnityEngine;
using UnityEngine.UI;

namespace UI.Xml
{
    public static class StringExtensions
    {
        /// <summary>
        /// Remove the specified characters from the string.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static string StripChars(this string s, params char[] chars)
        {
            if (String.IsNullOrEmpty(s) || !chars.Any()) return s;

            var pattern = "[" + String.Join("", chars.Select(c => c.ToString()).ToArray()) + "]";
            var regex = new Regex(pattern);

            return regex.Replace(s, "");
        }

        /// <summary>
        /// Returns the string with spaced in front of each capital letter.
        /// Existing whitespace is left as is.
        /// If the string starts with a capital letter, no space is added for that letter.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string SplitByCapitals(this string s)
        {
            if (String.IsNullOrEmpty(s))
            {
                s = "";
            }

            var regex = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

            return regex.Replace(s, " ");
        }        

        public static string ToTitleCase(this string s)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());
        }
    }
}
