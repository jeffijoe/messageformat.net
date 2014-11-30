using System.Collections.Generic;
using System.Linq;

namespace Jeffijoe.MessageFormat.Formatting
{
    /// <summary>
    /// Manages formatters to use.
    /// </summary>
    public class FormatterLibrary : IFormatterLibrary
    {
        /// <summary>
        /// Gets the formatters list.
        /// </summary>
        /// <value>
        /// The formatters.
        /// </value>
        public IList<IFormatter> Formatters { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatterLibrary"/> class.
        /// </summary>
        public FormatterLibrary()
        {
            Formatters = new List<IFormatter>();
        }

        /// <summary>
        /// Adds the specified formatter, making it available to use for whoever wants to.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        public void Add(IFormatter formatter)
        {
            Formatters.Add(formatter);
        }

        /// <summary>
        /// Gets the formatter to use. If none was found, throws an exception.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <exception cref="FormatterNotFoundException"></exception>
        public IFormatter GetFormatter(FormatterRequest request)
        {
            var formatter = Formatters.FirstOrDefault(x => x.CanFormat(request));
            if(formatter == null) throw new FormatterNotFoundException(request);
            return formatter;
        }
    }
}