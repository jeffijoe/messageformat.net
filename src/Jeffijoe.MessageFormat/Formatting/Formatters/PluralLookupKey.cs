namespace Jeffijoe.MessageFormat.Formatting.Formatters
{
    /// <summary>
    /// Used to retrieve a specific <see cref="ContextPluralizer"/>.
    /// </summary>
    /// <param name="PluralType">e.g., 'cardinal' or 'ordinal'.</param>
    /// <param name="Locale"></param>
    public record struct PluralRuleKey(string PluralType, string Locale);
}
