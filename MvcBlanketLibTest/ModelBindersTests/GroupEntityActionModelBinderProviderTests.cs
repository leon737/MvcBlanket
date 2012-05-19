/*
MVC Blanket Library Copyright (C) 2009-2012 Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcBlanketLib.ModelBinders;
using System;

namespace MvcBlanketLibTest.ModelBindersTests
{
    [TestClass]
    public class GroupEntityActionModelBinderProviderTests
    {
        [TestMethod]
        public void TestIncorrectType()
        {
            var provider = new GroupEntityActionModelBinderProvider();
            var result = provider.GetBinder(typeof (int));
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestValidSimpleType()
        {
            var provider = new GroupEntityActionModelBinderProvider();
            var result = provider.GetBinder(typeof(GroupAction<string>));
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(GroupEntityActionModelBinder<string, string>));
        }

        [TestMethod]
        public void TestValidGenericType()
        {
            var provider = new GroupEntityActionModelBinderProvider();
            var result = provider.GetBinder(typeof(GroupAction<int, string>));
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(GroupEntityActionModelBinder<int, string>));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidGenericTypeWithThreeGenericParameters() // has more than 2 generic parameters
        {
            var provider = new GroupEntityActionModelBinderProvider();
            provider.GetBinder(typeof(InvalidGroupActionClassWithThreeGenericParameters<int, int, string>));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidGenericTypeWithoutGenericParameters() // has less than 1 generic parameter
        {
            var provider = new GroupEntityActionModelBinderProvider();
            provider.GetBinder(typeof(InvalidGroupActionClassWithoutThreeGenericParameters));
        }

        public class InvalidGroupActionClassWithThreeGenericParameters<T1, T2, T3> : IGroupAction
        {
            
        }

        public class InvalidGroupActionClassWithoutThreeGenericParameters : IGroupAction
        {

        }
    }
}
