using System.Collections.Generic;
using Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing;

/// <summary>
/// Represents multiple fully parsed documents of <see cref="PluralRule"/> instances, each with a given type (i.e., 'cardinal' vs 'ordinals').
/// </summary>
public class PluralRuleSet
{
    private readonly List<PluralRule> _allRules = [];

    /// <summary>
    /// Indexes a set of <see cref="PluralRule"/> instances by plural type & locale.
    /// </summary>
    /// <remarks>
    /// Intended for runtime lookup of plural rules when formatting messages.
    /// </remarks>
    private readonly Dictionary<PluralRuleKey, int> _byLocaleAndType = [];

    /// <summary>
    ///     Gets the unique conditions that have been indexed. Can be used to generate unique helper functions
    ///     to match specific rules based on an input number.
    /// </summary>
    public IReadOnlyList<PluralRule> UniqueRules => this._allRules;

    /// <summary>
    /// Walks indexes plural rules by locale + type, and gets the index into <see cref="UniqueRules"/>
    /// for each tuple.
    /// </summary>
    public IEnumerable<KeyValuePair<PluralRuleKey, int>> RuleIndicesByKey => this._byLocaleAndType;

    /// <summary>
    /// Adds the given rule to our index under the given plural type.
    /// </summary>
    /// <param name="pluralType">e.g., 'cardinal' or 'ordinal'.</param>
    /// <param name="rule">The parsed rule.</param>
    public void Add(string pluralType, PluralRule rule)
    {
        this._allRules.Add(rule);
        int newRuleIndex = this._allRules.Count - 1;

        foreach (var locale in rule.Locales)
        {
            this._byLocaleAndType.Add(
                new PluralRuleKey(PluralType: pluralType, Locale: locale),
                newRuleIndex
            );
        }
    }

    /// <summary>
    /// Used to retrieve a specific <see cref="PluralRule"/>.
    /// </summary>
    public readonly record struct PluralRuleKey(string PluralType, string Locale)
    {
        /// <summary>
        /// e.g., 'cardinal' or 'ordinal'.
        /// </summary>
        public string PluralType { get; } = PluralType;

        /// <summary>
        /// The language to query.
        /// </summary>
        public string Locale { get; } = Locale;
    }
}