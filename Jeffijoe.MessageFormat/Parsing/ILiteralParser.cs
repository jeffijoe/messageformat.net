// MessageFormat for .NET
// - ILiteralParser.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System.Collections.Generic;
using System.Text;

namespace Jeffijoe.MessageFormat.Parsing
{
    /// <summary>
    ///     Brace parser contract.
    /// </summary>
    public interface ILiteralParser
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Finds the brace matches.
        /// </summary>
        /// <param name="sb">
        ///     The sb.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        IEnumerable<Literal> ParseLiterals(StringBuilder sb);

        #endregion
    }
}