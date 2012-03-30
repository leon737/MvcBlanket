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
	public class IdentityMonadsTest
	{
		[TestMethod]
		public void TestToIndentityValueType() // means that Identity(m).Value = m
		{
			const int valueType = 10;
			var result = 10.ToIdentity();
			Assert.AreEqual(result.Value, valueType);
		}

		[TestMethod]
		public void TestToIdentityRefType() // means that Identity(m).Value = m
		{
			const string refType = "Hello";
			var result = refType.ToIdentity();
			Assert.AreEqual(result.Value, refType);
		}

		[TestMethod]
		public void TestIndentityLeftIdentity() // means that Identity.Compose(f) = f
		{
			const int m = 10;
			Func<int, int> g = x => x.Identity();
			Func<int, int> f = x => x/2;
			var result = g.Compose(f);
			Assert.AreEqual(f(m), result(m));
		}

		[TestMethod]
		public void TestIdentityRightIdentity() // means that f.Compose(Identity) f
		{
			const int m = 10;
			Func<int, int> g = x => x.Identity();
			Func<int, int> f = x => x / 2;
			var result = f.Compose(g);
			Assert.AreEqual(f(m), result(m));
		}

		[TestMethod]
		public void TestIdentityAssociative() // means that f.Compose(g.Compose(h)) = (f.Compose(g)).Compose(h)
		{
			const int m = 10;
			Func<int, int> g = x => x.Identity();
			Func<int, int> f = x => x / 2;
			Func<int, int> h = x => x+5;
			var leftSide = f.Compose(g.Compose(h));
			var rightSide = (f.Compose(g)).Compose(h);
			Assert.AreEqual(leftSide(m), rightSide(m));
		}

		[TestMethod]
		public void TestBindLeftIdentity() // means that Bind(Unit(e),k) = k(e)
		{
			Func<int, Identity<int>> func = x => (x * 2).ToIdentity();
			const int original = 10;
			var result = original.ToIdentity().Bind(func);
			Assert.AreEqual(result.Value, original * 2);
		}

		[TestMethod]
		public void TestBindRightIdentity() // means that Bind(m, Unit) = m
		{
			//Func<int, Identity<int>> func = 
			//const int m = 10;
			
		}
	}
}
