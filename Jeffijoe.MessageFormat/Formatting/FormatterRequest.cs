// FormatterRequest.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using Jeffijoe.MessageFormat.Parsing;

namespace Jeffijoe.MessageFormat.Formatting
{
    /// <summary>
    /// Formatter request.
    /// </summary>
    public class FormatterRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatterRequest"/> class.
        /// </summary>
        /// <param name="sourceLiteral"></param>
        /// <param name="variable">The variable.</param>
        /// <param name="formatterName">Name of the formatter.</param>
        /// <param name="formatterArguments">The formatter arguments.</param>
        public FormatterRequest(
            Literal sourceLiteral,
            string variable, 
            string formatterName, 
            string formatterArguments)
        {
            SourceLiteral = sourceLiteral;
            Variable = variable;
            FormatterName = formatterName;
            FormatterArguments = formatterArguments;
        }

        /// <summary>
        /// Gets the source literal.
        /// </summary>
        /// <value>
        /// The source literal.
        /// </value>
        public Literal SourceLiteral { get; private set; }

        /// <summary>
        /// Gets the variable name. Never null.
        /// </summary>
        /// <value>
        /// The variable.
        /// </value>
        public string Variable { get; private set; }

        /// <summary>
        /// Gets the name of the formatter to use . e.g. 'select', 'plural'. Can be null.
        /// </summary>
        /// <value>
        /// The name of the formatter.
        /// </value>
        public string FormatterName { get; private set; }

        /// <summary>
        /// Gets the formatter arguments that the formatter implementation will parse. Can be null.
        /// </summary>
        /// <value>
        /// The formatter arguments.
        /// </value>
        public string FormatterArguments { get; private set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public FormatterRequest Clone()
        {
            return new FormatterRequest(SourceLiteral.Clone(), Variable, FormatterName, FormatterArguments);
        }
    }
}