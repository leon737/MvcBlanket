using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcBlanketLib.Fluent
{
	public class ChooseOperand<TComp, TOutput>
	{
		public TComp IfValue { get; private set; }
		public TOutput ThenValue { get; private set; }

		public ChooseOperand<TComp, TOutput> If(TComp ifValue)
		{
			IfValue = ifValue;
			return this;
		}

		public ChooseOperand<TComp, TOutput> Then(TOutput thenValue)
		{
			ThenValue = thenValue;
			return this;
		}
	}
}
