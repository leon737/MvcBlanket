/*
MVC Blanket Library Copyright (C) 2009-2012 Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MvcBlanketLib.PageFilters;
using System.Collections.Specialized;
using MvcBlanketLib.ActionFilters;

namespace MvcBlanketLibTest.ActionFiltersTests
{

    [TestClass]
    public class SelectFilterAttributeTests
    {


        private readonly Mock<ActionExecutingContext> filterContext = new Mock<ActionExecutingContext>();
        private readonly Mock<HttpContextBase> httpContext = new Mock<HttpContextBase>();
        private readonly Mock<HttpRequestBase> httpRequest = new Mock<HttpRequestBase>();
        private readonly Mock<ControllerBase> controller = new Mock<ControllerBase>();
        private readonly NameValueCollection queryString = new NameValueCollection();

        public SelectFilterAttributeTests()
        {
            filterContext.Setup(m => m.HttpContext).Returns(httpContext.Object);
            httpContext.Setup(m => m.Request).Returns(httpRequest.Object);
            filterContext.Setup(m => m.Controller).Returns(controller.Object);
            httpRequest.Setup(m => m.QueryString).Returns(queryString);
        }

        [TestMethod]
        public void TestParseFilterModel()
        {
            queryString.Add("s_stringfilter", "StringValue");
            queryString.Add("s_intfilter", "10");
            queryString.Add("s_datefilter", "2010-09-08");
            queryString.Add("s_boolfilter", "true");
            queryString.Add("s_stringfilter2", "StringValue1,StringValue2,StringValue3");
            queryString.Add("s_intfilter2", "20,30,40");
            queryString.Add("s_rangeintfilter", "10,50");
            queryString.Add("s_rangedatefilter", "2009-08-07,2010-09-08");

            var selectFilterAttribute = new SelectFilterAttribute { FiltersModel = typeof(TestFilterModel) };
            selectFilterAttribute.OnActionExecuting(filterContext.Object);
            
            var model = controller.Object.ViewBag.FiltersModel as TestFilterModel;
            Assert.IsNotNull(model);

            Assert.IsTrue(model.StringFilter.Selected);
            Assert.AreEqual("StringValue", model.StringFilter.Value);
            
            Assert.IsTrue(model.IntFilter.Selected);
            Assert.AreEqual(10, model.IntFilter.Value);

            Assert.IsTrue(model.DateFilter.Selected);
            Assert.AreEqual(new DateTime(2010, 09, 08), model.DateFilter.Value);
            
            Assert.IsTrue(model.BoolFilter.Selected);
            Assert.AreEqual(true, model.BoolFilter.Value);
            
            Assert.IsTrue(model.EnumerableStringFilter.Selected);
            Assert.AreEqual(3, model.EnumerableStringFilter.Value.Count());
            Assert.AreEqual("StringValue1", model.EnumerableStringFilter.Value.ElementAt(0));
            Assert.AreEqual("StringValue2", model.EnumerableStringFilter.Value.ElementAt(1));
            Assert.AreEqual("StringValue3", model.EnumerableStringFilter.Value.ElementAt(2));

            Assert.IsTrue(model.EnumerableIntFilter.Selected);
            Assert.AreEqual(3, model.EnumerableIntFilter.Value.Count());
            Assert.AreEqual(20, model.EnumerableIntFilter.Value.ElementAt(0));
            Assert.AreEqual(30, model.EnumerableIntFilter.Value.ElementAt(1));
            Assert.AreEqual(40, model.EnumerableIntFilter.Value.ElementAt(2));

            Assert.IsTrue(model.RangeIntFilter.Selected);
            Assert.AreEqual(10, model.RangeIntFilter.Value.LowerBound);
            Assert.AreEqual(50, model.RangeIntFilter.Value.UpperBound);

            Assert.IsTrue(model.RangeDateFilter.Selected);
            Assert.AreEqual(new DateTime(2009, 08, 07), model.RangeDateFilter.Value.LowerBound);
            Assert.AreEqual(new DateTime(2010, 09, 08), model.RangeDateFilter.Value.UpperBound);
        }

        [TestMethod]
        public void TestParseNullableFilterModel()
        {
            var selectFilterAttribute = new SelectFilterAttribute { FiltersModel = typeof(TestFilterModel) };
            selectFilterAttribute.OnActionExecuting(filterContext.Object);

            var model = controller.Object.ViewBag.FiltersModel as TestFilterModel;
            Assert.IsNotNull(model);

            Assert.IsFalse(model.StringFilter.Selected);
            Assert.IsFalse(model.IntFilter.Selected);
            Assert.IsFalse(model.DateFilter.Selected);
            Assert.IsFalse(model.BoolFilter.Selected);
            Assert.IsFalse(model.EnumerableStringFilter.Selected);
            Assert.IsFalse(model.EnumerableIntFilter.Selected);
            Assert.IsFalse(model.RangeIntFilter.Selected);
            Assert.IsFalse(model.RangeDateFilter.Selected);
        }

        [TestMethod]
        public void TestParseInvalidFormattedFilterModel()
        {

            queryString.Add("s_intfilter", "abcde");
            queryString.Add("s_datefilter", "abcde");
            queryString.Add("s_boolfilter", "abcde");

            var selectFilterAttribute = new SelectFilterAttribute { FiltersModel = typeof(TestFilterModel) };
            selectFilterAttribute.OnActionExecuting(filterContext.Object);

            var model = controller.Object.ViewBag.FiltersModel as TestFilterModel;
            Assert.IsNotNull(model);

            Assert.IsFalse(model.IntFilter.Selected);
            Assert.IsInstanceOfType(model.IntFilter.FormatException, typeof(FormatException));

            Assert.IsFalse(model.DateFilter.Selected);
            Assert.IsInstanceOfType(model.DateFilter.FormatException, typeof(FormatException));

            Assert.IsFalse(model.BoolFilter.Selected);
            Assert.IsInstanceOfType(model.BoolFilter.FormatException, typeof(FormatException));
        }


        [TestMethod]
        public void TestParseNotSelected()
        {
            queryString.Add("s_stringfilter", "abcd");
            queryString.Add("s_intfilter", "202");
         
            var selectFilterAttribute = new SelectFilterAttribute { FiltersModel = typeof(TestFilterModel) };
            selectFilterAttribute.OnActionExecuting(filterContext.Object);

            var model = controller.Object.ViewBag.FiltersModel as TestFilterModel;
            Assert.IsNotNull(model);

            Assert.AreEqual("I'm not selected", model.StringFilter.NotSelectedValue);

            Assert.AreEqual("101", model.IntFilter.NotSelectedValue);
        }


        class TestFilterModel
        {
            [NotSelectedValue(NotSelectedValue= "I'm not selected")]
            public PageFilter<string> StringFilter { get; set; }

            [NotSelectedValue(NotSelectedValue = "101")]
            public PageFilter<int> IntFilter { get; set; }

            public PageFilter<DateTime> DateFilter { get; set; }

            public PageFilter<bool> BoolFilter { get; set; }

            [Alias(Name = "stringfilter2")]
            public PageFilter<IEnumerable<string>> EnumerableStringFilter { get; set; }

            [Alias(Name = "intfilter2")]
            public PageFilter<IEnumerable<int>> EnumerableIntFilter { get; set; }

            public PageFilter<IRange<int>> RangeIntFilter { get; set; }

            public PageFilter<IRange<DateTime>> RangeDateFilter { get; set; }
        }
        
    }



}
