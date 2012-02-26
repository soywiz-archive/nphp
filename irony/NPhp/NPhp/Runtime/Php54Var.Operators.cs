using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPhp.Runtime
{
	public sealed partial class Php54Var
	{
		static public Php54Var BitAnd(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.IntegerValue & Right.IntegerValue, TypeEnum.Int);
		}

		static public Php54Var BitOr(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.IntegerValue | Right.IntegerValue, TypeEnum.Int);
		}

		static public Php54Var UnaryBitNot(Php54Var Left)
		{
			return new Php54Var(~Left.IntegerValue, TypeEnum.Int);
		}

		static public Php54Var UnaryLogicNot(Php54Var Left)
		{
			return new Php54Var(!Left.BooleanValue, TypeEnum.Bool);
		}

		static public Php54Var Add(Php54Var Left, Php54Var Right)
		{
			var CombinedType = CombineTypes(Left.ReferencedType, Right.ReferencedType);
			switch (CombinedType)
			{
				case TypeEnum.Int:
					try
					{
						checked
						{
							int Result = Left.IntegerValue + Right.IntegerValue;
							return new Php54Var(Result, TypeEnum.Int);
						}
					}
					catch (OverflowException)
					{
						return new Php54Var(Left.DoubleValue + Right.DoubleValue, TypeEnum.Double);
					}
				default:
					{
						return new Php54Var(Left.DoubleValue + Right.DoubleValue, TypeEnum.Double);
					}
			}
		}

		static public Php54Var Mul(Php54Var Left, Php54Var Right)
		{
			var CombinedType = CombineTypes(Left.ReferencedType, Right.ReferencedType);
			switch (CombinedType)
			{
				case TypeEnum.Int:
					try
					{
						checked
						{
							int Result = Left.IntegerValue * Right.IntegerValue;
							return new Php54Var(Result, TypeEnum.Int);
						}
					}
					catch (OverflowException)
					{
						return new Php54Var(Left.DoubleValue * Right.DoubleValue, TypeEnum.Double);
					}
				default:
					{
						return new Php54Var(Left.DoubleValue * Right.DoubleValue, TypeEnum.Double);
					}
			}
		}

		static public Php54Var Sub(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.NumericValue - Right.NumericValue, CombineTypes(Left.ReferencedType, Right.ReferencedType));
		}

		static public Php54Var Div(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.NumericValue / Right.NumericValue, CombineTypes(Left.ReferencedType, Right.ReferencedType));
		}

		static public Php54Var Mod(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.NumericValue % Right.NumericValue, CombineTypes(Left.ReferencedType, Right.ReferencedType));
		}

		static public Php54Var Concat(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.StringValue + Right.StringValue, CombineTypes(Left.ReferencedType, Right.ReferencedType));
		}

		static public Php54Var UnaryPostIncrement(Php54Var Left, int Count)
		{
			var Old = Left.NumericValue;
			Left.ReferencedDynamicValue = Old + Count;
			return new Php54Var(Old, Left.ReferencedType);
		}

		static public Php54Var UnaryPreIncrement(Php54Var Left, int Count)
		{
			Left.ReferencedDynamicValue = Left.NumericValue + Count;
			return new Php54Var(Left.ReferencedDynamicValue, Left.ReferencedType);
		}

		static public Php54Var UnarySilence(Php54Var Right)
		{
			return Right;
		}

		static public Php54Var UnaryAdd(Php54Var Right)
		{
			//return new Php54Var(+Right.DynamicValue, Right.Type);
			return Right;
		}

		static public Php54Var UnarySub(Php54Var Right)
		{
			return new Php54Var(-Right.NumericValue, Right.ReferencedType);
		}

		public static bool CompareStrictEquals(Php54Var Left, Php54Var Right)
		{
			//if (Left.ReferencedType != Right.ReferencedType) return false;
			return Left == Right;
		}

		public static bool CompareStrictNotEquals(Php54Var Left, Php54Var Right)
		{
			//if (Left.ReferencedType != Right.ReferencedType) return false;
			return !(CompareStrictEquals(Left, Right));
		}

		public static bool CompareEquals(Php54Var Left, Php54Var Right)
		{
			switch (CombineTypes(Left.ReferencedType, Right.ReferencedType))
			{
				case TypeEnum.Int: return Left.IntegerValue == Right.IntegerValue;
				case TypeEnum.Double: return Left.DoubleValue == Right.DoubleValue;
				default: return Left.StringValue == Right.StringValue;
			}
		}

		public static bool CompareNotEquals(Php54Var Left, Php54Var Right)
		{
			return !CompareEquals(Left, Right);
		}

		public static bool CompareGreaterThan(Php54Var Left, Php54Var Right)
		{
			return Left.NumericValue > Right.NumericValue;
		}

		public static bool CompareGreaterOrEqualThan(Php54Var Left, Php54Var Right)
		{
			return Left.NumericValue >= Right.NumericValue;
		}

		public static bool CompareLessOrEqualThan(Php54Var Left, Php54Var Right)
		{
			return Left.NumericValue <= Right.NumericValue;
		}

		public static bool CompareLessThan(Php54Var Left, Php54Var Right)
		{
			return Left.NumericValue < Right.NumericValue;
		}

		public static bool CompareLessThan(Php54Var Left, int Right)
		{
			return Left.IntegerValue < Right;
		}

		public static bool LogicalAnd(Php54Var Left, Php54Var Right)
		{
			return Left.BooleanValue && Right.BooleanValue;
		}

		public static bool LogicalOr(Php54Var Left, Php54Var Right)
		{
			return Left.BooleanValue || Right.BooleanValue;
		}

		public static bool LogicalAnd(bool Left, bool Right)
		{
			return Left && Right;
		}

		public static bool LogicalOr(bool Left, bool Right)
		{
			return Left || Right;
		}

		static public void Assign(Php54Var Left, Php54Var Right)
		{
			if (Left.IsReference)
			{
				Assign(Left.ReferencedValue, Right);
				return;
			}

			if (Right.IsReference)
			{
				Left.ReferencedDynamicValue = Right;
				Left.ThisType = TypeEnum.Reference;
				return;
			}

			switch (Right.ReferencedType)
			{
				case TypeEnum.Array:
					Left.ThisType = Right.ReferencedType;
					Left.ReferencedDynamicValue = Right.ArrayValue.Clone();
					break;
				default:
					Left.ThisType = Right.ReferencedType;
					Left.ReferencedDynamicValue = Right.ReferencedDynamicValue;
					break;
			}
		}

		static public void AssignAdd(Php54Var Left, Php54Var Right)
		{
			//Left.DynamicValue += Right.DynamicValue;
			Assign(Left, Add(Left, Right));
		}

		static public void AssignSub(Php54Var Left, Php54Var Right)
		{
			//Left.DynamicValue += Right.DynamicValue;
			Assign(Left, Sub(Left, Right));
		}
	}

}
