namespace Jeffijoe.MessageFormat.Formatting
{
    /// <summary>
    /// Contains extensions to be used by formatters.
    /// Example, the offset extension for the Plural Format.
    /// </summary>
    public class FormatterExtension
    {
        /// <summary>
        /// Gets the extension.
        /// </summary>
        /// <value>
        /// The extension.
        /// </value>
        public string Extension { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatterExtension"/> class.
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <param name="value">The value.</param>
        public FormatterExtension(string extension, string value)
        {
            Extension = extension;
            Value = value;
        }
    }
}