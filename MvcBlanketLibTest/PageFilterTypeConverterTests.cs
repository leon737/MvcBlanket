using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcBlanketLib.TypeConverters;
using MvcBlanketLib.PageFilters;

namespace MvcBlanketLibTest
{
    [TestClass]
    public class PageFilterTypeConverterTests
    {
        [TestMethod]
        public void TestSimpleStringConvert()
        {
            string input = "Test value";
            string expected = "Test value";
            object result = PageFilterTypeConverter.Convert(input, typeof(string));
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSimpleIntConvert()
        {
            string input = "12";
            int expected = 12;
            object result = PageFilterTypeConverter.Convert(input, typeof(int));
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSimpleDateConvert()
        {
            string input = "2010-09-08";
            DateTime expected = new DateTime(2010, 09, 08);
            object result = PageFilterTypeConverter.Convert(input, typeof(DateTime));
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestEnumerableStringsConvert()
        {
            string input = "Mike,Jessy,Louis";
            IEnumerable<string> expected = new[] { "Mike", "Jessy", "Louis" };
            IEnumerable<string> result = PageFilterTypeConverter.Convert(input, typeof(IEnumerable<string>)) as IEnumerable<string>;
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Count(), result.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.AreEqual(expected.ElementAt(i), result.ElementAt(i));
            }
        }

        [TestMethod]
        public void TestEnumerableIntConvert()
        {
            string input = "10,20,30";
            IEnumerable<int> expected = new[] { 10, 20, 30 };
            IEnumerable<int> result = PageFilterTypeConverter.Convert(input, typeof(IEnumerable<int>)) as IEnumerable<int>;
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Count(), result.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.AreEqual(expected.ElementAt(i), result.ElementAt(i));
            }
        }

        [TestMethod]
        public void TestEnumerableDateConvert()
        {
            string input = "2010-09-08,2009-08-07,2008-07-06";
            IEnumerable<DateTime> expected = new[] { new DateTime(2010, 09, 08), new DateTime(2009, 08, 07), new DateTime(2008, 07, 06) };
            IEnumerable<DateTime> result = PageFilterTypeConverter.Convert(input, typeof(IEnumerable<DateTime>)) as IEnumerable<DateTime>;
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Count(), result.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.AreEqual(expected.ElementAt(i), result.ElementAt(i));
            }
        }

        [TestMethod]
        public void TestRangeStringConvert()
        {
            string input = "Mike,Louis";
            IRange<string> expected = new Range<string>("Mike", "Louis");
            IRange<string> result = PageFilterTypeConverter.Convert(input, typeof(IRange<string>)) as IRange<string>;
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.LowerBound, result.LowerBound);
            Assert.AreEqual(expected.UpperBound, result.UpperBound);            
        }

        [TestMethod]
        public void TestRangeIntConvert()
        {
            string input = "10,400";
            IRange<int> expected = new Range<int>(10, 400);
            IRange<int> result = PageFilterTypeConverter.Convert(input, typeof(IRange<int>)) as IRange<int>;
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.LowerBound, result.LowerBound);
            Assert.AreEqual(expected.UpperBound, result.UpperBound);
        }

        [TestMethod]
        public void TestRangeDateConvert()
        {
            string input = "2010-09-08,2011-10-09";
            IRange<DateTime> expected = new Range<DateTime>(new DateTime(2010, 09, 08), new DateTime(2011, 10, 09));
            IRange<DateTime> result = PageFilterTypeConverter.Convert(input, typeof(IRange<DateTime>)) as IRange<DateTime>;
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.LowerBound, result.LowerBound);
            Assert.AreEqual(expected.UpperBound, result.UpperBound);
        }
    }
}
