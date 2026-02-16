namespace Jeffijoe.MessageFormat.Formatting.Formatters
{
    /// <summary>
    /// Used to retrieve a specific <see cref="ContextPluralizer"/>.
    /// </summary>
    /// <param name="PluralType">e.g., 'cardinal' or 'ordinal'.</param>
    /// <param name="Locale"></param>
    public readonly record struct PluralRuleKey(string PluralType, string Locale)
    {
        /// <summary>
        /// Helper to generate a cardinal rule look up for a locale, suitable for the 'plural' MessageFormat function.
        /// </summary>
        public static PluralRuleKey Cardinal(string locale) => new(PluralType: PluralFormatter.CardinalType, Locale: locale);
        
        /// <summary>
        /// Helper to generate an ordinal rule look up for a locale, suitable for the 'selectordinal' MessageFormat function.
        /// </summary>
        public static PluralRuleKey Ordinal(string locale) => new(PluralType: PluralFormatter.OrdinalType, Locale: locale);
    }
}
