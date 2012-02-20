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

		static private readonly Type NullType = typeof(DBNull);
		static private readonly Type BoolType = typeof(bool);
		static private readonly Type IntType = typeof(int);
		static private readonly Type DoubleType = typeof(double);
		static private readonly Type StringType = typeof(string);

		/*
		public enum TypeEnum
		{
			Null,
			Bool,
			Int,
			Double,
			String,
		}
		*/

		private Type _Type;

		private Type Type
		{
			get
			{
				if (_Type == null)
				{
					if (DynamicValue == null) return null;
					_Type = DynamicValue.GetType();
				}
				return _Type;
			}
		}

		static public readonly Php54Var Methods = new Php54Var(null, null, false);

		public double GetDoubleOrDefault(double DefaultValue)
		{
			if (DynamicValue == null) return DefaultValue;
			return DoubleValue;
		}

		public string GetStringOrDefault(string DefaultValue)
		{
			if (DynamicValue == null) return DefaultValue;
			return StringValue;
		}

		public int GetIntegerOrDefault(int DefaultValue)
		{
			if (DynamicValue == null) return DefaultValue;
			return IntegerValue;
		}

		public bool GetBooleanOrDefault(bool DefaultValue)
		{
			if (DynamicValue == null) return DefaultValue;
			return BoolValue;
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
				if (Type == IntType) return (double)DynamicValue;
				if (Type == DoubleType) return DynamicValue;
				if (Type == null) return 0;
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

		public int IntegerValue
		{
			get
			{
				if (Type == IntType) return DynamicValue;
				if (Type == DoubleType) return (int)(double)DynamicValue;
				if (Type == null) return 0;
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

		public dynamic NumericValue
		{
			get
			{
				if (Type == null) return 0;
				if ((Type == typeof(int)) || (Type == typeof(double))) return DynamicValue;
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


		public Php54Var(dynamic Value, Type Type = null, bool IsRef = false)
		{
			this.DynamicValue = Value;
			this._Type = Type;
			this.IsRef = IsRef;
			//this.Type = (Value != null) ? Value.GetType() : null;
		}

		static public Php54Var CreateRef(Php54Var ReferencedValue)
		{
			return new Php54Var(ReferencedValue, ReferencedValue.Type, IsRef: true);
		}

		static public Php54Var FromInt(int Value)
		{
			return new Php54Var(Value, IntType);
		}

		static public Php54Var FromString(string Value)
		{
			return new Php54Var(Value, StringType);
		}

		static public Php54Var FromTrue()
		{
			return new Php54Var(true, BoolType);
		}

		static public Php54Var FromFalse()
		{
			return new Php54Var(false, BoolType);
		}

		static public Php54Var FromNull()
		{
			return new Php54Var(null, NullType);
		}

		static public Php54Var Add(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.NumericValue + Right.NumericValue);
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
			//Left.Type
			return new Php54Var(Left.DynamicValue / Right.DynamicValue);
		}

		static public Php54Var UnaryPostInc(Php54Var Left, int Count)
		{
			var Old = Left.NumericValue;
			Left.DynamicValue = Old + Count;
			return new Php54Var(Old, Left.Type);
		}

		static public Php54Var UnaryPreInc(Php54Var Left, int Count)
		{
			Left.DynamicValue = Left.NumericValue + Count;
			return new Php54Var(Left.DynamicValue, Left.Type);
		}

		static public Php54Var UnaryAdd(Php54Var Right)
		{
			return new Php54Var(+Right.DynamicValue, Right.Type);
		}

		static public Php54Var UnarySub(Php54Var Right)
		{
			return new Php54Var(-Right.NumericValue, Right.Type);
		}

		public static Php54Var CompareEquals(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.NumericValue == Right.NumericValue, BoolType);
		}

		public static Php54Var CompareGreaterThan(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.NumericValue > Right.NumericValue, BoolType);
		}

		public static Php54Var CompareLessThan(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.NumericValue < Right.NumericValue, BoolType);
		}

		public static Php54Var CompareNotEquals(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.DynamicValue != Right.DynamicValue, BoolType);
		}

		public static Php54Var LogicalAnd(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.BoolValue && Right.BoolValue, BoolType);
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
			Left._Type = Right.Type;
		}

		public bool ToBool()
		{
			return BoolValue;
		}

		public override string ToString()
		{
			return StringValue;
		}
	}

}
