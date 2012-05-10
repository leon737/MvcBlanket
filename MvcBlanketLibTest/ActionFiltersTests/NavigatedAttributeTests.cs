/*
MVC Blanket Library Copyright (C) 2009-2012 Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web.Mvc;
using MvcBlanketLib.ActionFilters;
using MvcContrib.Sorting;
using MvcContrib.UI.Grid;

namespace MvcBlanketLibTest.ActionFiltersTests
{
    [TestClass]
    public class NavigatedAttributeTests
    {
        private readonly Mock<ActionExecutingContext> filterContext = new Mock<ActionExecutingContext>();
        private readonly Mock<HttpContextBase> httpContext = new Mock<HttpContextBase>();
        private readonly Mock<HttpRequestBase>  httpRequest = new Mock<HttpRequestBase>();
        private readonly Mock<ControllerBase> controller = new Mock<ControllerBase>();

        public NavigatedAttributeTests()
        {
            filterContext.Setup(m => m.HttpContext).Returns(httpContext.Object);
            httpContext.Setup(m => m.Request).Returns(httpRequest.Object);
            filterContext.Setup(m => m.Controller).Returns(controller.Object);
        }
 
        [TestMethod]
        public void TestParsingPageAndSize()
        {
            filterContext.Setup(m => m.HttpContext).Returns(httpContext.Object);
            httpRequest.SetupGet(m => m["page"]).Returns("2");
            httpRequest.SetupGet(m => m["size"]).Returns("4");

            var navigatedAttribute = new NavigatedAttribute();
            navigatedAttribute.OnActionExecuting(filterContext.Object);

            Assert.AreEqual(2, controller.Object.ViewBag.PageNumber);
            Assert.AreEqual(4, controller.Object.ViewBag.PageSize);

            var gso = controller.Object.ViewBag.GridSortOptions as GridSortOptions;
            Assert.IsNotNull(gso);
            Assert.AreEqual(SortDirection.Descending, gso.Direction);
            Assert.IsNull(gso.Column);
        }

        [TestMethod]
        public void TestEmpty()
        {
            filterContext.Setup(m => m.HttpContext).Returns(httpContext.Object);

            var navigatedAttribute = new NavigatedAttribute();
            navigatedAttribute.OnActionExecuting(filterContext.Object);

            Assert.AreEqual(1, controller.Object.ViewBag.PageNumber);
            Assert.AreEqual(navigatedAttribute.PageSize, controller.Object.ViewBag.PageSize);

            var gso = controller.Object.ViewBag.GridSortOptions as GridSortOptions;
            Assert.IsNotNull(gso);
            Assert.AreEqual(SortDirection.Descending, gso.Direction);
            Assert.IsNull(gso.Column);
        }

        [TestMethod]
        public void TestSortColumnAndDirection()
        {
            filterContext.Setup(m => m.HttpContext).Returns(httpContext.Object);
            httpRequest.SetupGet(m => m["column"]).Returns("mycolumn");
            httpRequest.SetupGet(m => m["direction"]).Returns("ascending");

            var navigatedAttribute = new NavigatedAttribute();
            navigatedAttribute.OnActionExecuting(filterContext.Object);

            Assert.AreEqual(1, controller.Object.ViewBag.PageNumber);
            Assert.AreEqual(navigatedAttribute.PageSize, controller.Object.ViewBag.PageSize);

            var gso = controller.Object.ViewBag.GridSortOptions as GridSortOptions;
            Assert.IsNotNull(gso);
            Assert.AreEqual(SortDirection.Ascending, gso.Direction);
            Assert.AreEqual("mycolumn", gso.Column);
        }
    }
}
