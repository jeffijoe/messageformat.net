// MessageFormat for .NET
// - ObjectHelperTests.cs
// Author: Jeff Hansen <jeff@jeffijoe.com>
// Copyright (C) Jeff Hansen 2014. All rights reserved.

using System.Linq;

using Jeffijoe.MessageFormat.Helpers;
using Jeffijoe.MessageFormat.Tests.TestHelpers;

using Xunit;

namespace Jeffijoe.MessageFormat.Tests.Helpers
{
    public class ObjectHelperTests
    {
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

        [Fact]
        public void GetProperties_base_and_derived()
        {
            var actual = ObjectHelper.GetProperties(new Base());
            Assert.Equal(1, actual.Count());

            actual = ObjectHelper.GetProperties(new Derived());
            Assert.Equal(2, actual.Count());
        }

        [Fact]
        public void ToDictionary()
        {
            var obj = new { name = "test", num = 1337 };
            var actual = obj.ToDictionary();
            Assert.Equal(2, actual.Count);
            Assert.Equal("test", actual["name"]);
            Assert.Equal(1337, actual["num"]);

            Benchmark.Start("Converting object to dictionary..");
            for (int i = 0; i < 10000; i++)
            {
                obj.ToDictionary();
            }

            Benchmark.End();
        }

        #region Test classes

        private class Base
        {
            public string Prop1 { get; set; }
        }

        private class Derived : Base
        {
            public int Prop2 { get; set; }
        }

        #endregion
    }
}