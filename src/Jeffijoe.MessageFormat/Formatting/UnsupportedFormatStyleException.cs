namespace Jeffijoe.MessageFormat.Formatting;

/// <summary>
///     Thrown when formatter is unable to apply the given style.
/// </summary>
public class UnsupportedFormatStyleException : MessageFormatterException
{
    #region Constructors and Destructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnsupportedFormatStyleException"/> class.
    /// </summary>
    /// <param name="variable">
    ///     The variable.
    /// </param>
    /// <param name="format">
    ///     The format.
    /// </param>
    /// <param name="style">
    ///     The style that was not supported.
    /// </param>
    public UnsupportedFormatStyleException(
        string variable,
        string format,
        string style)
        : base(BuildMessage(variable, format, style))
    {
        this.Variable = variable;
        this.Format = format;
        this.Style = style;
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///     Gets the name of the missing variable.
    /// </summary>
    /// <value>
    ///     The missing variable.
    /// </value>
    public string Variable { get; private set; }

    /// <summary>
    ///     Gets the format that attempted to apply the style.
    /// </summary>
    /// <value>
    ///     The format.
    /// </value>
    public string Format { get; private set; }

    /// <summary>
    ///     Gets the style that could not be applied.
    /// </summary>
    /// <value>
    ///     The style.
    /// </value>
    public string Style { get; private set; }

    #endregion

    #region Methods

    /// <summary>
    ///     Builds the message.
    /// </summary>
    /// <param name="variable">
    ///     The variable.
    /// </param>
    /// <param name="format">
    ///     The format.
    /// </param>
    /// <param name="style">
    ///     The style that was not supported.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    private static string BuildMessage(
        string variable,
        string format,
        string style) =>
        $"The variable '{variable}' could not be formatted as a '{format}' because the style '{style}' is not supported.";

    #endregion
}