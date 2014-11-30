using System;
using System.Collections.Generic;
using System.Linq;

namespace Jeffijoe.MessageFormat.Formatting
{
    /// <summary>
    /// Container class for formatter argument parsing result.
    /// </summary>
    public class ParsedArguments
    {
        /// <summary>
        /// Gets the keyed blocks.
        /// </summary>
        /// <value>
        /// The keyed blocks.
        /// </value>
        public IEnumerable<KeyedBlock> KeyedBlocks { get; private set; }

        /// <summary>
        /// Gets the extensions.
        /// </summary>
        /// <value>
        /// The extensions.
        /// </value>
        public IEnumerable<FormatterExtension> Extensions { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsedArguments"/> class.
        /// </summary>
        public ParsedArguments(IEnumerable<KeyedBlock> keyedBlocks, IEnumerable<FormatterExtension> extensions)
        {
            if (keyedBlocks == null) throw new ArgumentNullException("keyedBlocks");
            if (extensions == null) throw new ArgumentNullException("extensions");
            KeyedBlocks = keyedBlocks.ToList();
            Extensions = extensions.ToList();
        }
    }
}