// MessageFormatter_full_integration_tests.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat.Tests
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System;
using System.Collections.Generic;
using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Formatting.Formatters;
using Jeffijoe.MessageFormat.Parsing;
using Jeffijoe.MessageFormat.Tests.TestHelpers;
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
            var subject = new MessageFormatter();

            // Warmup
            subject.FormatMessage(source, args);
            Benchmark.Start("Formatting");
            string result = subject.FormatMessage(source, args);
            Benchmark.End();
            Assert.Equal(expected, result);
            Console.WriteLine(result);
        }

        [Fact]
        public void FormatMessage_debug()
        {
            var source = @"{gender, select, 
                           male {He}
                           female {She}
                           other {They}
                      } said: You have {count, plural, 
                            zero {no notifications}
                            one {just one notification}
                            =42 {a universal amount of notifications}
                            other {# notifications}
                      }. Have a nice day!";
            var expected = "He said: You have 5 notifications. Have a nice day!";
            var args = new Dictionary<string, object>
            {
                {"gender", "male"},
                {"count", 5}
            };
            var subject = new MessageFormatter();

            string result = subject.FormatMessage(source, args);
            Assert.Equal(expected, result);
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
                            =42 {a universal amount of notifications}
                            other {# notifications}
                      }. Have a nice day!";
                string case3 = @"You have {count, plural, 
                            zero {no notifications}
                            one {just one notification}
                            =42 {a universal amount of notifications}
                            other {# notifications}
                      }. Have a nice day!";
                string case4 = @"{gender, select, 
                           male {He}
                           female {She}
                           other {They}
                      } said: You have {count, plural, 
                            zero {no notifications}
                            one {just one notification}
                            =42 {a universal amount of notifications}
                            other {# notifications}
                      }. Have a nice day!";
                string case5 = @"{gender, select, 
                           male {He (who has {genitals, plural, 
                                    zero {no testicles}
                                    one {just one testicle}
                                    =2 {a normal amount of testicles}
                                    other {the insane amount of # testicles}
                                })}
                           female {She (who has {genitals, plural, 
                                    zero {no boobies}
                                    one {just one boob}
                                    =2 {a pair of lovelies}
                                    other {the freakish amount of # boobies}
                                })}
                           other {They}
                      } said: You have {count, plural, 
                            zero {no notifications}
                            one {just one notification}
                            =42 {a universal amount of notifications}
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
                    "She - {Amanda} - said: You have just one notification. Have a nice day!"
                };
                yield return new object[]
                {
                    case2,
                    new Dictionary<string, object>
                    {
                        {"gender", "uni"},
                        {"count", 42}
                    },
                    "They said: You have a universal amount of notifications. Have a nice day!"
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
                yield return new object[]
                {
                    case4,
                    new Dictionary<string, object>
                    {
                        {"count", 5},
                        {"gender", "male"}
                    },
                    "He said: You have 5 notifications. Have a nice day!"
                };
                yield return new object[]
                {
                    case5,
                    new Dictionary<string, object>
                    {
                        {"count", 5},
                        {"gender", "male"},
                        {"genitals", 0}
                    },
                    "He (who has no testicles) said: You have 5 notifications. Have a nice day!"
                };
                yield return new object[]
                {
                    case5,
                    new Dictionary<string, object>
                    {
                        {"count", 5},
                        {"gender", "female"},
                        {"genitals", 0}
                    },
                    "She (who has no boobies) said: You have 5 notifications. Have a nice day!"
                };
                yield return new object[]
                {
                    case5,
                    new Dictionary<string, object>
                    {
                        {"count", 0},
                        {"gender", "female"},
                        {"genitals", 1}
                    },
                    "She (who has just one boob) said: You have no notifications. Have a nice day!"
                };
                yield return new object[]
                {
                    case5,
                    new Dictionary<string, object>
                    {
                        {"count", 0},
                        {"gender", "female"},
                        {"genitals", 2}
                    },
                    "She (who has a pair of lovelies) said: You have no notifications. Have a nice day!"
                };
                yield return new object[]
                {
                    case5,
                    new Dictionary<string, object>
                    {
                        {"count", 0},
                        {"gender", "female"},
                        {"genitals", 102}
                    },
                    "She (who has the freakish amount of 102 boobies) said: You have no notifications. Have a nice day!"
                };
                yield return new object[]
                {
                    case5,
                    new Dictionary<string, object>
                    {
                        {"count", 42},
                        {"gender", "female"},
                        {"genitals", 102}
                    },
                    "She (who has the freakish amount of 102 boobies) said: You have a universal amount of notifications. Have a nice day!"
                };
                yield return new object[]
                {
                    case5,
                    new Dictionary<string, object>
                    {
                        {"count", 1},
                        {"gender", "male"},
                        {"genitals", 102}
                    },
                    "He (who has the insane amount of 102 testicles) said: You have just one notification. Have a nice day!"
                };
            }
        }
    }
}