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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcBlanketLib.Fluent
{
	public static class MonadExtensions
	{

		/* base monads */
		public static Func<T, V> Compose<T, U, V>(this Func<U, V> f, Func<T, U> g)
		{
			return x => f(g(x));
		}

		
		/* maybe monads */

		public static Maybe<T> ToMaybe<T>(this T value)
		{
			return new Maybe<T>(value);
		}
		
		public static Maybe<TResult> With<TInput, TResult>(this Maybe<TInput> o, Func<TInput, TResult> evaluator)
		{
			if (o == null || !o.HasValue) return Maybe<TResult>.Nothing;
			return new Maybe<TResult>(evaluator(o.Value));
		}

		public static Maybe<TResult> Return<TInput, TResult>(this Maybe<TInput> o,
			Func<TInput, TResult> evaluator, TResult failureValue)
		{
			if (o == null || !o.HasValue) return new Maybe<TResult>(failureValue);
			return new Maybe<TResult>(evaluator(o.Value));
		}

		public static Maybe<TInput> If<TInput>(this Maybe<TInput> o, Func<TInput, bool> evaluator)
		{
			if (o == null || !o.HasValue) return null;
			return evaluator(o.Value) ? o : Maybe<TInput>.Nothing;
		}

		public static Maybe<TInput> Unless<TInput>(this Maybe<TInput> o, Func<TInput, bool> evaluator)
		{
			if (o == null || !o.HasValue) return null;
			return evaluator(o.Value) ? Maybe<TInput>.Nothing : o;
		}

		public static Maybe<TInput> Do<TInput>(this Maybe<TInput> o, Action<TInput> action)
		{
			if (o == null || !o.HasValue) return null;
			action(o.Value);
			return o;
		}

		public static Maybe<TInput> Do<TInput>(this Maybe<TInput> o, params Action<TInput>[] actions)
		{
			if (o == null || !o.HasValue) return null;
			foreach (var action in actions)
				action(o.Value);
			return o;
		}

		public static Maybe<TInput> ApplyIf<TInput>(this Maybe<TInput> o, Func<TInput, bool> evaluator, Func<TInput, TInput> action)
		{
			if (o == null || !o.HasValue) return null;
			return evaluator(o.Value) ? new Maybe<TInput>(action(o.Value)) : o;
		}

		public static Maybe<TInput> ApplyUnless<TInput>(this Maybe<TInput> o, Func<TInput, bool> evaluator, Func<TInput, TInput> action)
		{
			if (o == null || !o.HasValue) return null;
			return evaluator(o.Value) ? o : new Maybe<TInput>(action(o.Value));
		}

		public static Maybe<TInput> IsNull<TInput>(this Maybe<TInput> o, Func<TInput> func)
		{
			if (o == null || !o.HasValue) return new Maybe<TInput>(func());
			return o;
		}

		public static Maybe<U> SelectMany<T, U>(this Maybe<T> m, Func<T, Maybe<U>> k)
		{
			if (!m.HasValue)
				return Maybe<U>.Nothing;
			return k(m.Value);
		}


		/* Identity monads */

		public static Identity<T> ToIdentity<T>(this T value)
		{
			return new Identity<T>(value);
		}

		public static T Identity<T>(this T value)
		{
			return value;
		}

		public static Identity<U> SelectMany<T, U>(this Identity<T> id, Func<T, Identity<U>> k)
		{
			return k(id.Value);
		}

		public static Identity<V> SelectMany<T, U, V>(this Identity<T> id, Func<T, Identity<U>> k, Func<T, U, V> s)
		{
			return s(id.Value, k(id.Value).Value).ToIdentity();
		}

		internal static Identity<U> Bind<T, U>(this Identity<T> id, Func<T, Identity<U>> k)
		{
			return k(id.Value);
		}


		/* Continuation monads */

		public delegate Answer K<T, Answer>(Func<T, Answer> k);

		public static K<T, Answer> ToContinuation<T, Answer>(this T value)
		{
			return (Func<T, Answer> c) => c(value);
		}

		public static K<U, Answer> SelectMany<T, U, Answer>(this K<T, Answer> m, Func<T, K<U, Answer>> k)
		{
			return (Func<U, Answer> c) => m((T x) => k(x)(c));
		}

		public static K<V, Answer> SelectMany<T, U, V, Answer>(this K<T, Answer> m, Func<T, K<U, Answer>> k, Func<T, U, V> s)
		{
			return m.SelectMany(x => k(x).SelectMany(y => s(x, y).ToContinuation<V, Answer>()));
		}

		/* Choose monads */

		public static TOutput Choose<TInput, TComp, TOutput>
			(this TInput value, Func<TInput, Func<TComp>, bool> evaluator,
			params Func<ChooseOperand<TComp, TOutput>, ChooseOperand<TComp, TOutput>>[] opFuncs)
		{
			foreach (var opFunc in opFuncs)
			{
				var operand = opFunc(new ChooseOperand<TComp, TOutput>());
				if (evaluator(value, () => operand.IfValue))
					return operand.ThenValue;
			}
			return default(TOutput);
		}

		public static TOutput Choose<TInput, TOutput>
			(this TInput value, Func<TInput, Func<TInput>, bool> evaluator,
			params Func<ChooseOperand<TInput, TOutput>, ChooseOperand<TInput, TOutput>>[] opFuncs)
		{
			foreach (var opFunc in opFuncs)
			{
				var operand = opFunc(new ChooseOperand<TInput, TOutput>());
				if (evaluator(value, () => operand.IfValue))
					return operand.ThenValue;
			}
			return default(TOutput);
		}

		public static TOutput Choose<TInput, TComp, TOutput>
			(this TInput value, Func<TInput, Func<TComp>, bool> evaluator,
			TOutput defaultValue,
			params Func<ChooseOperand<TComp, TOutput>, ChooseOperand<TComp, TOutput>>[] opFuncs)
		{
			foreach (var opFunc in opFuncs)
			{
				var operand = opFunc(new ChooseOperand<TComp, TOutput>());
				if (evaluator(value, () => operand.IfValue))
					return operand.ThenValue;
			}
			return defaultValue;
		}

		public static TOutput Choose<TInput, TOutput>
			(this TInput value, Func<TInput, Func<TInput>, bool> evaluator,
			TOutput defaultValue,
			params Func<ChooseOperand<TInput, TOutput>, ChooseOperand<TInput, TOutput>>[] opFuncs)
		{
			foreach (var opFunc in opFuncs)
			{
				var operand = opFunc(new ChooseOperand<TInput, TOutput>());
				if (evaluator(value, () => operand.IfValue))
					return operand.ThenValue;
			}
			return defaultValue;
		}

	}
}
