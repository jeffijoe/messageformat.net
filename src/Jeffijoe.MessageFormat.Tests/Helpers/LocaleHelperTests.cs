using Jeffijoe.MessageFormat.Formatting.Formatters;
using Jeffijoe.MessageFormat.Helpers;
using System.Linq;
using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Helpers;

/// <summary>
///     The locale helper tests.
/// </summary>
public class LocaleHelperTests
{
    /// <summary>
    ///     Tests that both '-' and '_' are supported when extracting the base language.
    /// </summary>
    [Fact]
    public void GetInheritanceChain_HandlesBothSeparators()
    {
        Assert.Equal(
            ["en-US", "en", PluralRulesMetadata.RootLocale],
            LocaleHelper.GetInheritanceChain("en-US").ToList()
        );

        Assert.Equal(
            ["en_US", "en", PluralRulesMetadata.RootLocale],
            LocaleHelper.GetInheritanceChain("en_US").ToList()
        );
    }

    /// <summary>
    ///     Confirms that our implementation only returns the original locale,
    ///     the language, and the root.
    /// </summary>
    /// <remarks>
    ///     This is a perf optimization given the CLDR data set we're using.
    /// </remarks>
    [Fact]
    public void GetInheritanceChain_SkipsIntermediateTags()
    {
        Assert.Equal(
            ["th-TH-u-nu-thai", "th", PluralRulesMetadata.RootLocale],
            LocaleHelper.GetInheritanceChain("th-TH-u-nu-thai").ToList()
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("-")]
    [InlineData("_")]
    [InlineData("x")]
    [InlineData("x-")]
    [InlineData("x-test")]
    [InlineData("i-test")]
    public void GetInheritanceChain_HandlesBadInput(string input)
    {
        Assert.Equal(
            [PluralRulesMetadata.RootLocale],
            LocaleHelper.GetInheritanceChain(input).ToList()
        );
    }
}