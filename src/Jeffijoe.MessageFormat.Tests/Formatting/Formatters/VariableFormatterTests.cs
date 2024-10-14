﻿// MessageFormat for .NET
// - VariableFormatterTests.cs
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using System.Collections.Generic;
using Jeffijoe.MessageFormat.Formatting;
using Jeffijoe.MessageFormat.Formatting.Formatters;
using Jeffijoe.MessageFormat.Parsing;
using Jeffijoe.MessageFormat.Tests.TestHelpers;

using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Formatting.Formatters;

/// <summary>
///     The variable formatter tests.
/// </summary>
public class VariableFormatterTests
{
    #region Fields

    /// <summary>
    ///     The subject.
    /// </summary>
    private readonly VariableFormatter subject;

    /// <summary>
    ///     The fake message formatter.
    /// </summary>
    private readonly IMessageFormatter formatter;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="VariableFormatterTests" /> class.
    /// </summary>
    public VariableFormatterTests()
    {
        this.formatter = new FakeMessageFormatter();
        this.subject = new VariableFormatter();
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///     Verifies that an empty string is returned when the argument is null.
    /// </summary>
    [Fact]
    public void VerifyAnEmptyStringIsReturnedWhenTheArgumentIsNull()
    {
        var req = CreateRequest();
        var args = new Dictionary<string, object?>();

        Assert.Equal(string.Empty, this.subject.Format("en", req, args, null, this.formatter));
    }

    /// <summary>
    ///     Verifies that the value from the given arguments is returned as a string.
    /// </summary>
    [Fact]
    public void VerifyTheValueFromTheGivenArgumentsIsReturnedAsAString()
    {
        var req = CreateRequest();
        var args = new Dictionary<string, object?>();

        Assert.Equal("is good", this.subject.Format("en", req, args, "is good", this.formatter));
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Creates the request.
    /// </summary>
    /// <returns>
    ///     The <see cref="FormatterRequest" />.
    /// </returns>
    private static FormatterRequest CreateRequest()
    {
        var req = new FormatterRequest(new Literal(1, 10, 1, 1, ""), "test", null, null);
        return req;
    }

    #endregion
}