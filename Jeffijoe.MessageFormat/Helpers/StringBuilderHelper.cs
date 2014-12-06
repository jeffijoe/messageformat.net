// StringBuilderHelper.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System.Text;

namespace Jeffijoe.MessageFormat.Helpers
{
    /// <summary>
    /// String Builder helper
    /// </summary>
    internal static class StringBuilderHelper
    {
        /// <summary>
        /// Trims the whitespace.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <returns></returns>
        internal static StringBuilder TrimWhitespace(this StringBuilder src)
         {
             var length = 0;

             for (int i = 0; i < src.Length; i++)
             {
                 var c = src[i];
                 if(char.IsWhiteSpace(c) == false)
                 {
                     length = i;
                     break;
                 }
             }
             if(length != 0)
             {
                 src = src.Remove(0, length);
             }

             var startIndex = 0;
             for (int i = src.Length-1; i >= 0; i--)
             {
                 var c = src[i];
                 if (char.IsWhiteSpace(c) == false)
                 {
                     startIndex = i+1;
                     break;
                 }
             }

             if (startIndex == src.Length)
                 return src;
            length = src.Length - startIndex;
            src.Remove(startIndex, length);
             return src;
         }

         /// <summary>
         /// Determines whether the specified source contains any of the specified characters.
         /// </summary>
         /// <param name="src">The source.</param>
         /// <param name="chars">The chars.</param>
         /// <returns></returns>
        internal static bool Contains(this StringBuilder src, params char[] chars)
        {
            for (int i = 0; i < src.Length; i++)
            {
                foreach (var c in chars)
                {
                    if (src[i] == c)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified source contains whitespace.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <returns></returns>
        internal static bool ContainsWhitespace(this StringBuilder src)
        {
            return src.Contains(' ', '\r', '\n', '\t');
        }
    }
}