/*
MVC Blanket Library Copyright (C) 2009-2012 Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MvcBlanketLib.ViewModels;
using MvcContrib.Sorting;
using MvcContrib.UI.Grid;

namespace MvcBlanketLibTest.ViewModelTests
{
    [TestClass]
    public class PagedViewModelFactoryTests
    {

        private readonly ViewDataDictionary viewData = new ViewDataDictionary();
        private readonly Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
        private readonly Mock<Controller> controller = new Mock<Controller>();

        public PagedViewModelFactoryTests()
        {
            controllerContext.Setup(m => m.Controller).Returns(controller.Object);
        }


        [TestMethod]
        public void TestCreateModelFromViewData()
        {
            viewData["PageNumber"] = 2;
            viewData["PageSize"] = 2;
            var gso = new GridSortOptions {Column = "Published", Direction = SortDirection.Ascending};
            viewData["GridSortOptions"] = gso;


            var model =
                PagedViewModelFactory.Create<FakeEntity>(viewData).Apply(
                    f => new FakeRepository().Get(f as FakeFiltersModel)).Setup();

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.PagedList);
            Assert.AreEqual(2, model.Page);
            Assert.AreEqual(2, model.PageSize);
            Assert.AreEqual(6, model.PagedList.TotalItems);
            Assert.AreEqual(3, model.PagedList.TotalPages);
            Assert.AreEqual(2, model.PagedList.ElementAt(0).Id);
            Assert.AreEqual(3, model.PagedList.ElementAt(1).Id);
        }

        [TestMethod]
        public void TestCreateModelFromContext()
        {
            controller.Object.ViewBag.PageNumber = 2;
            controller.Object.ViewBag.PageSize = 2;
            var gso = new GridSortOptions { Column = "Published", Direction = SortDirection.Ascending };
            controller.Object.ViewBag.GridSortOptions = gso;


            var model =
                PagedViewModelFactory.Create<FakeEntity>(controllerContext.Object).Apply(
                    f => new FakeRepository().Get(f as FakeFiltersModel)).Setup();

            Assert.IsNotNull(model);
            Assert.IsNotNull(model.PagedList);
            Assert.AreEqual(2, model.Page);
            Assert.AreEqual(2, model.PageSize);
            Assert.AreEqual(6, model.PagedList.TotalItems);
            Assert.AreEqual(3, model.PagedList.TotalPages);
            Assert.AreEqual(2, model.PagedList.ElementAt(0).Id);
            Assert.AreEqual(3, model.PagedList.ElementAt(1).Id);
        }
    }
}
