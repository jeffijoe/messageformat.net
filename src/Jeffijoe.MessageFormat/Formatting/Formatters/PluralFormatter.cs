// MessageFormat for .NET
// - PluralFormatter.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Jeffijoe.MessageFormat.Formatting.Formatters;

/// <summary>
///     Plural Formatter
/// </summary>
public class PluralFormatter : BaseFormatter, IFormatter
{
    /// <summary>
    ///     CLDR plural type attribute for counting number ruleset.
    /// </summary>
    internal const string CardinalType = "cardinal";

    /// <summary>
    ///     CLDR plural type attribute for ordered number ruleset.
    /// </summary>
    internal const string OrdinalType = "ordinal";

    /// <summary>
    ///     ICU MessageFormat function name for "default" pluralization, based on cardinal numbers.
    /// </summary>
    internal const string PluralFunction = "plural";

    /// <summary>
    ///     ICU MessageFormat function name for ordinal pluralization.
    /// </summary>
    internal const string OrdinalFunction = "selectordinal";

    /// <summary>
    ///     Maps supported parser names to CLDR plural types.
    ///     The plural language rule schema is identical between these types and we just need to pick the correct set.
    /// </summary>
    private static readonly Dictionary<string, string> CldrTypeForFunction = new()
    {
        { PluralFunction, CardinalType },
        { OrdinalFunction, OrdinalType }
    };

