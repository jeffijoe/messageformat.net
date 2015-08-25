// MessageFormat for .NET
// - CharHelperTests.cs
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using Jeffijoe.MessageFormat.Helpers;

using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Helpers
{
    /// <summary>
    /// The char helper tests.
    /// </summary>
    public class CharHelperTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The is alpha numeric.
        /// </summary>
        [Fact]
        public void IsAlphaNumeric()
        {
            Assert.True('a'.IsAlphaNumeric());
            Assert.True('A'.IsAlphaNumeric());
            Assert.True('0'.IsAlphaNumeric());
            Assert.True('1'.IsAlphaNumeric());
            Assert.False('ä'.IsAlphaNumeric());
            Assert.False('ø'.IsAlphaNumeric());
            Assert.False('æ'.IsAlphaNumeric());
            Assert.False('å'.IsAlphaNumeric());
        }

        #endregion
    }
}