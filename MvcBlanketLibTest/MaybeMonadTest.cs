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
