namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing
{
    public class PluralRule
    {
        public PluralRule(string[] locales, Condition[] conditions)
        {
            Locales = locales;
            Conditions = conditions;
        }

        public string[] Locales { get; }

        public Condition[] Conditions { get; }
    }
}
