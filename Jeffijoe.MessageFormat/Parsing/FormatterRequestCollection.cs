// FormatterRequestCollection.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System;
using System.Collections.Generic;
using Jeffijoe.MessageFormat.Formatting;

namespace Jeffijoe.MessageFormat.Parsing
{
    /// <summary>
    /// Formatter requests collection.
    /// </summary>
    public class FormatterRequestCollection : List<FormatterRequest>, IFormatterRequestCollection
    {
        /// <summary>
        /// Updates the indices of all
        /// formatter requests' source literals, starting at
        /// next request after the specified index in this collection.
        /// </summary>
        /// <param name="indexToStartFrom">The index to start from.</param>
        /// <param name="formatterResultLength">Length of the formatter result.
        /// Used to compare each literal's inner text length, so we know what to set the
        /// indices to on the rest of the requests.</param>
        public void ShiftIndices(int indexToStartFrom, int formatterResultLength)
        {
            var start = this[indexToStartFrom];
            for (int i = indexToStartFrom +1; i < Count; i++)
            {
                var next = this[i];
                // "- 2" will compensate for { and }. (This works, don't ask why).
                next.SourceLiteral.ShiftIndices(formatterResultLength - 2, start.SourceLiteral);
            }
        }
    }
}