/*
MVC Blanket Library Copyright (C) 2009-2012 Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web.Mvc;
using MvcBlanketLib.ActionFilters;
using MvcBlanketLib.ViewModels;

namespace MvcBlanketLibTest.ActionFiltersTests
{
    [TestClass]
    public class SortMappingAttributeTests
    {
        private readonly Mock<ActionExecutingContext> filterContext = new Mock<ActionExecutingContext>();
        private readonly Mock<HttpContextBase> httpContext = new Mock<HttpContextBase>();
        private readonly IDictionary items = new Hashtable();

        public SortMappingAttributeTests()
        {
            
            filterContext.Setup(m => m.HttpContext).Returns(httpContext.Object);
            httpContext.SetupGet(m => m.Items).Returns(items);
        }
 
        [TestMethod]
        public void TestTwoNames()
        {
            filterContext.Setup(m => m.HttpContext).Returns(httpContext.Object);

            var sortMappingAttribute = new SortMappingAttribute()
                                           {Mapping = "Prop1=Column1,Column2;Prop2=Column2,Column3"};
            sortMappingAttribute.OnActionExecuting(filterContext.Object);

            var mappings = httpContext.Object.Items[SortMapping.ContentItemName] as IList<SortMapping>;

            Assert.IsNotNull(mappings);
            Assert.AreEqual(2, mappings.Count());

            var mapping1 = mappings.ElementAt(0);
            Assert.IsNotNull(mapping1);
            Assert.AreEqual("Prop1", mapping1.CommonName);
            Assert.AreEqual("Column1", mapping1.ColumnNames.ElementAt(0));
            Assert.AreEqual("Column2", mapping1.ColumnNames.ElementAt(1));

            var mapping2 = mappings.ElementAt(1);
            Assert.IsNotNull(mapping2);
            Assert.AreEqual("Prop2", mapping2.CommonName);
            Assert.AreEqual("Column2", mapping2.ColumnNames.ElementAt(0));
            Assert.AreEqual("Column3", mapping2.ColumnNames.ElementAt(1));

        }

    }
}
