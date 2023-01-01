// MessageFormat for .NET
// - FormatterLibrary.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System.Collections.Generic;
using Jeffijoe.MessageFormat.Formatting.Formatters;

namespace Jeffijoe.MessageFormat.Formatting
{
    /// <summary>
    ///     Manages formatters to use.
    /// </summary>
    public class FormatterLibrary : List<IFormatter>, IFormatterLibrary
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FormatterLibrary" /> class, and adds the default formatters.
        /// </summary>
        public FormatterLibrary()
        {
            this.Add(new VariableFormatter());
            this.Add(new SelectFormatter());
            this.Add(new PluralFormatter());
            this.Add(new NumberFormatter());
            this.Add(new DateFormatter());
            this.Add(new TimeFormatter());
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the formatter to use. If none was found, throws an exception.
        /// </summary>
        /// <param name="request">
        ///     The request.
        /// </param>
        /// <returns>
        ///     The <see cref="IFormatter" />.
        /// </returns>
        /// <exception cref="FormatterNotFoundException">
        ///     Thrown when the formatter was not found.
        /// </exception>
        public IFormatter GetFormatter(FormatterRequest request)
        {
            foreach (var formatter in this)
            {
                if (formatter.CanFormat(request))
                    return formatter;
            }

            throw new FormatterNotFoundException(request);
        }

        #endregion
    }
}