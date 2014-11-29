// Benchmark.cs
// - MessageFormat
// -- Jeffijoe.MessageFormat.Tests
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright © 2014.

using System;
using System.Diagnostics;

namespace Jeffijoe.MessageFormat.Tests.TestHelpers
{
    /// <summary>
    /// Benchmark helper.
    /// </summary>
    public static class Benchmark
    {
        private static Stopwatch _sw = new Stopwatch();

        /// <summary>
        /// Starts the benchmark, and writes the passed message to the console.
        /// </summary>
        /// <param name="messageForConsole">The message for console.</param>
        public static void Start(string messageForConsole)
        {
            Console.WriteLine(messageForConsole);
            _sw.Restart();
        }

        /// <summary>
        /// Ends the benchmark and prints the elapsed time to the console.
        /// </summary>
        public static void End()
        {
            _sw.Stop();
            Console.WriteLine("Result: {0}ms ({1} ticks)", _sw.ElapsedMilliseconds, _sw.ElapsedTicks);
        }
    }
}