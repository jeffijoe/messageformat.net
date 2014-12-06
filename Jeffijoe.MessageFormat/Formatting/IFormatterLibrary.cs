// IFormatterLibrary.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System.Collections.Generic;

namespace Jeffijoe.MessageFormat.Formatting
{
    /// <summary>
    /// Manages formatters to use.
    /// </summary>
    public interface IFormatterLibrary : IList<IFormatter>
    {
        /// <summary>
        /// Gets the formatter to use. If none was found, throws an exception.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        IFormatter GetFormatter(FormatterRequest request);
    }
}