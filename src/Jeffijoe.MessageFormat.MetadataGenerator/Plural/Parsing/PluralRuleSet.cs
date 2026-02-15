using System.Collections.Generic;
using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing;

/// <summary>
/// Represents multiple fully parsed documents of <see cref="PluralRule"/> instances, each with a given type (i.e., 'cardinal' vs 'ordinals').
/// </summary>
public class PluralRuleSet
{
    /// <summary>
    /// Indexes a set of <see cref="PluralRule"/> instances by plural type & locale.
    /// </summary>
    /// <remarks>
    /// Intended for runtime lookup of plural rules when formatting messages.
    /// </remarks>
    private readonly Dictionary<RuleKey, PluralRule> _byLocaleAndType = [];

    /// <summary>
    /// Keeps track of which languages we've seen.
    /// </summary>
    private readonly HashSet<string> _indexedLocales = [];

    /// <summary>
    ///     Gets the unique conditions that have been indexed. Can be used to generate unique helper functions
    ///     to match specific rules based on an input number.
    /// </summary>
    public IEnumerable<PluralRule> RulesWithUniqueConditions => _byLocaleAndType.Values;

    /// <summary>
    ///     Gets the set of observed locale strings.
    /// </summary>
    public IEnumerable<string> IndexedLocales => this._indexedLocales;

    /// <summary>
    /// Adds the given rule to our index under the given plural type.
    /// </summary>
    /// <param name="pluralType">e.g., 'cardinal' or 'ordinal'.</param>
    /// <param name="rule">The parsed rule.</param>
    public void Add(string pluralType, PluralRule rule)
    {
        foreach (var locale in rule.Locales)
        {
            this._indexedLocales.Add(locale);
            this._byLocaleAndType.Add(
                new RuleKey
                {
                    PluralType = pluralType,
                    Locale = locale,
                },
                rule
            );
        }
    }

    

    /// <summary>
    ///     Walks the types of plurals we've indexed for a specific locale, along with corresponding rule.
    ///     Each returned rule is guaranteed to have a unique <see cref="PluralRule.PluralType"/> value.
    /// </summary>
    public IEnumerable<PluralRule> GetIndexedRulesByTypeForLocale(string locale)
    {
        if (_byLocaleType.TryGetValue(locale, out var byType))
        {
            foreach (var kvp in byType)
            {
                yield return kvp.Value;
            }
        }
    }

    /// <summary>
    ///     Attempts to lookup the rule for a specific locale and a specific type.
    /// </summary>
    /// <returns>Null if no match.</returns>
    public PluralRule? Get(string locale, string pluralType)
    {
        if (_byLocaleType.TryGetValue(locale, out var byType))
        {
            byType.TryGetValue(pluralType, out var rule);
            return rule;
        }

        return null;
    }

    /// <summary>
    /// Used to retrieve a specific <see cref="PluralRule"/>.
    /// </summary>
    /// <param name="PluralType">e.g., 'cardinal' or 'ordinal'.</param>
    /// <param name="Locale"></param>
    public record struct RuleKey(string PluralType, string Locale);
}