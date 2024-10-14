using System;

namespace Jeffijoe.MessageFormat.MetadataGenerator.Plural.Parsing;

public class InvalidCharacterException : FormatException
{
    public InvalidCharacterException(char character) : base($"Invalid format, do not recognise character '{character}'")
    {
    }
}