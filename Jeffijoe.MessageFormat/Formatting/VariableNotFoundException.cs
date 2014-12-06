// VariableNotFoundException.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

namespace Jeffijoe.MessageFormat.Formatting
{
    /// <summary>
    /// Thrown when a variable declared in a pattern was non-existent.
    /// </summary>
    public class VariableNotFoundException : MessageFormatterException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableNotFoundException"/> class.
        /// </summary>
        /// <param name="variable">The variable.</param>
        public VariableNotFoundException(string variable)
            : base(BuildMessage(variable))
        {
        }

        /// <summary>
        /// Builds the message.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <returns></returns>
        private static string BuildMessage(string variable)
        {
            return string.Format("The variable '{0}' was not found in the arguments collection.", variable);
        }
    }
}