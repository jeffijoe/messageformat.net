// FormatterLibrary.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System.Collections.Generic;
using System.Linq;
using Jeffijoe.MessageFormat.Formatting.Formatters;

namespace Jeffijoe.MessageFormat.Formatting
{
    /// <summary>
    /// Manages formatters to use.
    /// </summary>
    public class FormatterLibrary : List<IFormatter>, IFormatterLibrary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatterLibrary"/> class, and adds the default formatters.
        /// </summary>
        public FormatterLibrary()
        {
            Add(new VariableFormatter());
            Add(new SelectFormatter());
            Add(new PluralFormatter());
        }

        /// <summary>
        /// Gets the formatter to use. If none was found, throws an exception.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <exception cref="FormatterNotFoundException"></exception>
        public IFormatter GetFormatter(FormatterRequest request)
        {
            var formatter = this.FirstOrDefault(x => x.CanFormat(request));
            if(formatter == null) throw new FormatterNotFoundException(request);
            return formatter;
        }
    }
}