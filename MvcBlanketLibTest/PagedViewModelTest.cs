using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcBlanketLib.ViewModels;
using MvcContrib.Sorting;
using MvcContrib.UI.Grid;

namespace MvcBlanketLibTest
{
    [TestClass]
    public class PagedViewModelTest
    {

        private class MockEntity
        {
            public int IntProp { get; set; }
            public string StringProp { get; set; }
        }

        private IQueryable<MockEntity> MockRepositoryMethod ()
        {
            return new[]
                   {new MockEntity { IntProp = 10, StringProp = "BBB"}, new MockEntity { IntProp = 10, StringProp = "AAA"}, new MockEntity { IntProp = 20, StringProp = "CCC"}}
                   .AsQueryable();
        }

        
        [TestMethod]
        public void SetupWithNullParameterReturnsSameSequence()
        {
            var query = MockRepositoryMethod();
            var model = new PagedViewModel<MockEntity> {Query = MockRepositoryMethod(), GridSortOptions = new GridSortOptions{ Column = "IntProp", Direction = SortDirection.Descending}}.Setup();
            Assert.IsTrue(model.PagedList.Count() == query.Count());
            Assert.AreEqual(query.First().IntProp, model.PagedList.Last().IntProp);
        }

        [TestMethod]
        public void SetupWithOneParameterReturnsOrderedSequence()
        {
            var query = MockRepositoryMethod();
            var model = new PagedViewModel<MockEntity> { Query = MockRepositoryMethod(), GridSortOptions = new GridSortOptions{ Direction = SortDirection.Descending}}.Setup(m => m.IntProp);
            Assert.IsTrue(model.PagedList.Count() == query.Count());
            Assert.AreEqual(query.First().IntProp, model.PagedList.Last().IntProp);
        }

        [TestMethod]
        public void SetupWithTwoParameterReturnsOrderedSequence()
        {
            var query = MockRepositoryMethod();
            var model = new PagedViewModel<MockEntity> { Query = MockRepositoryMethod(), GridSortOptions = new GridSortOptions { Direction = SortDirection.Ascending } }
                .SetupByExpressions(m => m.IntProp, m => m.StringProp);
            Assert.IsTrue(model.PagedList.Count() == query.Count());
            Assert.AreEqual(query.First().IntProp, model.PagedList.Skip(1).First().IntProp);
        }

        [TestMethod]
        public void SetupWithTwoColumnNamesReturnsOrderedSequence()
        {
            var query = MockRepositoryMethod();
            var model = new PagedViewModel<MockEntity> { Query = MockRepositoryMethod(), GridSortOptions = new GridSortOptions { Direction = SortDirection.Ascending } }
                .SetupByNames("IntProp", "StringProp");
            Assert.IsTrue(model.PagedList.Count() == query.Count());
            Assert.AreEqual(query.First().IntProp, model.PagedList.Skip(1).First().IntProp);
        }

        [TestMethod]
        public void ConstructPageViewModelWithApplyMethodResult ()
        {
            var query = MockRepositoryMethod();
            var viewData = new ViewDataDictionary();
            viewData["GridSortOptions"] = new GridSortOptions();
            viewData["PageNumber"] = 1;
            viewData["PageSize"] = 10;
            var model = PagedViewModelFactory.Create<MockEntity>(viewData).Apply(query).Setup();
            Assert.AreEqual(query.Count(), model.PagedList.Count());
        }
    }
}
