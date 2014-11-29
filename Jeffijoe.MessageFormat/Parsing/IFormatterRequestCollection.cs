using System.Collections.Generic;
using Jeffijoe.MessageFormat.Formatting;

namespace Jeffijoe.MessageFormat.Parsing
{
    /// <summary>
    /// Formatter requests collection.
    /// </summary>
    public interface IFormatterRequestCollection : IEnumerable<FormatterRequest>
    {
        /// <summary>
        /// Updates the indices of all
        /// formatter requests' source literals, starting at
        /// the specified index in this collection.
        /// </summary>
        /// <param name="indexToStartFrom">The index to start from.</param>
        /// <param name="formatterResultLength">
        /// Length of the formatter result.
        /// Used to compare each literal's inner text length, so we know what to set the 
        /// indices to on the rest of the requests.
        /// </param>
        void ShiftIndices(int indexToStartFrom, int formatterResultLength);
    }
}