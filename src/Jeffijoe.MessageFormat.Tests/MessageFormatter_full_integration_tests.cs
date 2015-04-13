// MessageFormat for .NET
// - MessageFormatter_full_integration_tests.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System;
using System.Collections.Generic;

using Jeffijoe.MessageFormat.Tests.TestHelpers;

using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions;

namespace Jeffijoe.MessageFormat.Tests
{
    public class MessageFormatter_full_integration_tests
    {
        private readonly ITestOutputHelper outputHelper;

        public MessageFormatter_full_integration_tests(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
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

                // Please take the following sample in the spirit it was intended. :)
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
                yield return
                    new object[]
                    {
                        case1, new Dictionary<string, object> { { "gender", "male" }, { "name", "Jeff" } }, 
                        "He - {Jeff} - said: You're pretty cool!"
                    };
                yield return
                    new object[]
                    {
                        case2, new Dictionary<string, object> { { "gender", "male" }, { "name", "Jeff" }, { "count", 0 } }, 
                        "He - {Jeff} - said: You have no notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        case2, 
                        new Dictionary<string, object> { { "gender", "female" }, { "name", "Amanda" }, { "count", 1 } }, 
                        "She - {Amanda} - said: You have just one notification. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        case2, new Dictionary<string, object> { { "gender", "uni" }, { "count", 42 } }, 
                        "They said: You have a universal amount of notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        case3, new Dictionary<string, object> { { "count", 5 } }, 
                        "You have 5 notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        case4, new Dictionary<string, object> { { "count", 5 }, { "gender", "male" } }, 
                        "He said: You have 5 notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        case5, new Dictionary<string, object> { { "count", 5 }, { "gender", "male" }, { "genitals", 0 } }, 
                        "He (who has no testicles) said: You have 5 notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        case5, 
                        new Dictionary<string, object> { { "count", 5 }, { "gender", "female" }, { "genitals", 0 } }, 
                        "She (who has no boobies) said: You have 5 notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        case5, 
                        new Dictionary<string, object> { { "count", 0 }, { "gender", "female" }, { "genitals", 1 } }, 
                        "She (who has just one boob) said: You have no notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        case5, 
                        new Dictionary<string, object> { { "count", 0 }, { "gender", "female" }, { "genitals", 2 } }, 
                        "She (who has a pair of lovelies) said: You have no notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        case5, 
                        new Dictionary<string, object> { { "count", 0 }, { "gender", "female" }, { "genitals", 102 } }, 
                        "She (who has the freakish amount of 102 boobies) said: You have no notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        case5, 
                        new Dictionary<string, object> { { "count", 42 }, { "gender", "female" }, { "genitals", 102 } }, 
                        "She (who has the freakish amount of 102 boobies) said: You have a universal amount of notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        case5, 
                        new Dictionary<string, object> { { "count", 1 }, { "gender", "male" }, { "genitals", 102 } }, 
                        "He (who has the insane amount of 102 testicles) said: You have just one notification. Have a nice day!"
                    };

                // Case from https://github.com/jeffijoe/messageformat.net/issues/2
                yield return
                    new object[]
                    {
                        "{nbrAttachments, plural, zero {} one {{nbrAttachmentsFmt} attachment} other {{nbrAttachmentsFmt} attachments}}", 
                        new Dictionary<string, object> { { "nbrAttachments", 0 }, {"nbrAttachmentsFmt", "wut"} }, 
                        string.Empty
                    };
            }
        }

        [Fact]
        public void FormatMessage_lets_non_ascii_characters_right_through()
        {
            var input = "中test中国话不用彁字。";
            var subject = new MessageFormatter(false);
            var actual = subject.FormatMessage(input, new Dictionary<string, object>());
            Assert.Equal(input, actual);
        }

        [Theory]
        [MemberData("Tests")]
        public void FormatMessage(string source, Dictionary<string, object> args, string expected)
        {
            var subject = new MessageFormatter(false);

            // Warmup
            subject.FormatMessage(source, args);
            Benchmark.Start("Formatting", this.outputHelper);
            string result = subject.FormatMessage(source, args);
            Benchmark.End(this.outputHelper);
            Assert.Equal(expected, result);
            this.outputHelper.WriteLine(result);
        }

