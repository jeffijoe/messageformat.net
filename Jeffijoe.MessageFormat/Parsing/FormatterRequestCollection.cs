// MessageFormat for .NET
// - FormatterRequestCollection.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System.Collections.Generic;

using Jeffijoe.MessageFormat.Formatting;

namespace Jeffijoe.MessageFormat.Parsing
{
    /// <summary>
    ///     Formatter requests collection.
    /// </summary>
    public class FormatterRequestCollection : List<FormatterRequest>, IFormatterRequestCollection
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Clones this instance and all of it's items. This lets us reuse pattern parsing result, without having to remember
        ///     the item's initial state before being modified to match the results of the formatters.
        /// </summary>
        /// <returns>
        ///     The <see cref="IFormatterRequestCollection" />.
        /// </returns>
        public IFormatterRequestCollection Clone()
        {
            var result = new FormatterRequestCollection();
            foreach (var request in this)
            {
                result.Add(request.Clone());
            }

            return result;
        }

        /// <summary>
        ///     Updates the indices of all
        ///     formatter requests' source literals, starting at
        ///     next request after the specified index in this collection.
        /// </summary>
        /// <param name="indexToStartFrom">
        ///     The index to start from.
        /// </param>
        /// <param name="formatterResultLength">
        ///     Length of the formatter result.
        ///     Used to compare each literal's inner text length, so we know what to set the
        ///     indices to on the rest of the requests.
        /// </param>
        public void ShiftIndices(int indexToStartFrom, int formatterResultLength)
        {
            var start = this[indexToStartFrom];

            // "- 2" will compensate for { and }. (This works, don't ask why).
            int resultLength = formatterResultLength - 2;
            for (int i = indexToStartFrom + 1; i < this.Count; i++)
            {
                var next = this[i];
                next.SourceLiteral.ShiftIndices(resultLength, start.SourceLiteral);
            }
        }

        #endregion
    }
}