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

namespace MvcBlanketLibTest.ModelBindersTests
{
    [TestClass]
    public class FlagStringAttributeTests
    {
        [TestMethod]
        public void TestConvertFromStringToMyClass()
        {
            string stringFromMappedPersistentEntity = "Second string";
            MyClass myObject = (MyClass)stringFromMappedPersistentEntity; // myObject will contain Flag value equals to MyEnum.First
            Assert.AreEqual(MyEnum.Second, myObject);
        }

        [TestMethod]
        public void TestConvertFromMyClassToString()
        {
            MyClass anotherObject = new MyClass(MyEnum.Second);
            string persistentString = (string)anotherObject; // persistentString will contain "Second string"
            Assert.AreEqual("Second string", persistentString);
        }


        public enum MyEnum
        {
            [FlagString(StringRepresentation = "First string")]
            First,
            [FlagString(StringRepresentation = "Second string")]
            Second
        }



        public class MyClass : FlagBase<MyClass, MyEnum>
        {
            public MyClass() { }
            
            public MyClass(MyEnum enumValue)
            {
                Flag = enumValue;
            }
        }

    }
}
