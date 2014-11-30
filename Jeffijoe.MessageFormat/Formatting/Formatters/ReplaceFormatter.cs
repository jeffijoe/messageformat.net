// ReplaceFormatter.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System.Collections.Generic;

namespace Jeffijoe.MessageFormat.Formatting.Formatters
{
    /// <summary>
    /// Simple variable replacer.
    /// </summary>
    public class ReplaceFormatter : IFormatter
    {
        /// <summary>
        /// Determines whether this instance can format a message based on the specified parameters.
        /// </summary>
        /// <param name="request">The parameters.</param>
        /// <returns></returns>
        public bool CanFormat(FormatterRequest request)
        {
            return request.FormatterName == null;
        }

        /// <summary>
        /// Using the specified parameters and arguments, a formatted string shall be returned.
        /// The <see cref="IMessageFormatter" /> is being provided as well, to enable
        /// nested formatting. This is only called if <see cref="CanFormat" /> returns true.
        /// The <see cref="args" /> will always contain the <see cref="FormatterRequest.Variable" />.
        /// </summary>
        /// <param name="locale">The locale being used. It is up to the formatter what they do with this information.</param>
        /// <param name="request">The parameters.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="messageFormatter">The message formatter.</param>
        /// <returns></returns>
        public string Format(string locale, FormatterRequest request, Dictionary<string, object> args, IMessageFormatter messageFormatter)
        {
            return args[request.Variable].ToString();
        }
    }
}