// MessageFormat for .NET
// - VariableFormatter.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;

namespace Jeffijoe.MessageFormat.Formatting.Formatters
{
    /// <summary>
    ///     Simple variable replacer.
    /// </summary>
    public class VariableFormatter : IFormatter
    {
        #region Fields

        private readonly ConcurrentDictionary<string, CultureInfo> cultures = new ConcurrentDictionary<string, CultureInfo>();

        #endregion

        #region Public Properties

        /// <summary>
        ///     This formatter requires the input variable to exist.
        /// </summary>
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
            return request.FormatterName == null;
        }

        /// <summary>
        /// Using the specified parameters and arguments, a formatted string shall be returned.
        /// The <see cref="IMessageFormatter" /> is being provided as well, to enable
        /// nested formatting. This is only called if <see cref="CanFormat" /> returns true.
        /// The args will always contain the <see cref="FormatterRequest.Variable" />.
        /// </summary>
        /// <param name="locale">The locale being used. It is up to the formatter what they do with this information.</param>
        /// <param name="request">The parameters.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="value">The value of <see cref="FormatterRequest.Variable" /> from the given args dictionary. Can be null.</param>
        /// <param name="messageFormatter">The message formatter.</param>
        /// <returns>
        /// The <see cref="string" />.
        /// </returns>
        public string Format(
            string locale, 
            FormatterRequest request,
            IDictionary<string, object?> args, 
            object? value,
            IMessageFormatter messageFormatter)
        {
            switch (value)
            {
                case IFormattable formattable:
                    return formattable.ToString(null, GetCultureInfo(locale));
                default:
                    return value?.ToString() ?? string.Empty;
            }
        }

        /// <summary>
        /// Get and cache the culture for a locale.
        /// </summary>
        /// <param name="locale">Locale for which to get the culture.</param>
        /// <returns>
        /// Culture of locale.
        /// </returns>
        private CultureInfo GetCultureInfo(string locale)
        {
            if (!this.cultures.ContainsKey(locale))
            {
                this.cultures[locale] = new CultureInfo(locale);
            }
            return this.cultures[locale];
        }

        #endregion
    }
}