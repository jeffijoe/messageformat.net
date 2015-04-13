// MessageFormat for .NET
// - VariableNotFoundException.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.
namespace Jeffijoe.MessageFormat.Formatting
{
    /// <summary>
    ///     Thrown when a variable declared in a pattern was non-existent.
    /// </summary>
    public class VariableNotFoundException : MessageFormatterException
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="VariableNotFoundException" /> class.
        /// </summary>
        /// <param name="variable">
        ///     The variable.
        /// </param>
        public VariableNotFoundException(string variable)
            : base(BuildMessage(variable))
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Builds the message.
        /// </summary>
        /// <param name="variable">
        ///     The variable.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        private static string BuildMessage(string variable)
        {
            return string.Format("The variable '{0}' was not found in the arguments collection.", variable);
        }

        #endregion
    }
}