// IFormatterLibrary.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.
namespace Jeffijoe.MessageFormat.Formatting
{
    /// <summary>
    /// Manages formatters to use.
    /// </summary>
    public interface IFormatterLibrary
    {
        /// <summary>
        /// Adds the specified formatter, making it available to use for whoever wants to.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        void Add(IFormatter formatter);

        /// <summary>
        /// Removes the specified formatter. Why you doin' me like this eh?
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        void Remove(IFormatter formatter);

        /// <summary>
        /// Gets the formatter to use. If none was found, throws an exception.
        /// </summary>
        /// <param name="request">The parameters.</param>
        /// <returns></returns>
        IFormatter GetFormatter(FormatterRequest request);
    }
}