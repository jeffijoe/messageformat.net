// MessageFormat for .NET
// - FormatterLibraryTests.cs
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Parsing;
using Jeffijoe.MessageFormat.Tests.TestHelpers;
using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Formatting;

/// <summary>
/// The formatter library tests.
/// </summary>
public class FormatterLibraryTests
{
    #region Public Methods and Operators

    /// <summary>
    /// The get formatter.
    /// </summary>
    [Fact]
    public void GetFormatter()
    {
        var subject = new FormatterLibrary();
            
        var req = new FormatterRequest(new Literal(1, 1, 1, 1, ""), "test", "dawg", null);
        var formatter1 = new FakeFormatter();
        var formatter2 = new FakeFormatter();
            
        subject.Add(formatter1);
        subject.Add(formatter2);

        Assert.Throws<FormatterNotFoundException>(() => subject.GetFormatter(req));

        formatter2.SetCanFormat(true);
            
        var actual = subject.GetFormatter(req);
        Assert.Same(formatter2, actual);

        formatter1.SetCanFormat(true);
        actual = subject.GetFormatter(req);
        Assert.Same(formatter1, actual);
    }

    #endregion

    #region Fakes

    #endregion
}