// CharHelper.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.
namespace Jeffijoe.MessageFormat.Helpers
{
    /// <summary>
    /// Char helper
    /// </summary>
    internal static class CharHelper
    {
        private static readonly char[] Alphanumberic = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_".ToCharArray();
        

        /// <summary>
        /// Determines whether the specified character is alpha numeric.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
         internal static bool IsAlphaNumeric(this char c)
         {
             foreach (var chr in Alphanumberic)
             {
                 if (chr == c)
                     return true;
             }
             return false;
         }
    }
}