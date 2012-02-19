using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPhp.Runtime
{
	public sealed class Php54Var
	{
		//private Type Type;
		internal bool IsRef = false;
		private dynamic DynamicValue;
		static private readonly Type BoolType = typeof(bool);
		private Type Type
		{
			get
			{
				if (DynamicValue == null) return null;
				return DynamicValue.GetType();
			}
		}

		/*
		public dynamic DynamicValue
		{
			get
			{
				return Value;
			}
		}
		*/
		public string StringValue
		{
			get
			{
				if (Type == null) return "";
				if (Type == BoolType) return (DynamicValue) ? "1" : "";
				return DynamicValue.ToString();
			}
		}
		public bool BoolValue
		{
			get
			{
				if (Type == BoolType) return DynamicValue;
				return (DynamicValue != 0);
			}
		}
		public double DoubleValue
		{
			get
			{
				if (Type == null) return 0;
				if (Type == typeof(double)) return DynamicValue;
				var Str = StringValue;
				int value = 0;
				for (int n = 0; n < Str.Length; n++)
				{
					if (Str[n] >= '0' && Str[n] <= '9')
					{
						value *= 10;
						value += Str[n] - '0';
					}
					else
					{
						break;
					}
				}
				//return double.Parse(StringValue);
				return value;
			}
		}

		public Php54Var(dynamic Value, bool IsRef = false)
		{
			this.DynamicValue = Value;
			this.IsRef = IsRef;
			//this.Type = (Value != null) ? Value.GetType() : null;
		}

		static public Php54Var CreateRef(Php54Var ReferencedValue)
		{
			return new Php54Var(ReferencedValue, IsRef: true);
		}

		static public Php54Var FromInt(int Value)
		{
			return new Php54Var(Value);
		}

		static public Php54Var FromString(string Value)
		{
			return new Php54Var(Value);
		}

		static public Php54Var FromTrue()
		{
			return new Php54Var(true);
		}

		static public Php54Var FromFalse()
		{
			return new Php54Var(false);
		}

		static public Php54Var FromNull()
		{
			return new Php54Var(null);
		}

		static public Php54Var Add(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.DoubleValue + Right.DoubleValue);
		}

		static public Php54Var Concat(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.StringValue + Right.StringValue);
		}

		static public Php54Var Sub(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.DynamicValue - Right.DynamicValue);
		}

		static public Php54Var Mul(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.DynamicValue * Right.DynamicValue);
		}

		static public Php54Var Div(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.DynamicValue / Right.DynamicValue);
		}

		static public Php54Var UnaryAdd(Php54Var Right)
		{
			return new Php54Var(+Right.DynamicValue);
		}

		static public Php54Var UnaryPostInc(Php54Var Left, int Count)
		{
			var Old = Left.DoubleValue;
			Left.DynamicValue = Old + Count;
			return new Php54Var(Old);
		}

		static public Php54Var UnaryPreInc(Php54Var Left, int Count)
		{
			Left.DynamicValue = Left.DoubleValue + Count;
			return new Php54Var(Left.DynamicValue);
		}

		static public Php54Var UnarySub(Php54Var Right)
		{
			return new Php54Var(-Right.DoubleValue);
		}

		public static Php54Var CompareEquals(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.DoubleValue == Right.DoubleValue);
		}

		public static Php54Var CompareGreaterThan(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.DoubleValue > Right.DoubleValue);
		}

		public static Php54Var CompareLessThan(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.DoubleValue < Right.DoubleValue);
		}

		public static Php54Var CompareNotEquals(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.DynamicValue != Right.DynamicValue);
		}

		public static Php54Var LogicalAnd(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.BoolValue && Right.BoolValue);
		}

		static public void Assign(Php54Var Left, Php54Var Right)
		{
			if (Left.IsRef)
			{
				Assign(Left.DynamicValue, Right);
				return;
			}

			if (Right.IsRef)
			{
				Left.DynamicValue = Right;
				Left.IsRef = true;
				return;
			}

			Left.DynamicValue = Right.DynamicValue;
			//Left.Type = Right.Type;
		}

		static public bool ToBool(Php54Var Variable)
		{
			return Variable.BoolValue;
		}

		public override string ToString()
		{
			return StringValue;
		}
	}

}