        [Fact]
        public void ReadMe_test_to_make_sure_I_dont_look_like_a_fool()
        {
            {
                var mf = new MessageFormatter(false);
                var str = @"You have {notifications, plural,
                              zero {no notifications}
                              one {one notification}
                              =42 {a universal amount of notifications}
                              other {# notifications}
                            }. Have a nice day, {name}!";
                var formatted = mf.FormatMessage(
                    str, 
                    new Dictionary<string, object> { { "notifications", 4 }, { "name", "Jeff" } });
                Assert.Equal("You have 4 notifications. Have a nice day, Jeff!", formatted);
            }
            {
                var mf = new MessageFormatter(false);
                var str = @"You {NUM_ADDS, plural, offset:1
                              =0{didnt add this to your profile} 
                              zero{added this to your profile}
                              one{and one other person added this to their profile}
                              other{and # others added this to their profiles}
                          }.";
                var formatted = mf.FormatMessage(str, new Dictionary<string, object> { { "NUM_ADDS", 0 } });
                Assert.Equal("You didnt add this to your profile.", formatted);

                formatted = mf.FormatMessage(str, new Dictionary<string, object> { { "NUM_ADDS", 1 } });
                Assert.Equal("You added this to your profile.", formatted);

                formatted = mf.FormatMessage(str, new Dictionary<string, object> { { "NUM_ADDS", 2 } });
                Assert.Equal("You and one other person added this to their profile.", formatted);

                formatted = mf.FormatMessage(str, new Dictionary<string, object> { { "NUM_ADDS", 3 } });
                Assert.Equal("You and 2 others added this to their profiles.", formatted);
            }
            {
                var mf = new MessageFormatter(false);
                var str = @"{GENDER, select,
                                male {He}
                              female {She}
                               other {They}
                            } found {NUM_RESULTS, plural,
                                        one {1 result}
                                      other {# results}
                                    } in {NUM_CATEGORIES, plural,
                                              one {1 category}
                                            other {# categories}
                                         }.";
                var formatted = mf.FormatMessage(
                    str, 
                    new Dictionary<string, object>
                    {
                        { "GENDER", "male" }, 
                        { "NUM_RESULTS", 1 }, 
                        { "NUM_CATEGORIES", 2 }
                    });
                Assert.Equal(formatted, "He found 1 result in 2 categories.");

                formatted = mf.FormatMessage(
                    str, 
                    new Dictionary<string, object>
                    {
                        { "GENDER", "male" }, 
                        { "NUM_RESULTS", 1 }, 
                        { "NUM_CATEGORIES", 1 }
                    });
                Assert.Equal(formatted, "He found 1 result in 1 category.");

                formatted = mf.FormatMessage(
                    str, 
                    new Dictionary<string, object>
                    {
                        { "GENDER", "female" }, 
                        { "NUM_RESULTS", 2 }, 
                        { "NUM_CATEGORIES", 1 }
                    });
                Assert.Equal(formatted, "She found 2 results in 1 category.");
            }
            {
                var mf = new MessageFormatter(false);
                var str = @"Your {NUM, plural, one{message} other{messages}} go here.";
                var formatted = mf.FormatMessage(str, new Dictionary<string, object> { { "NUM", 1 } });
                Assert.Equal(formatted, "Your message go here.");

                formatted = mf.FormatMessage(str, new Dictionary<string, object> { { "NUM", 3 } });
                Assert.Equal(formatted, "Your messages go here.");
            }
            {
                var mf = new MessageFormatter(false);
                var str = @"His name is {LAST_NAME}... {FIRST_NAME} {LAST_NAME}";
                var formatted = mf.FormatMessage(
                    str, 
                    new Dictionary<string, object> { { "FIRST_NAME", "James" }, { "LAST_NAME", "Bond" } });
                Assert.Equal(formatted, "His name is Bond... James Bond");
            }
            {
                var mf = new MessageFormatter(false);
                var str = @"{GENDER, select, male{He} female{She} other{They}} liked this.";
                var formatted = mf.FormatMessage(str, new Dictionary<string, object> { { "GENDER", "male" } });
                Assert.Equal(formatted, "He liked this.");

                formatted = mf.FormatMessage(str, new Dictionary<string, object> { { "GENDER", "female" } });
                Assert.Equal(formatted, "She liked this.");

                formatted = mf.FormatMessage(str, new Dictionary<string, object> { { "GENDER", "somethingelse" } });
                Assert.Equal(formatted, "They liked this.");

                formatted = mf.FormatMessage(str, new Dictionary<string, object> { });
                Assert.Equal(formatted, "They liked this.");
            }
            {
                var mf = new MessageFormatter(true, "en");
                mf.Pluralizers["en"] = n => {
                    // ´n´ is the number being pluralized.
                    if (n == 0)
                    {
                        return "zero";
                    }

                    if (n == 1)
                    {
                        return "one";
                    }

                    if (n > 1000)
                    {
                        return "thatsalot";
                    }

                    return "other";
                };

                var actual =
                    mf.FormatMessage(
                        "You have {number, plural, thatsalot {a shitload of notifications} other {# notifications}}", 
                        new Dictionary<string, object> { { "number", 1001 } });
                Assert.Equal("You have a shitload of notifications", actual);
            }
        }

        [Fact]
        public void FormatMessage_with_reflection_overload()
        {
            var subject = new MessageFormatter(false);
            var pattern = "You have {UnreadCount, plural, " + "zero {no unread messages}"
                          + "one {just one unread message}" + "other {# unread messages}" + "} today.";
            var actual = subject.FormatMessage(pattern, new { UnreadCount = 0 });
            Assert.Equal("You have no unread messages today.", actual);

            // The absence of UnreadCount means it will be treated as "zero".
            actual = subject.FormatMessage(pattern, new { });
            Assert.Equal("You have no unread messages today.", actual);

            actual = subject.FormatMessage(pattern, new { UnreadCount = 1 });
            Assert.Equal("You have just one unread message today.", actual);
            actual = subject.FormatMessage(pattern, new { UnreadCount = 2 });
            Assert.Equal("You have 2 unread messages today.", actual);

            actual = subject.FormatMessage(pattern, new { UnreadCount = 3 });
            Assert.Equal("You have 3 unread messages today.", actual);
        }

        [Fact]
        public void FormatMessage_nesting_with_brace_escaping()
        {
            var subject = new MessageFormatter(false);
            var pattern = @"{s1, select, 
                                1 {{s2, select, 
                                   2 {\{}
                                }}
                            }";
            var actual = subject.FormatMessage(pattern, new { s1 = 1, s2 = 2 });
            this.outputHelper.WriteLine(actual);
            Assert.Equal("{", actual);
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
            var args = new Dictionary<string, object> { { "gender", "male" }, { "count", 5 } };
            var subject = new MessageFormatter(false);

            string result = subject.FormatMessage(source, args);
            Assert.Equal(expected, result);
        }
    }
}