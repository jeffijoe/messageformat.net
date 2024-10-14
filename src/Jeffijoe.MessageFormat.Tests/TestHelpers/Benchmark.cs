// MessageFormat for .NET
// - Benchmark.cs
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using System.Diagnostics;

using Xunit.Abstractions;

namespace Jeffijoe.MessageFormat.Tests.TestHelpers;

/// <summary>
///     Benchmark helper.
/// </summary>
public static class Benchmark
{
    #region Static Fields

    /// <summary>
    ///     The stopwatch.
    /// </summary>
    private static readonly Stopwatch Sw = new Stopwatch();

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// Ends the benchmark and prints the elapsed time to the console.
    /// </summary>
    /// <param name="outputHelper">
    /// The output helper.
    /// </param>
    public static void End(ITestOutputHelper outputHelper)
    {
        Sw.Stop();
        outputHelper.WriteLine("Result: {0}ms ({1} ticks)", Sw.ElapsedMilliseconds, Sw.ElapsedTicks);
    }

    /// <summary>
    /// Starts the benchmark, and writes the passed message to the output helper.
    /// </summary>
    /// <param name="messageForConsole">
    /// The message for console.
    /// </param>
    /// <param name="outputHelper">
    /// The output helper.
    /// </param>
    public static void Start(string messageForConsole, ITestOutputHelper outputHelper)
    {
        outputHelper.WriteLine(messageForConsole);
        Sw.Restart();
    }

    #endregion
}