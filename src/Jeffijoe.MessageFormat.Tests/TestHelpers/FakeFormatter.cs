using System.Collections.Generic;
using Jeffijoe.MessageFormat.Formatting;

namespace Jeffijoe.MessageFormat.Tests.TestHelpers
{
    /// <summary>
    ///     Fake formatter used for testing.
    /// </summary>
    internal class FakeFormatter : IFormatter
    {
        /// <summary>
        ///     What to return when <see cref="Format"/> is called.
        /// </summary>
        private readonly string formatResult;

        /// <summary>
        ///     Whether we should announce that we can format the input.
        /// </summary>
        private bool canFormat;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FakeFormatter"/> class.
        /// </summary>
        /// <param name="canFormat">Whether to return <c>true</c> for <see cref="CanFormat"/>.</param>
        /// <param name="formatResult">The result to return.</param>
        public FakeFormatter(bool canFormat = false, string formatResult = "formatted")
        {
            this.canFormat = canFormat;
            this.formatResult = formatResult;
        }
        
        /// <inheritdoc />
        public bool VariableMustExist => false;

        /// <inheritdoc />
        public bool CanFormat(FormatterRequest request) => this.canFormat;

        /// <summary>
        ///     Sets the value of what <see cref="CanFormat"/> returns.
        /// </summary>
        /// <param name="value"></param>
        public void SetCanFormat(bool value) => this.canFormat = value;

        /// <inheritdoc />
        public string Format(string locale, FormatterRequest request, IDictionary<string, object?> args, object? value,
            IMessageFormatter messageFormatter) =>
            formatResult;
    }
}