using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST;
using System;
using System.Collections.Generic;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing;

/// <summary>
///     Represents multiple fully parsed documents of <see cref="PluralRule"/> instances, each with a given type (i.e., 'cardinal' vs 'ordinals').
/// </summary>
public class PluralRuleSet
{
    /// <summary>
    ///     Special CLDR locale ID to use as the default for inheritance. All locales can chain to this
    ///     during lookups.
    /// </summary>
    public const string RootLocale = "root";

    /// <summary>
    ///     CLDR plural type attribute for the counting number ruleset.
    ///     Used to translate strings that contain a count, e.g., to pluralize nouns.
    /// </summary>
    public const string CardinalType = "cardinal";

    /// <summary>
    ///     CLDR plural type attribute for the ordered number ruleset.
    ///     Used to translate strings containing ordinal numbers, e.g., "1st", "2nd", "3rd".
    /// </summary>
    public const string OrdinalType = "ordinal";

    // Backing fields for the public properties below.
    private readonly List<PluralRule> _allRules = [];
    private readonly Dictionary<string, PluralRuleIndices> _indicesByLocale = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Gets the unique conditions that have been indexed. Can be used to generate unique helper functions
    ///     to match specific rules based on an input number.
    /// </summary>
    public IReadOnlyList<PluralRule> UniqueRules => this._allRules;

    /// <summary>
    ///     Maps <see cref="NormalizeCldrLocale(string)">normalized CLDR locale IDs</see> to indices within
    ///     <see cref="UniqueRules"/> for their cardinal and ordinal rules, if defined.
    /// </summary>
    public IReadOnlyDictionary<string, PluralRuleIndices> RuleIndicesByLocale => this._indicesByLocale;

    /// <summary>
    ///     Adds the given rule to our indices under the given plural type.
    /// </summary>
    /// <param name="pluralType">e.g., 'cardinal' or 'ordinal'.</param>
    /// <param name="rule">The parsed rule.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a nonstandard plural type is provided.</exception>
    public void Add(string pluralType, PluralRule rule)
    {
        this._allRules.Add(rule);
        int newRuleIndex = this._allRules.Count - 1;

        int? cardinalIndex = null;
        int? ordinalIndex = null;
        if (pluralType == CardinalType)
        {
            cardinalIndex = newRuleIndex;
        }
        else if (pluralType == OrdinalType)
        {
            ordinalIndex = newRuleIndex;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(pluralType), pluralType, "Unexpected plural type");
        }

        // Loop over each locale for this rule and update our indices with the new value.
        // If we've seen it before (for a different plural type), we'll update it in-place.
        foreach (var locale in rule.Locales)
        {
            var normalized = this.NormalizeCldrLocale(locale);

            PluralRuleIndices newIndices;
            if (this._indicesByLocale.TryGetValue(normalized, out var existingIndices))
            {
                // Merge any previous indices we've observed for this locale
                newIndices = existingIndices with
                {
                    CardinalRuleIndex = cardinalIndex ?? existingIndices.CardinalRuleIndex,
                    OrdinalRuleIndex = ordinalIndex ?? existingIndices.OrdinalRuleIndex
                };
            }
            else
            {
                newIndices = new PluralRuleIndices(
                    CardinalRuleIndex: cardinalIndex,
                    OrdinalRuleIndex: ordinalIndex
                );

            }

            this._indicesByLocale[normalized] = newIndices;
            if (normalized != locale)
            {
                this._indicesByLocale[locale] = newIndices;
            }
        }
    }

    /// <summary>
    ///     Converts a CLDR locale ID to a normalized form for indexing.
    ///
    ///     See <see href="https://unicode.org/reports/tr35/#unicode-locale-identifier"/>the LDML spec</see>
    ///     for an explanation of the forms that Unicode locale IDs can take.
    ///
    ///     Notably, CLDR locale IDs use underscores as separators, while BCP 47 (which is the primary form
    ///     we expect as inputs at runtime) use dashes.
    /// </summary>
    /// <remarks>
    ///     The return string is intended to be used for case-insensitive runtime lookup of input locales,
    ///     but the string itself is not strictly BCP 47 or CLDR compliant. For example, the CLDR 'root'
    ///     language is passed through instead of being remapped to 'und'.
    /// </remarks>
    private string NormalizeCldrLocale(string cldrLocaleId)
    {
        return cldrLocaleId.Replace('_', '-');
    }

    /// <summary>
    ///     Helper type to represent the pluralization rules for a given locale, which may include both
    ///     cardinal and ordinal rules, or just one of the two.
    /// </summary>
    /// <remarks>
    ///     For example, in CLDR 48.1, "pt_PT" has a defined plural rule but is expected to chain to "pt"
    ///     for ordinal lookup.
    /// </remarks>
    public record PluralRuleIndices(int? CardinalRuleIndex, int? OrdinalRuleIndex);
}