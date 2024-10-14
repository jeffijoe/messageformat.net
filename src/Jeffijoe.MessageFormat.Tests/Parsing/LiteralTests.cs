// MessageFormat for .NET
// - LiteralTests.cs
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using Jeffijoe.MessageFormat.Parsing;

using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Parsing;

/// <summary>
/// The literal tests.
/// </summary>
public class LiteralTests
{
    #region Public Methods and Operators

    /// <summary>
    /// The shift indices.
    /// </summary>
    [Fact]
    public void ShiftIndices()
    {
        var subject = new Literal(20, 29, 1, 1, new string('a', 10));
        var other = new Literal(5, 10, 1, 1, new string('a', 6));

        subject.ShiftIndices(2, other);

        // I honestly have no explanation for this, but it works with the formatter. Magic?
        Assert.Equal(18, subject.StartIndex);
        Assert.Equal(27, subject.EndIndex);
    }

    #endregion
}