    #region Constructors and Destructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="PluralFormatter" /> class.
    /// </summary>
    public PluralFormatter()
    {
        this.Pluralizers = new Dictionary<PluralRuleKey, Pluralizer>();
        this.AddStandardPluralizers();
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///     This formatter requires the input variable to exist.
    /// </summary>
    public bool VariableMustExist => true;

    /// <summary>
    ///     Gets the pluralizers dictionary. Key is the locale and plural type.
    /// </summary>
    /// <value>
    ///     The pluralizers.
    /// </value>
    public IDictionary<PluralRuleKey, Pluralizer> Pluralizers { get; private set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///     Determines whether this instance can format a message based on the specified parameters.
    /// </summary>
    /// <param name="request">
    ///     The parameters.
    /// </param>
    /// <returns>
    ///     The <see cref="bool" />.
    /// </returns>
    public bool CanFormat(FormatterRequest request)
    {
        if (request.FormatterName is null)
        {
            return false;
        }

        return CldrTypeForFunction.ContainsKey(request.FormatterName);
    }

    /// <summary>
    ///     Using the specified parameters and arguments, a formatted string shall be returned.
    ///     The <see cref="IMessageFormatter" /> is being provided as well, to enable
    ///     nested formatting. This is only called if <see cref="CanFormat" /> returns true.
    ///     The args will always contain the <see cref="FormatterRequest.Variable" />.
    /// </summary>
    /// <param name="locale">
    ///     The locale being used. It is up to the formatter what they do with this information.
    /// </param>
    /// <param name="request">
    ///     The parameters.
    /// </param>
    /// <param name="args">
    ///     The arguments.
    /// </param>
    /// <param name="value">The value of <see cref="FormatterRequest.Variable"/> from the given args dictionary. Can be null.</param>
    /// <param name="messageFormatter">
    ///     The message formatter.
    /// </param>
    /// <returns>
    ///     The <see cref="string" />.
    /// </returns>
    /// <exception cref="MessageFormatterException">
    ///     If <paramref name="request"/> does not specify a formatter name supported by <see cref="CanFormat(FormatterRequest)"/>.
    /// </exception>
    public string Format(string locale,
        FormatterRequest request,
        IReadOnlyDictionary<string, object?> args,
        object? value,
        IMessageFormatter messageFormatter)
    {
        var arguments = this.ParseArguments(request);
        double offset = 0;
        var offsetExtension = arguments.Extensions.FirstOrDefault(x => x.Extension == "offset");
        if (offsetExtension != null)
        {
            offset = Convert.ToDouble(offsetExtension.Value);
        }

        // Get CLDR plural ruleset from request.
        // CanFormat() should have guaranteed this is valid, but we'll be defensive just in case.
        if (!CldrTypeForFunction.TryGetValue(request.FormatterName ?? string.Empty, out var pluralType))
        {
            throw new MessageFormatterException($"Unsupported plural formatter name: {request.FormatterName}");
        }

        var ctx = CreatePluralContext(value, offset);
        var pluralized = this.Pluralize(
            new PluralRuleKey(PluralType: pluralType, Locale: locale),
            arguments,
            ctx,
            offset);
        var result = this.ReplaceNumberLiterals(pluralized, ctx.Number);
        var formatted = messageFormatter.FormatMessage(result, args);
        return formatted;
    }

    #endregion

    #region Methods

    /// <summary>
    ///     Returns the correct plural block.
    /// </summary>
    /// <param name="ruleKey">
    ///     The locale and pluralType.
    /// </param>
    /// <param name="arguments">
    ///     The parsed arguments string.
    /// </param>
    /// <param name="context">
    ///     The plural context.
    /// </param>
    /// <param name="offset">
    ///     The offset (already applied in context).
    /// </param>
    /// <returns>
    ///     The <see cref="string" />.
    /// </returns>
    /// <exception cref="MessageFormatterException">
    ///     The 'other' option was not found in pattern.
    /// </exception>
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]
    internal string Pluralize(PluralRuleKey ruleKey, ParsedArguments arguments, PluralContext context, double offset)
    {
        string pluralForm;
        if (this.Pluralizers.TryGetValue(ruleKey, out var pluralizer))
        {
            pluralForm = pluralizer(context.Number);
        }
        else if (PluralRulesMetadata.TryGetRuleByLocale(ruleKey, out var contextPluralizer))
        {
            pluralForm = contextPluralizer(context);
        }
        else
        {
            pluralForm = this.Pluralizers[new PluralRuleKey(Locale: "en", PluralType: ruleKey.PluralType)](context.Number);
        }
            
        KeyedBlock? other = null;
        foreach (var keyedBlock in arguments.KeyedBlocks)
        {
            if (keyedBlock.Key == OtherKey)
            {
                other = keyedBlock;
            }

            if (keyedBlock.Key.StartsWith("="))
            {
                var numberLiteral = Convert.ToDouble(keyedBlock.Key.Substring(1));

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (numberLiteral == context.Number + offset)
                {
                    return keyedBlock.BlockText;
                }
            }

            if (keyedBlock.Key == pluralForm)
            {
                return keyedBlock.BlockText;
            }
        }

        if (other == null)
        {
            throw new MessageFormatterException("'other' option not found in pattern.");
        }

        return other.BlockText;
    }

    /// <summary>
    ///     Replaces the number literals with the actual number.
    /// </summary>
    /// <param name="pluralized">
    ///     The pluralized.
    /// </param>
    /// <param name="n">
    ///     The n.
    /// </param>
    /// <returns>
    ///     The <see cref="string" />.
    /// </returns>
    internal string ReplaceNumberLiterals(string pluralized, double n)
    {
        var sb = StringBuilderPool.Get();

        try
        {
            // I've done this a few times now..
            const char OpenBrace = '{';
            const char CloseBrace = '}';
            const char Pound = '#';
            const char EscapeChar = '\'';
            var braceBalance = 0;
            var insideEscapeSequence = false;
            for (int i = 0; i < pluralized.Length; i++)
            {
                var c = pluralized[i];

                if (c == EscapeChar)
                {
                    // Append it anyway because the escae
                    sb.Append(EscapeChar);

                    if (i == pluralized.Length - 1)
                    {
                        // The last char can't open a new escape sequence, it can only close one
                        if (insideEscapeSequence)
                        {
                            insideEscapeSequence = false;
                        }

                        continue;
                    }

                    var nextChar = pluralized[i + 1];
                    if (nextChar == EscapeChar)
                    {
                        sb.Append(EscapeChar);
                        ++i;
                        continue;
                    }

                    if (insideEscapeSequence)
                    {
                        insideEscapeSequence = false;
                        continue;
                    }

                    if (nextChar is '{' or '}' or '#')
                    {
                        sb.Append(nextChar);
                        insideEscapeSequence = true;
                        ++i;
                    }

                    continue;
                }

                if (insideEscapeSequence)
                {
                    sb.Append(c);
                    continue;
                }

                if (c == OpenBrace)
                {
                    braceBalance++;
                }
                else if (c == CloseBrace)
                {
                    braceBalance--;
                }
                else if (c == Pound)
                {
                    if (braceBalance == 0)
                    {
                        sb.Append(n);
                        continue;
                    }
                }

                sb.Append(c);
            }

            return sb.ToString();
        }
        finally
        {
            StringBuilderPool.Return(sb);
        }
    }

    /// <summary>
    ///     Adds the standard pluralizers.
    /// </summary>
    private void AddStandardPluralizers()
    {
        this.Pluralizers.Add(
            PluralRuleKey.Cardinal("en"),
            n =>
            {
                // ReSharper disable CompareOfFloatsByEqualityOperator
                if (n == 0)
                {
                    return "zero";
                }

                if (n == 1)
                {
                    return "one";
                }

                // ReSharper restore CompareOfFloatsByEqualityOperator
                return "other";
            }
        );
        this.Pluralizers.Add(
            PluralRuleKey.Ordinal("en"),
            n =>
            {
                // e.g., 1st
                if (n % 10 == 1 && n % 100 != 11)
                {
                    return "one";
                }

                // e.g., 2nd
                if (n % 10 == 2 && n % 100 != 12)
                {
                    return "two";
                }

                // e.g., 3rd
                if (n % 10 == 3 && n % 100 != 13)
                {
                    return "few";
                }

                // e.g., 4th, 11th, etc
                return "other";
            }
        );
    }

    /// <summary>
    ///     Creates a <see cref="PluralContext"/> for the specified value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    [ExcludeFromCodeCoverage]
    private static PluralContext CreatePluralContext(object? value, double offset)
    {
        if (offset == 0)
        {
            if (value is string v)
            {
                return new PluralContext(v);
            }

            if (value is int i)
            {
                return new PluralContext(i);
            }

            if (value is decimal d)
            {
                return new PluralContext(d);
            }

            return new PluralContext(Convert.ToDouble(value));
        }

        return new PluralContext(Convert.ToDouble(value) - offset);
    }

    #endregion
}