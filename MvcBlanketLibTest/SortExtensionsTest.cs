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
