using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcBlanketLib.Extensions;
using MvcContrib.Sorting;

namespace MvcBlanketLibTest
{
    [TestClass]
    public class SortExtensionsTest
    {
        private class MockEntity
        {
            public int IntProp { get; set; }
            public string StringProp { get; set; }
        }

        private IQueryable<SortExtensionsTest.MockEntity> MockRepositoryMethod()
        {
            return new[] { new SortExtensionsTest.MockEntity { IntProp = 10, StringProp = "BBB" }, new SortExtensionsTest.MockEntity { IntProp = 10, StringProp = "AAA" }, new SortExtensionsTest.MockEntity { IntProp = 20, StringProp = "CCC" } }
                   .AsQueryable();
        }

        [TestMethod]
        public void SortByTwoFieldsByNames()
        {
            var query = MockRepositoryMethod(); //.Where(x => x.IntProp > 0);
            var ordered = query.OrderBy("IntProp", SortDirection.Ascending, true).OrderBy("StringProp", SortDirection.Ascending, false);
            Assert.IsTrue(ordered.First().StringProp == "AAA");
        }
    }
}
