using Jeffijoe.MessageFormat.Formatting.Formatters;
using System.Collections.Generic;

namespace Jeffijoe.MessageFormat.Helpers;

/// <summary>
///     Helpers for working with locale strings.
/// </summary>
internal class LocaleHelper
{
    /// <summary>
    ///     Partial implementation of <see href="https://www.unicode.org/reports/tr35/tr35.html#Locale_Inheritance">locale inheritance</see>
    ///     from the LDML spec.
    ///
    ///     Given an input locale in BCP 47 format, yields back various strings to use as lookups in CLDR data.
    /// </summary>
    /// <remarks>
    ///     This function doesn't perform any canonicalization of input or fully implement the LDML spec.
    ///     It first yields the input as-is, then the base language tag, then the CLDR "root" value.
    ///     
    ///     This is because at the time of authorship, the only lookups needed by this library are for CLDR plurals,
    ///     which almost exclusively use languages without subtags.
    /// </remarks>
    /// <example>
    ///     Given "language-Script-REGION", yields:
    ///     - language-Script-REGION
    ///     - language
    ///     - root
    /// </example>
    /// <param name="locale">A BCP 47 locale tag</param>
    public static IEnumerable<string> GetInheritanceChain(string locale)
    {
        // 0 or 1 characters do not form a valid language ID, so we can skip those
        // Also skip x- and i- as those BCP 47 tags will never match CLDR and should
        // only resolve to 'root'.
        if (locale.Length >= 2 && locale[1] != '-')
        {
            yield return locale;
        }

        // If the length is 2, we don't have any subtags for valid input
        if (locale.Length >= 3 && locale[1] != '-')
        {
            // Find the first separator character, Substring to that, and break
            for (int i = 2; i < locale.Length; i++)
            {
                if (locale[i] == '_' || locale[i] == '-')
                {
                    yield return locale.Substring(0, i);
                    break;
                }
            }
        }

        yield return PluralRulesMetadata.RootLocale;
    }
}
