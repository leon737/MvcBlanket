﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcBlanketLib.Fluent
{
	public class Maybe<T>
	{
		public readonly static Maybe<T> Nothing = new Maybe<T>();
		public T Value { get; private set; }
		public bool HasValue { get; private set; }
		Maybe()
		{
			HasValue = false;
		}
		public Maybe(T value)
		{
			Value = value;
			HasValue = true;
		}
		
	}
}
