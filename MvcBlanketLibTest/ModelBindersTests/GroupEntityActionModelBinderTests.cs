/*
MVC Blanket Library Copyright (C) 2009-2012 Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web;
using MvcBlanketLib.ModelBinders;

namespace MvcBlanketLibTest.ModelBindersTests
{
    [TestClass]
    public class GroupEntityActionModelBinderTests
    {
        private readonly Mock<ControllerContext> controllerContext = new Mock<ControllerContext>();
        private readonly Mock<ModelBindingContext> modelBindingContext = new Mock<ModelBindingContext>();
        private readonly Mock<HttpContextBase> httpContext = new Mock<HttpContextBase>();
        private readonly Mock<HttpRequestBase> httpRequest = new Mock<HttpRequestBase>();
        private readonly NameValueCollection form = new NameValueCollection();

        public GroupEntityActionModelBinderTests()
        {
            controllerContext.SetupGet(m => m.HttpContext).Returns(httpContext.Object);
            httpContext.SetupGet(m => m.Request).Returns(httpRequest.Object);
            httpRequest.SetupGet(m => m.Form).Returns(form);
        }
        
        [TestMethod]
        public void TestBindSimpleGroupActionModel()
        {
            form.Add("action", "action1");
            form.Add("chk", "10");
            form.Add("chk", "20");
            form.Add("chk", "30");

            var binder = new GroupEntityActionModelBinder<string, int>();
            var model =
                binder.BindModel(controllerContext.Object, modelBindingContext.Object) as GroupAction<string, int>;
            
            Assert.IsNotNull(model);

            Assert.AreEqual("action1", model.Action);

            Assert.AreEqual(3, model.Identifiers.Count());
            Assert.AreEqual(10, model.Identifiers.ElementAt(0));
            Assert.AreEqual(20, model.Identifiers.ElementAt(1));
            Assert.AreEqual(30, model.Identifiers.ElementAt(2));
        }

        [TestMethod]
        public void TestBindGenericGroupActionModel()
        {
            form.Add("action", "act2");
            form.Add("chk", "10");
            form.Add("chk", "20");
            form.Add("chk", "30");

            var binder = new GroupEntityActionModelBinder<MyActions, int>();
            var model =
                binder.BindModel(controllerContext.Object, modelBindingContext.Object) as GroupAction<MyActions, int>;

            Assert.IsNotNull(model);

            Assert.AreEqual(MyActions.Action2, model.Action);

            Assert.AreEqual(3, model.Identifiers.Count());
            Assert.AreEqual(10, model.Identifiers.ElementAt(0));
            Assert.AreEqual(20, model.Identifiers.ElementAt(1));
            Assert.AreEqual(30, model.Identifiers.ElementAt(2));
        }

        enum MyActions
        {
            [FlagString(StringRepresentation="act1")]
            Action1,
            [FlagString(StringRepresentation = "act2")]
            Action2,
            [FlagString(StringRepresentation = "act3")]
            Action3
        }
    }
}
