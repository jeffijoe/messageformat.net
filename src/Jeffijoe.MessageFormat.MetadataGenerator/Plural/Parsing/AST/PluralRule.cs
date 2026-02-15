using System.Collections.Generic;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing.AST;

/// <summary>
/// Corresponds to a pluralRules tag in CLDR XML (not to be confused with pluralRule).
/// Each instance of this class represents multiple individual rules for a set of locales.
/// </summary>
/// <example>
///     &lt;pluralRules locales="ast de en et fi fy gl ia ie io ji lij nl sc sv sw ur yi"&gt;
///         &lt;pluralRule count = "one"&gt; i = 1 and v = 0 @integer 1&lt;/pluralRule&gt;
///         ...
///     &lt;/pluralRules&gt;
/// </example>
public class PluralRule
{
    public PluralRule(string[] locales, IReadOnlyList<Condition> conditions)
    {
        Locales = locales;
        Conditions = conditions;
    }

    public string[] Locales { get; }

    public IReadOnlyList<Condition> Conditions { get; }
}