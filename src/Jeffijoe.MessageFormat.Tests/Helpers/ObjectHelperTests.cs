// MessageFormat for .NET
// - ObjectHelperTests.cs
// 
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2015. All rights reserved.

using System.Linq;

using Jeffijoe.MessageFormat.Helpers;
using Jeffijoe.MessageFormat.Tests.TestHelpers;

using Xunit;
using Xunit.Abstractions;

namespace Jeffijoe.MessageFormat.Tests.Helpers
{
    /// <summary>
    /// The object helper tests.
    /// </summary>
    public class ObjectHelperTests
    {
        #region Fields

        /// <summary>
        /// The output helper.
        /// </summary>
        private readonly ITestOutputHelper outputHelper;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectHelperTests"/> class.
        /// </summary>
        /// <param name="outputHelper">
        /// The output helper.
        /// </param>
        public ObjectHelperTests(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get properties_anonymous_and_dynamic.
        /// </summary>
        [Fact]
        public void GetProperties_anonymous_and_dynamic()
        {
            var obj = new { Test = "wee", Toast = "woo" };
            var actual = ObjectHelper.GetProperties(obj);
            Assert.Equal(2, actual.Count());

            dynamic d = new { Cool = "sweet" };
            actual = ObjectHelper.GetProperties(d);
            Assert.Equal(1, actual.Count());
        }

        /// <summary>
        /// The get properties_base_and_derived.
        /// </summary>
        [Fact]
        public void GetProperties_base_and_derived()
        {
            var actual = ObjectHelper.GetProperties(new Base());
            Assert.Equal(1, actual.Count());

            actual = ObjectHelper.GetProperties(new Derived());
            Assert.Equal(2, actual.Count());
        }

        /// <summary>
        /// The to dictionary.
        /// </summary>
        [Fact]
        public void ToDictionary()
        {
            var obj = new { name = "test", num = 1337 };
            var actual = obj.ToDictionary();
            Assert.Equal(2, actual.Count);
            Assert.Equal("test", actual["name"]);
            Assert.Equal(1337, actual["num"]);

            Benchmark.Start("Converting object to dictionary..", this.outputHelper);
            for (int i = 0; i < 10000; i++)
            {
                obj.ToDictionary();
            }

            Benchmark.End(this.outputHelper);
        }

        #endregion

        /// <summary>
        /// The base.
        /// </summary>
        private class Base
        {
            #region Public Properties

            /// <summary>
            /// Gets or sets the prop 1.
            /// </summary>
            public string Prop1 { get; set; }

            #endregion
        }

        /// <summary>
        /// The derived.
        /// </summary>
        private class Derived : Base
        {
            #region Public Properties

            /// <summary>
            /// Gets or sets the prop 2.
            /// </summary>
            public int Prop2 { get; set; }

            #endregion
        }
    }
}