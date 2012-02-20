using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcBlanketLib.Fluent
{
	public class Identity<T>
	{
		public T Value { get; private set; }
		public Identity(T value) { this.Value = value; }
	}
}
