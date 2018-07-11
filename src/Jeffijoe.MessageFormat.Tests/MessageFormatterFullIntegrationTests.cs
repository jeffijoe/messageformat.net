// MessageFormat for .NET
// - MessageFormatter_full_integration_tests.cs
//
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using System.Collections.Generic;

using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Tests.TestHelpers;

using Xunit;
using Xunit.Abstractions;

namespace Jeffijoe.MessageFormat.Tests
{
    /// <summary>
    /// The message formatter_full_integration_tests.
    /// </summary>
    public class MessageFormatterFullIntegrationTests
    {
        #region Fields

        /// <summary>
        /// The output helper.
        /// </summary>
        private readonly ITestOutputHelper outputHelper;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageFormatterFullIntegrationTests"/> class.
        /// </summary>
        /// <param name="outputHelper">
        /// The output helper.
        /// </param>
        public MessageFormatterFullIntegrationTests(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        #endregion

        #region Public Properties

        public static IEnumerable<object[]> EscapingTests
        {
            get
            {
                yield return
                    new object[]
                    {
                        "This '{isn''t}' obvious",
                        new Dictionary<string, object>(),
                        "This {isn't} obvious"
                    };
                yield return
                    new object[]
                    {
                        "Anna's house has '{0} and # in the roof' and {NUM_COWS} cows.",
                        new Dictionary<string, object> { { "NUM_COWS", 5 } },
                        "Anna's house has {0} and # in the roof and 5 cows."
                    };
                yield return
                    new object[]
                    {
                        "Anna's house has '{'0'} and # in the roof' and {NUM_COWS} cows.",
                        new Dictionary<string, object> { { "NUM_COWS", 5 } },
                        "Anna's house has {0} and # in the roof and 5 cows."
                    };
                yield return
                    new object[]
                    {
                        "Anna's house has '{0}' and '# in the roof' and {NUM_COWS} cows.",
                        new Dictionary<string, object> { { "NUM_COWS", 5 } },
                        "Anna's house has {0} and # in the roof and 5 cows."
                    };
                yield return
                    new object[]
                    {
                        "Anna's house 'has {NUM_COWS} cows'.",
                        new Dictionary<string, object> { { "NUM_COWS", 5 } },
                        "Anna's house 'has 5 cows'."
                    };
                yield return
                    new object[]
                    {
                        "Anna''s house a'{''''b'",
                        new Dictionary<string, object>(),
                        "Anna's house a{''b"
                    };
                yield return
                    new object[]
                    {
                        "a''{NUM_COWS}'b",
                        new Dictionary<string, object> { { "NUM_COWS", 5 } },
                        "a'5'b"
                    };
                yield return
                    new object[]
                    {
                        "a'{NUM_COWS}'b'",
                        new Dictionary<string, object> { { "NUM_COWS", 5 } },
                        "a{NUM_COWS}b'"
                    };
                yield return
                    new object[]
                    {
                        "These '{'braces'}' and thoses '{braces}' ain''t not escaped, which makes a total of {braces, plural, one {a single pair} other {'#'# (=#) pairs}} of escaped braces.",
                        new Dictionary<string, object> { { "braces", 2 } },
                        "These {braces} and thoses {braces} ain't not escaped, which makes a total of #2 (=2) pairs of escaped braces."
                    };
                yield return
                    new object[]
                    {
                        "{num, plural, =1 {1} other {'#'{num, plural, =1 {1} other {'{'#'#'#'}'}}}}",
                        new Dictionary<string, object> { { "num", 2 } },
                        "#{2#2}"
                    };
            }
        }

        /// <summary>
        /// Gets the tests.
        /// </summary>
        public static IEnumerable<object[]> Tests
        {
            get
            {
                const string Case1 = @"{gender, select, 
                           male {He - '{'{name}'}' -}
                           female {She - '{'{name}'}' -}
                           other {They}
                      } said: You're pretty cool!";
                const string Case2 = @"{gender, select, 
                           male {He - '{'{name}'}' -}
                           female {She - '{'{name}'}' -}
                           other {They}
                      } said: You have {count, plural, 
                            zero {no notifications}
                            one {just one notification}
                            =42 {a universal amount of notifications}
                            other {# notifications}
                      }. Have a nice day!";
                const string Case3 = @"You have {count, plural, 
                            zero {no notifications}
                            one {just one notification}
                            =42 {a universal amount of notifications}
                            other {# notifications}
                      }. Have a nice day!";
                const string Case4 = @"{gender, select, 
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
                const string Case5 = @"{gender, select, 
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

                const string Case6 = @"You {count, plural, offset:1,
                                        =0{didn't add this to your profile}
                                        =1{added this to your profile}
                                        one {and one other person added this to their profile}
                                        other {and # others added this to their profiles}
                                        }.";

                yield return
                    new object[]
                    {
                        Case1,
                        new Dictionary<string, object> { { "gender", "male" }, { "name", "Jeff" } },
                        "He - {Jeff} - said: You're pretty cool!"
                    };
                yield return
                    new object[]
                    {
                        Case2,
                        new Dictionary<string, object> { { "gender", "male" }, { "name", "Jeff" }, { "count", 0 } },
                        "He - {Jeff} - said: You have no notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        Case2,
                        new Dictionary<string, object> { { "gender", "female" }, { "name", "Amanda" }, { "count", 1 } },
                        "She - {Amanda} - said: You have just one notification. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        Case2,
                        new Dictionary<string, object> { { "gender", "uni" }, { "count", 42 } },
                        "They said: You have a universal amount of notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        Case3,
                        new Dictionary<string, object> { { "count", 5 } },
                        "You have 5 notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        Case4,
                        new Dictionary<string, object> { { "count", 5 }, { "gender", "male" } },
                        "He said: You have 5 notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        Case5,
                        new Dictionary<string, object> { { "count", 5 }, { "gender", "male" }, { "genitals", 0 } },
                        "He (who has no testicles) said: You have 5 notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        Case5,
                        new Dictionary<string, object> { { "count", 5 }, { "gender", "female" }, { "genitals", 0 } },
                        "She (who has no boobies) said: You have 5 notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        Case5,
                        new Dictionary<string, object> { { "count", 0 }, { "gender", "female" }, { "genitals", 1 } },
                        "She (who has just one boob) said: You have no notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        Case5,
                        new Dictionary<string, object> { { "count", 0 }, { "gender", "female" }, { "genitals", 2 } },
                        "She (who has a pair of lovelies) said: You have no notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        Case5,
                        new Dictionary<string, object> { { "count", 0 }, { "gender", "female" }, { "genitals", 102 } },
                        "She (who has the freakish amount of 102 boobies) said: You have no notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        Case5,
                        new Dictionary<string, object> { { "count", 42 }, { "gender", "female" }, { "genitals", 102 } },
                        "She (who has the freakish amount of 102 boobies) said: You have a universal amount of notifications. Have a nice day!"
                    };
                yield return
                    new object[]
                    {
                        Case5,
                        new Dictionary<string, object> { { "count", 1 }, { "gender", "male" }, { "genitals", 102 } },
                        "He (who has the insane amount of 102 testicles) said: You have just one notification. Have a nice day!"
                    };

                // Case from https://github.com/jeffijoe/messageformat.net/issues/2
                yield return
                    new object[]
                    {
                        "{nbrAttachments, plural, zero {} one {{nbrAttachmentsFmt} attachment} other {{nbrAttachmentsFmt} attachments}}",
                        new Dictionary<string, object> { { "nbrAttachments", 0 }, { "nbrAttachmentsFmt", "wut" } },
                        string.Empty
                    };

                // Following 2 cases from https://github.com/jeffijoe/messageformat.net/issues/4
                yield return
                    new object[]
                    {
                        "{maybeCount}",
                        new Dictionary<string, object> { { "maybeCount", null } },
                        string.Empty
                    };
                yield return
                    new object[]
                    {
                        "{maybeCount}",
                        new Dictionary<string, object> { { "maybeCount", (int?)2 } },
                        "2"
                    };
                yield return
                    new object[]
                    {
                        Case6,
                        new Dictionary<string, object> { { "count", 0 } },
                        "You didn't add this to your profile."
                    };
                yield return
                    new object[]
                    {
                        Case6,
                        new Dictionary<string, object> { { "count", 1 } },
                        "You added this to your profile."
                    };
                yield return
                    new object[]
                    {
                        Case6,
                        new Dictionary<string, object> { { "count", 2 } },
                        "You and one other person added this to their profile."
                    };
                yield return
                    new object[]
                    {
                        Case6,
                        new Dictionary<string, object> { { "count", 3 } },
                        "You and 2 others added this to their profiles."
                    };
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The format message.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <param name="expected">
        /// The expected.
        /// </param>
        [Theory]
        [MemberData(nameof(Tests))]
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

        /// <summary>
        /// The format message_debug.
        /// </summary>
        [Theory]
        [MemberData(nameof(EscapingTests))]
        public void FormatMessage_escaping(string source, Dictionary<string, object> args, string expected)
        {
            var subject = new MessageFormatter(false);

            string result = subject.FormatMessage(source, args);
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// The format message_debug.
        /// </summary>
        [Fact]
        public void FormatMessage_debug()
        {
            const string Source = @"{gender, select, 
                           male {He}
                           female {She}
                           other {They}
                      } said: You have {count, plural, 
                            zero {no notifications}
                            one {just one notification}
                            =42 {a universal amount of notifications}
                            other {# notifications}
                      }. Have a nice day!";
            const string Expected = "He said: You have 5 notifications. Have a nice day!";
            var args = new Dictionary<string, object> { { "gender", "male" }, { "count", 5 } };
            var subject = new MessageFormatter(false);

            string result = subject.FormatMessage(Source, args);
            Assert.Equal(Expected, result);
        }

        /// <summary>
        /// The format message_lets_non_ascii_characters_right_through.
        /// </summary>
        [Fact]
        public void FormatMessage_lets_non_ascii_characters_right_through()
        {
            const string Input = "中test中国话不用彁字。";
            var subject = new MessageFormatter(false);
            var actual = subject.FormatMessage(Input, new Dictionary<string, object>());
            Assert.Equal(Input, actual);
        }

        /// <summary>
        /// The format message_nesting_with_brace_escaping.
        /// </summary>
        [Fact]
        public void FormatMessage_nesting_with_brace_escaping()
        {
            var subject = new MessageFormatter(false);
            const string Pattern = @"{s1, select, 
                                1 {{s2, select, 
                                   2 {'{'}
                                }}
                            }";
            var actual = subject.FormatMessage(Pattern, new { s1 = 1, s2 = 2 });
            this.outputHelper.WriteLine(actual);
            Assert.Equal("{", actual);
        }

        /// <summary>
        /// The format message_with_reflection_overload.
        /// </summary>
        [Fact]
        public void FormatMessage_with_reflection_overload()
        {
            var subject = new MessageFormatter(false);
            const string Pattern = "You have {UnreadCount, plural, "
                                   + "zero {no unread messages}"
                                   + "one {just one unread message}" + "other {# unread messages}" + "} today.";
            var actual = subject.FormatMessage(Pattern, new { UnreadCount = 0 });
            Assert.Equal("You have no unread messages today.", actual);

            // The absence of UnreadCount should make it throw.
            var ex = Assert.Throws<VariableNotFoundException>(() => subject.FormatMessage(Pattern, new { }));
            Assert.Equal("UnreadCount", ex.MissingVariable);

            actual = subject.FormatMessage(Pattern, new { UnreadCount = 1 });
            Assert.Equal("You have just one unread message today.", actual);
            actual = subject.FormatMessage(Pattern, new { UnreadCount = 2 });
            Assert.Equal("You have 2 unread messages today.", actual);

            actual = subject.FormatMessage(Pattern, new { UnreadCount = 3 });
            Assert.Equal("You have 3 unread messages today.", actual);
        }

        /// <summary>
        /// The read me_test_to_make_sure_ i_dont_look_like_a_fool.
        /// </summary>
        [Fact]
        public void ReadMe_test_to_make_sure_I_dont_look_like_a_fool()
        {
            {
                var mf = new MessageFormatter(false);
                const string Str = @"You have {notifications, plural,
                              zero {no notifications}
                              one {one notification}
                              =42 {a universal amount of notifications}
                              other {# notifications}
                            }. Have a nice day, {name}!";
                var formatted = mf.FormatMessage(
                    Str,
                    new Dictionary<string, object> { { "notifications", 4 }, { "name", "Jeff" } });
                Assert.Equal("You have 4 notifications. Have a nice day, Jeff!", formatted);
            }

            {
                var mf = new MessageFormatter(false);
                const string Str = @"You {NUM_ADDS, plural, offset:1
                              =0{didnt add this to your profile} 
                              zero{added this to your profile}
                              one{and one other person added this to their profile}
                              other{and # others added this to their profiles}
                          }.";
                var formatted = mf.FormatMessage(Str, new Dictionary<string, object> { { "NUM_ADDS", 0 } });
                Assert.Equal("You didnt add this to your profile.", formatted);

                formatted = mf.FormatMessage(Str, new Dictionary<string, object> { { "NUM_ADDS", 1 } });
                Assert.Equal("You added this to your profile.", formatted);

                formatted = mf.FormatMessage(Str, new Dictionary<string, object> { { "NUM_ADDS", 2 } });
                Assert.Equal("You and one other person added this to their profile.", formatted);

                formatted = mf.FormatMessage(Str, new Dictionary<string, object> { { "NUM_ADDS", 3 } });
                Assert.Equal("You and 2 others added this to their profiles.", formatted);
            }

            {
                var mf = new MessageFormatter(false);
                const string Str = @"{GENDER, select,
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
                    Str,
                    new Dictionary<string, object>
                    {
                        { "GENDER", "male" },
                        { "NUM_RESULTS", 1 },
                        { "NUM_CATEGORIES", 2 }
                    });
                Assert.Equal("He found 1 result in 2 categories.", formatted);

                formatted = mf.FormatMessage(
                    Str,
                    new Dictionary<string, object>
                    {
                        { "GENDER", "male" },
                        { "NUM_RESULTS", 1 },
                        { "NUM_CATEGORIES", 1 }
                    });
                Assert.Equal("He found 1 result in 1 category.", formatted);

                formatted = mf.FormatMessage(
                    Str,
                    new Dictionary<string, object>
                    {
                        { "GENDER", "female" },
                        { "NUM_RESULTS", 2 },
                        { "NUM_CATEGORIES", 1 }
                    });
                Assert.Equal("She found 2 results in 1 category.", formatted);
            }

            {
                var mf = new MessageFormatter(false);
                const string Str = @"Your {NUM, plural, one{message} other{messages}} go here.";
                var formatted = mf.FormatMessage(Str, new Dictionary<string, object> { { "NUM", 1 } });
                Assert.Equal("Your message go here.", formatted);

                formatted = mf.FormatMessage(Str, new Dictionary<string, object> { { "NUM", 3 } });
                Assert.Equal("Your messages go here.", formatted);
            }

            {
                var mf = new MessageFormatter(false);
                const string Str = @"His name is {LAST_NAME}... {FIRST_NAME} {LAST_NAME}";
                var formatted = mf.FormatMessage(
                    Str,
                    new Dictionary<string, object> { { "FIRST_NAME", "James" }, { "LAST_NAME", "Bond" } });
                Assert.Equal("His name is Bond... James Bond", formatted);
            }

            {
                var mf = new MessageFormatter(false);
                const string Str = @"{GENDER, select, male{He} female{She} other{They}} liked this.";
                var formatted = mf.FormatMessage(Str, new Dictionary<string, object> { { "GENDER", "male" } });
                Assert.Equal("He liked this.", formatted);

                formatted = mf.FormatMessage(Str, new Dictionary<string, object> { { "GENDER", "female" } });
                Assert.Equal("She liked this.", formatted);

                formatted = mf.FormatMessage(Str, new Dictionary<string, object> { { "GENDER", "somethingelse" } });
                Assert.Equal("They liked this.", formatted);

                formatted = mf.FormatMessage(Str, new Dictionary<string, object> { { "GENDER", null } });
                Assert.Equal("They liked this.", formatted);
            }

            {
                var mf = new MessageFormatter(true, "en");
                mf.Pluralizers["en"] = n => {
                    // ´n´ is the number being pluralized.
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
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

        #endregion
    }
}