// MessageFormat for .NET
// - SelectFormatter.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Jeffijoe.MessageFormat.Formatting.Formatters
{
    /// <summary>
    ///     Implementation of the SelectFormat.
    /// </summary>
    public class SelectFormatter : BaseFormatter, IFormatter
    {
        #region Public Properties

        /// <summary>
        ///     This formatter requires the input variable to exist.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public bool VariableMustExist => true;
        
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
            return request.FormatterName == "select";
        }

        /// <summary>
        /// Using the specified parameters and arguments, a formatted string shall be returned.
        /// The <see cref="IMessageFormatter" /> is being provided as well, to enable
        /// nested formatting. This is only called if <see cref="CanFormat" /> returns true.
        /// The argswill always contain the <see cref="FormatterRequest.Variable" />.
        /// </summary>
        /// <param name="locale">The locale being used. It is up to the formatter what they do with this information.</param>
        /// <param name="request">The parameters.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="value">The value of <see cref="FormatterRequest.Variable" /> from the given args dictionary. Can be null.</param>
        /// <param name="messageFormatter">The message formatter.</param>
        /// <returns>
        /// The <see cref="string" />.
        /// </returns>
        /// <exception cref="MessageFormatterException">'other' option not found in pattern, and variable was not present in collection.</exception>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        public string Format(string locale,
            FormatterRequest request,
            IReadOnlyDictionary<string, object?> args,
            object? value,
            IMessageFormatter messageFormatter)
        {
            var parsed = this.ParseArguments(request);
            KeyedBlock? other = null;
            foreach (var keyedBlock in parsed.KeyedBlocks)
            {
                var str = Convert.ToString(value);
                if (str == keyedBlock.Key)
                {
                    return messageFormatter.FormatMessage(keyedBlock.BlockText, args);
                }

                if (keyedBlock.Key == OtherKey)
                {
                    other = keyedBlock;
                }
            }

            if (other == null)
            {
                throw new MessageFormatterException(
                    "'other' option not found in pattern, and variable was not present in collection.");
            }

            return messageFormatter.FormatMessage(other.BlockText, args);
        }

        #endregion
    }
}