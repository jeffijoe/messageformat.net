// MessageFormat for .NET
// - MessageFormatterStringExtensionTests.cs
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using System.Threading.Tasks;

using Xunit;

namespace Jeffijoe.MessageFormat.Tests
{
    /// <summary>
    /// The message formatter string extension tests.
    /// </summary>
    public class MessageFormatterStringExtensionTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The format message_with_multiple_tasks.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task FormatMessage_with_multiple_tasks()
        {
            var pattern = "Copying {fileCount, plural, one {one file} other{# files}}.";

            // 2 with the same message to test there are no issues with caching with multiple threads.
            var t1 = Task.Run(() => MessageFormatter.Format(pattern, new { fileCount = 1 }));
            var t2 = Task.Run(() => MessageFormatter.Format(pattern, new { fileCount = 1 }));
            var t3 = Task.Run(() => MessageFormatter.Format(pattern, new { fileCount = 5 }));
            await Task.WhenAll(t1, t2, t3);

            Assert.Equal("Copying one file.", t1.Result);
            Assert.Equal("Copying one file.", t2.Result);
            Assert.Equal("Copying 5 files.", t3.Result);
        }

        #endregion
    }
}