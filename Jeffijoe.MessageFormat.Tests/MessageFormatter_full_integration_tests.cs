// MessageFormatter_full_integration_tests.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat.Tests
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System.Collections.Generic;
using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Formatting.Formatters;
using Jeffijoe.MessageFormat.Parsing;
using Xunit;
using Xunit.Extensions;

namespace Jeffijoe.MessageFormat.Tests
{
    public class MessageFormatter_full_integration_tests
    {
        [Theory]
        [PropertyData("Tests")]
        public void FormatMessage(string source, Dictionary<string, object> args, string expected)
        {
            var formatterLibrary = new FormatterLibrary();
            formatterLibrary.Add(new ReplaceFormatter());
            formatterLibrary.Add(new SelectFormatter());
            formatterLibrary.Add(new PluralFormatter());
            var literalParser = new LiteralParser();
            var patternParser = new PatternParser(literalParser);
            var subject = new MessageFormatter(patternParser, formatterLibrary);

            Assert.Equal(expected, subject.FormatMessage(source, args));
        }

        public static IEnumerable<object[]> Tests
        {
            get
            {
                string case1 = @"{gender, select, 
                           male {He - \{{name}\} -}
                           female {She - \{{name}\} -}
                           other {They}
                      } said: You're pretty cool!";
                string case2 = @"{gender, select, 
                           male {He - \{{name}\} -}
                           female {She - \{{name}\} -}
                           other {They}
                      } said: You have {count, plural, 
                            zero {no notifications}
                            one {just one notification}
                            =42 {an universal amount of notifications}
                            other {# notifications}
                      }. Have a nice day!";
                string case3 = @"You have {count, plural, 
                            zero {no notifications}
                            one {just one notification}
                            =42 {an universal amount of notifications}
                            other {# notifications}
                      }. Have a nice day!";
                yield return new object[]
                {
                    case1,
                    new Dictionary<string, object>
                    {
                        {"gender", "male"},
                        {"name", "Jeff"}
                    },
                    "He - {Jeff} - said: You're pretty cool!"
                };
                yield return new object[]
                {
                    case2,
                    new Dictionary<string, object>
                    {
                        {"gender", "male"},
                        {"name", "Jeff"},
                        {"count", 0}
                    },
                    "He - {Jeff} - said: You have no notifications. Have a nice day!"
                };
                yield return new object[]
                {
                    case2,
                    new Dictionary<string, object>
                    {
                        {"gender", "female"},
                        {"name", "Amanda"},
                        {"count", 1}
                    },
                    "She - {Amanda} - said: You have one notification. Have a nice day!"
                };
                yield return new object[]
                {
                    case2,
                    new Dictionary<string, object>
                    {
                        {"gender", "uni"},
                        {"count", 42}
                    },
                    "They said: You have an universal amount of notifications. Have a nice day!"
                };
                yield return new object[]
                {
                    case3,
                    new Dictionary<string, object>
                    {
                        {"count", 5}
                    },
                    "You have 5 notifications. Have a nice day!"
                };
            }
        }
    }
}