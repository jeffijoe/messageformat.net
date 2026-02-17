// MessageFormat for .NET
// - PluralFormatter.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using Jeffijoe.MessageFormat.Helpers;
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
    ///     ICU MessageFormat function name for "default" pluralization, based on cardinal numbers.
    /// </summary>
    internal const string PluralFunction = "plural";

    /// <summary>
    ///     ICU MessageFormat function name for ordinal pluralization.
    /// </summary>
    internal const string OrdinalFunction = "selectordinal";

    /// <summary>
    ///     Delegate type to try to look up a specific plural rule for a given locale.
    /// </summary>
    internal delegate bool TryGetRuleForLocale(string locale, [NotNullWhen(true)] out ContextPluralizer? contextPluralizer);

    #region Constructors and Destructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="PluralFormatter" /> class.
    /// </summary>
    public PluralFormatter()
    {
        this.CardinalPluralizers = new Dictionary<string, Pluralizer>();
        this.OrdinalPluralizers = new Dictionary<string, Pluralizer>();
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///     This formatter requires the input variable to exist.
    /// </summary>
    public bool VariableMustExist => true;

    /// <summary>
    ///     Gets the pluralizers dictionary to use for cardinal numbers. Key is the locale.
    /// </summary>
    /// <value>
    ///     The pluralizers.
    /// </value>
    public IDictionary<string, Pluralizer> CardinalPluralizers { get; }

    /// <summary>
    ///     Gets the pluralizers dictionary to use for ordinal numbers. Key is the locale.
    /// </summary>
    /// <value>
    ///     The ordinal pluralizers.
    /// </value>
    public IDictionary<string, Pluralizer> OrdinalPluralizers { get; }

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

        return request.FormatterName == PluralFunction || request.FormatterName == OrdinalFunction;
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
        TryGetRuleForLocale cldrPluralLookup;
        IDictionary<string, Pluralizer> customLookup;
        if (request.FormatterName == PluralFunction)
        {
            cldrPluralLookup = PluralRulesMetadata.TryGetCardinalRuleByLocale;
            customLookup = this.CardinalPluralizers;
        }
        else if (request.FormatterName == OrdinalFunction)
        {
            cldrPluralLookup = PluralRulesMetadata.TryGetOrdinalRuleByLocale;
            customLookup = this.OrdinalPluralizers;
        }
        else
        {
            throw new MessageFormatterException($"Unsupported plural formatter name: {request.FormatterName}");
        }

        var ctx = CreatePluralContext(value, offset);
        var pluralized = this.Pluralize(
            locale,
            cldrPluralLookup,
            customLookup,
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
    /// <param name="locale">
    ///     The locale.
    /// </param>
    /// <param name="cldrPluralLookup">
    ///     Delegate to retrieve a <see cref="ContextPluralizer"/> for a given locale.
    /// </param>
    /// <param name="customLookup">
    ///     Dictionary to retrieve a <see cref="Pluralizer"/> for a given locale, to be evaluated
    ///     before resolving against <paramref name="cldrPluralLookup"/>.
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
    ///     The 'other' option was not found in pattern, or <paramref name="cldrPluralLookup"/> is missing
    ///     both the provided locale and the CLDR root locale.
    /// </exception>
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]
    internal string Pluralize(
        string locale,
        TryGetRuleForLocale cldrPluralLookup,
        IDictionary<string, Pluralizer> customLookup,
        ParsedArguments arguments,
        PluralContext context,
        double offset)
    {
        string? pluralForm = null;
        if (customLookup.TryGetValue(locale, out var pluralizer))
        {
            pluralForm = pluralizer(context.Number);
        }
        else
        {
            foreach (var candidate in LocaleHelper.GetInheritanceChain(locale))
            {
                if (cldrPluralLookup(candidate, out var contextPluralizer))
                {
                    pluralForm = contextPluralizer(context);
                    break;
                }
            }
        }

        if (pluralForm is null)
        {
            // GetInheritanceChain should resolve the root CLDR locale as a last attempt, so this should never happen...
            throw new MessageFormatterException($"Could not find locale {locale} in specified plural rule lookup");
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