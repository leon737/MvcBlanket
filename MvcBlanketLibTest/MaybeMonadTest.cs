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
using MvcBlanketLib.Fluent;

namespace MvcBlanketLibTest
{
    [TestClass]
    public class MaybeMonadTest
    {
        [TestMethod]
        public void ReferenceWithValueToMaybe()
        {
            string reference = "Some string";
            Maybe<string> maybe = reference.ToMaybe();
            Assert.IsTrue(maybe.HasValue);
            Assert.AreEqual(reference, maybe.Value);
        }

        [TestMethod]
        public void ReferenceWithoutValueToMaybe()
        {
            Maybe<string> maybe = new Maybe<string>(null);
            Assert.IsFalse(maybe.HasValue);
        }

        [TestMethod]
        public void ValueToMaybe()
        {
            int value = 10;
            Maybe<int> maybe = new Maybe<int>(value);
            Assert.IsTrue(maybe.HasValue);
            Assert.AreEqual(value, maybe.Value);
        }
    }
}
