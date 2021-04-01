using System;
using System.Collections.Generic;
using System.Text;

namespace Jeffijoe.MessageFormat.Formatting.Formatters
{
    public static partial class PluralRulesMetadata
    {
        public static string DefaultPluralRule(double number)
        {
            if(number == 0)
            {
                return "zero";
            }

            if(number == 1)
            {
                return "one";
            }

            return "other";
        }

        public static partial bool TryGetRuleByLocale(string locale, out Pluralizer pluralizer);
    }
}
