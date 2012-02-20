using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;

namespace NPhp.Codegen
{
	public partial class SafeILGenerator
	{
		public void UnaryOperation(UnaryOperatorEnum Operator)
		{
			if (TrackStack)
			{
				var TypeRight = TypeStack.Pop();
				var TypeLeft = TypeStack.Pop();

				if (TypeLeft != TypeRight) throw (new InvalidOperationException("Binary operation mismatch"));

				TypeStack.Push(TypeRight);
			}
			if (DoEmit)
			{
				switch (Operator)
				{
					case UnaryOperatorEnum.Negate: ILGenerator.Emit(OpCodes.Neg); break;
					case UnaryOperatorEnum.Not: ILGenerator.Emit(OpCodes.Not); break;
					default: throw (new NotImplementedException());
				}
			}
		}

		public void StoreElement(Type Type)
		{
			if (TrackStack)
			{
				var StoreValueType = TypeStack.Pop();
				var StoreIndexValue = TypeStack.Pop();
				var StoreArrayValue = TypeStack.Pop();
			}

			if (DoEmit)
			{
				while (true)
				{
					if (Type == typeof(bool)) { ILGenerator.Emit(OpCodes.Stelem_I); break; }
					if (Type == typeof(sbyte) || Type == typeof(byte)) { ILGenerator.Emit(OpCodes.Stelem_I1); break; }
					if (Type == typeof(short) || Type == typeof(ushort)) { ILGenerator.Emit(OpCodes.Stelem_I2); break; }
					if (Type == typeof(int) || Type == typeof(uint)) { ILGenerator.Emit(OpCodes.Stelem_I4); break; }
					if (Type == typeof(float)) { ILGenerator.Emit(OpCodes.Stelem_R4); break; }
					if (Type == typeof(double)) { ILGenerator.Emit(OpCodes.Stelem_R8); break; }
					throw (new NotImplementedException());
				}
			}
		}

		public void StoreElement<TType>()
		{
			StoreElement(typeof(TType));
		}

		public void StoreIndirect(Type Type)
		{
			if (TrackStack)
			{
				var StoreValueType = TypeStack.Pop();
				var StoreAddressValue = TypeStack.Pop();
			}

			if (DoEmit)
			{
				while (true)
				{
					if (Type == typeof(bool)) { ILGenerator.Emit(OpCodes.Stind_I); break; }
					if (Type == typeof(sbyte) || Type == typeof(byte)) { ILGenerator.Emit(OpCodes.Stind_I1); break; }
					if (Type == typeof(short) || Type == typeof(ushort)) { ILGenerator.Emit(OpCodes.Stind_I2); break; }
					if (Type == typeof(int) || Type == typeof(uint)) {ILGenerator.Emit(OpCodes.Stind_I4); break; }
					if (Type == typeof(float)) { ILGenerator.Emit(OpCodes.Stind_R4); break; }
					if (Type == typeof(double)) { ILGenerator.Emit(OpCodes.Stind_R8); break; }
					throw(new NotImplementedException());
				}
			}
		}

		public void StoreIndirect<TType>()
		{
			StoreIndirect(typeof(TType));
		}

		public void StoreLocation<TType>(LocalBuilder Local)
		{
			if (TrackStack)
			{
				var StoreValueType = TypeStack.Pop();
			}

			if (DoEmit)
			{
				int LocalIndex = Local.LocalIndex;
				switch (LocalIndex)
				{
					case 0: ILGenerator.Emit(OpCodes.Stloc_0); break;
					case 1: ILGenerator.Emit(OpCodes.Stloc_1); break;
					case 2: ILGenerator.Emit(OpCodes.Stloc_2); break;
					case 3: ILGenerator.Emit(OpCodes.Stloc_3); break;
					default: ILGenerator.Emit(((int)(byte)LocalIndex == (int)LocalIndex) ? OpCodes.Stloc_S : OpCodes.Stloc, Local); break;
				}
			}
		}

		public void StoreArgument<TType>(int ArgumentIndex)
		{
			if (TrackStack)
			{
				var ArgumentIndexType = TypeStack.Pop();
				var ArgumentValueType = TypeStack.Pop();
			}

			if (DoEmit)
			{
				ILGenerator.Emit(((int)(byte)ArgumentIndex == (int)ArgumentIndex) ? OpCodes.Starg_S : OpCodes.Starg, ArgumentIndex);
			}
		}

		public void Box<TType>()
		{
			Box(typeof(TType));
		}

		public void Box(Type Type)
		{
			if (TrackStack)
			{
				var TypeType = TypeStack.Pop();
				if (TypeType != Type) throw(new InvalidCastException());
				TypeStack.Push(typeof(object));
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Box, Type);
			}
		}

		public void Unbox(Type Type)
		{
			if (TrackStack)
			{
				var ObjectType = TypeStack.Pop();
				TypeStack.Push(Type);
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Unbox);
			}
		}

		public void Unbox<TType>()
		{
			Unbox(typeof(TType));
		}

		public void SetPointerAttributes(PointerAttributesSet Attributes)
		{
			if (TrackStack)
			{
				var AddressType = TypeStack.Pop();
				TypeStack.Push(AddressType); // Unaligned
			}

			if (DoEmit)
			{
				if ((Attributes & PointerAttributesSet.Unaligned) != 0)
				{
					ILGenerator.Emit(OpCodes.Unaligned);
				}

				if ((Attributes & PointerAttributesSet.Volatile) != 0)
				{
					ILGenerator.Emit(OpCodes.Volatile);
				}
			}
		}

		public void Tailcall()
		{
			throw (new NotImplementedException());
			//ILGenerator.Emit(OpCodes.Tailcall);
		}

		public void Switch(Dictionary<int, Label> Labels, Label DefaultLabel)
		{
			var MinKey = Labels.Keys.Min();
			var MaxKey = Labels.Keys.Max();
			var Length = MaxKey - MinKey;

			BranchAlways(DefaultLabel);
			throw (new NotImplementedException());
		}

		public void Switch(Label[] Labels)
		{
			if (TrackStack)
			{
				var IndexType = TypeStack.Pop();
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Switch, Labels.Select(Label => Label.ReflectionLabel).ToArray());
			}
		}

		public void Sizeof()
		{
			if (TrackStack)
			{
				var BaseType = TypeStack.Pop();
				TypeStack.Push(typeof(int));
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Sizeof);
			}
		}

		public void Ret()
		{
			if (TrackStack)
			{
				// @TODO CHECK RETURN TYPE
				//var ReturnType = TypeStack.Pop();
				throw(new NotImplementedException());
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ret);
			}
		}

		public void NoOperation()
		{
			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Nop);
			}
		}
		
		public void BinaryOperation(BinaryOperatorEnum Operator)
		{
			if (TrackStack)
			{
				var TypeRight = TypeStack.Pop();
				var TypeLeft = TypeStack.Pop();

				if (TypeLeft != TypeRight) throw (new InvalidOperationException("Binary operation mismatch"));

				TypeStack.Push(TypeRight);
			}

			if (DoEmit)
			{
				switch (Operator)
				{
					case BinaryOperatorEnum.AdditionSigned: ILGenerator.Emit(OverflowCheck ? OpCodes.Add_Ovf : OpCodes.Add); break;
					case BinaryOperatorEnum.AdditionUnsigned: ILGenerator.Emit(OverflowCheck ? OpCodes.Add_Ovf_Un : OpCodes.Add); break;
					case BinaryOperatorEnum.SubstractionSigned: ILGenerator.Emit(OverflowCheck ? OpCodes.Sub_Ovf : OpCodes.Sub); break;
					case BinaryOperatorEnum.SubstractionUnsigned: ILGenerator.Emit(OverflowCheck ? OpCodes.Sub_Ovf_Un : OpCodes.Sub); break;
					case BinaryOperatorEnum.DivideSigned: ILGenerator.Emit(OpCodes.Div); break;
					case BinaryOperatorEnum.DivideUnsigned: ILGenerator.Emit(OpCodes.Div_Un); break;
					case BinaryOperatorEnum.RemainingSigned: ILGenerator.Emit(OpCodes.Rem); break;
					case BinaryOperatorEnum.RemainingUnsigned: ILGenerator.Emit(OpCodes.Rem_Un); break;
					case BinaryOperatorEnum.MultiplySigned: ILGenerator.Emit(OverflowCheck ? OpCodes.Mul_Ovf : OpCodes.Mul); break;
					case BinaryOperatorEnum.MultiplyUnsigned: ILGenerator.Emit(OverflowCheck ? OpCodes.Mul_Ovf_Un : OpCodes.Mul); break;
					case BinaryOperatorEnum.And: ILGenerator.Emit(OpCodes.And); break;
					case BinaryOperatorEnum.Or: ILGenerator.Emit(OpCodes.Or); break;
					case BinaryOperatorEnum.Xor: ILGenerator.Emit(OpCodes.Xor); break;
					case BinaryOperatorEnum.ShiftLeft: ILGenerator.Emit(OpCodes.Shl); break;
					case BinaryOperatorEnum.ShiftRightSigned: ILGenerator.Emit(OpCodes.Shr); break;
					case BinaryOperatorEnum.ShiftRightUnsigned: ILGenerator.Emit(OpCodes.Shr_Un); break;
					default: throw (new NotImplementedException());
				}
			}
		}

		public void Duplicate()
		{
			if (TrackStack)
			{
				var TypeToDuplicate = TypeStack.Pop();
				TypeStack.Push(TypeToDuplicate);
				TypeStack.Push(TypeToDuplicate);
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Dup);
			}
		}

		public void CompareBinary(BinaryComparison2Enum Comparison)
		{
			if (TrackStack)
			{
				var TypeRight = TypeStack.Pop();
				var TypeLeft = TypeStack.Pop();

				if (TypeLeft != TypeRight) throw (new InvalidOperationException("Binary operation mismatch"));

				TypeStack.Push(typeof(bool));
			}

			if (DoEmit)
			{
				switch (Comparison)
				{
					case BinaryComparison2Enum.Equals: ILGenerator.Emit(OpCodes.Ceq); break;
					case BinaryComparison2Enum.GreaterThanSigned: ILGenerator.Emit(OpCodes.Cgt); break;
					case BinaryComparison2Enum.GreaterThanUnsigned: ILGenerator.Emit(OpCodes.Cgt_Un); break;
					case BinaryComparison2Enum.LessThanSigned: ILGenerator.Emit(OpCodes.Clt); break;
					case BinaryComparison2Enum.LessThanUnsigned: ILGenerator.Emit(OpCodes.Clt_Un); break;
					default: throw(new NotImplementedException());
				}
			}
		}

		public void ConvertTo<TType>()
		{
		}

		public void ConvertTo(Type Type)
		{
			// @TODO: From unsigned values

			if (TrackStack)
			{
				var PreviousType = TypeStack.Pop();
				TypeStack.Push(Type);
			}

			if (DoEmit)
			{
				while (true)
				{
					if (Type == typeof(bool)) { ILGenerator.Emit(OverflowCheck ? OpCodes.Conv_Ovf_I : OpCodes.Conv_I); break; }
					if (Type == typeof(sbyte)) { ILGenerator.Emit(OverflowCheck ? OpCodes.Conv_Ovf_I1 : OpCodes.Conv_I1); break; }
					if (Type == typeof(byte)) { ILGenerator.Emit(OverflowCheck ? OpCodes.Conv_Ovf_U1 : OpCodes.Conv_U1); break; }
					if (Type == typeof(short)) { ILGenerator.Emit(OverflowCheck ? OpCodes.Conv_Ovf_I2 : OpCodes.Conv_I2); break; }
					if (Type == typeof(ushort)) { ILGenerator.Emit(OverflowCheck ? OpCodes.Conv_Ovf_U2 : OpCodes.Conv_U2); break; }
					if (Type == typeof(int)) { ILGenerator.Emit(OverflowCheck ? OpCodes.Conv_Ovf_I4 : OpCodes.Conv_I4); break; }
					if (Type == typeof(uint)) { ILGenerator.Emit(OverflowCheck ? OpCodes.Conv_Ovf_U4 : OpCodes.Conv_U4); break; }
					if (Type == typeof(long)) { ILGenerator.Emit(OverflowCheck ? OpCodes.Conv_Ovf_I8 : OpCodes.Conv_I8); break; }
					if (Type == typeof(ulong)) { ILGenerator.Emit(OverflowCheck ? OpCodes.Conv_Ovf_U8 : OpCodes.Conv_U8); break; }
					if (Type == typeof(float)) { ILGenerator.Emit(OpCodes.Conv_R4); break; }
					if (Type == typeof(double)) { ILGenerator.Emit(OpCodes.Conv_R8); break; }

					throw (new NotImplementedException());
				}
			}
		}

		public void CopyBlock()
		{
			if (TrackStack)
			{
				var CountType = TypeStack.Pop();
				var DestinationAddressType = TypeStack.Pop();
				var SourceAddressType = TypeStack.Pop();
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Cpblk);
			}

			throw (new NotImplementedException());
		}

		public void CopyObject()
		{
			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Cpobj);
			}

			throw (new NotImplementedException());
		}

		public void Constrained()
		{
			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Constrained);
			}
		}

		public void CheckFinite()
		{
			if (TrackStack)
			{
				var Type = TypeStack.Pop();
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ckfinite);
			}
		}

		public void BranchBinaryComparison(BinaryComparisonEnum Comparison, Label Label)
		{
			if (TrackStack)
			{
				var TypeRight = TypeStack.Pop();
				var TypeLeft = TypeStack.Pop();

				if (TypeLeft != TypeRight) throw (new InvalidOperationException("Binary operation mismatch"));
			}

			if (DoEmit)
			{
				switch (Comparison)
				{
					case BinaryComparisonEnum.Equals: ILGenerator.Emit(OpCodes.Beq, Label.ReflectionLabel); break;
					case BinaryComparisonEnum.NotEquals: ILGenerator.Emit(OpCodes.Bne_Un, Label.ReflectionLabel); break;

					case BinaryComparisonEnum.GreaterOrEqualSigned: ILGenerator.Emit(OpCodes.Bge, Label.ReflectionLabel); break;
					case BinaryComparisonEnum.GreaterOrEqualUnsigned: ILGenerator.Emit(OpCodes.Bge_Un, Label.ReflectionLabel); break;
					case BinaryComparisonEnum.GreaterThanSigned: ILGenerator.Emit(OpCodes.Bgt, Label.ReflectionLabel); break;
					case BinaryComparisonEnum.GreaterThanUnsigned: ILGenerator.Emit(OpCodes.Bgt_Un, Label.ReflectionLabel); break;

					case BinaryComparisonEnum.LessOrEqualSigned: ILGenerator.Emit(OpCodes.Ble, Label.ReflectionLabel); break;
					case BinaryComparisonEnum.LessOrEqualUnsigned: ILGenerator.Emit(OpCodes.Ble_Un, Label.ReflectionLabel); break;
					case BinaryComparisonEnum.LessThanSigned: ILGenerator.Emit(OpCodes.Blt, Label.ReflectionLabel); break;
					case BinaryComparisonEnum.LessThanUnsigned: ILGenerator.Emit(OpCodes.Blt_Un, Label.ReflectionLabel); break;

					default: throw (new NotImplementedException());
				}
			}
		}

		private void _BranchUnaryComparison(UnaryComparisonEnum Comparison, Label Label)
		{
			if (TrackStack)
			{
				var Type = TypeStack.Pop();

				if (Type != typeof(bool)) throw (new InvalidOperationException("Required boolean value"));
			}

			if (DoEmit)
			{
				switch (Comparison)
				{
					case UnaryComparisonEnum.False: ILGenerator.Emit(OpCodes.Brfalse, Label.ReflectionLabel); break;
					case UnaryComparisonEnum.True: ILGenerator.Emit(OpCodes.Brtrue, Label.ReflectionLabel); break;
					default: throw (new NotImplementedException());
				}
			}
		}

		public void BranchIfTrue(Label Label)
		{
			_BranchUnaryComparison(UnaryComparisonEnum.True, Label);
		}

		public void BranchIfFalse(Label Label)
		{
			_BranchUnaryComparison(UnaryComparisonEnum.False, Label);
		}

		private void _Jmp_Call(OpCode OpCode, MethodInfo MethodInfo)
		{
			if (TrackStack)
			{
				foreach (var Parameter in MethodInfo.GetParameters().Reverse())
				{
					var StackParameterType = TypeStack.Pop();
					if (Parameter.ParameterType != StackParameterType) throw (new InvalidOperationException("Type mismatch"));
				}

				if (MethodInfo.ReturnType != typeof(void))
				{
					TypeStack.Push(MethodInfo.ReturnType);
				}
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCode, MethodInfo);
			}
		}

		public void ResetStack()
		{
			int StackCount = TypeStack.Count;

			for (int n = 0; n > StackCount; n++) Pop();
		}

		public void Jmp(MethodInfo MethodInfo)
		{
			ResetStack();
			_Jmp_Call(OpCodes.Jmp, MethodInfo);
		}

		public void Call(MethodInfo MethodInfo)
		{
			_Jmp_Call(OpCodes.Call, MethodInfo);
		}

		public void Break()
		{
			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Break);
			}
		}

		public void BranchAlways(Label Label)
		{
			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Br, Label.ReflectionLabel);
			}
		}

		public void Pop()
		{
			if (TrackStack)
			{
				TypeStack.Pop();
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Pop);
			}
		}

		public void LoadArgument<TType>(int Index)
		{
			if (TrackStack)
			{
				TypeStack.Push(typeof(TType));
			}

			if (DoEmit)
			{
				switch (Index)
				{
					case 0: ILGenerator.Emit(OpCodes.Ldarg_0); break;
					case 1: ILGenerator.Emit(OpCodes.Ldarg_1); break;
					case 2: ILGenerator.Emit(OpCodes.Ldarg_2); break;
					case 3: ILGenerator.Emit(OpCodes.Ldarg_3); break;
					default: ILGenerator.Emit(((int)(byte)Index == (int)Index) ? OpCodes.Ldarg_S : OpCodes.Ldarg, Index); break;
				}
			}
		}

		public void LoadArgumentFromIndexAtStack()
		{
			if (TrackStack)
			{
				var Type = TypeStack.Pop();
				
				// @check type is integer

				TypeStack.Push(typeof(object));
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ldarg_S);
			}

			//OpCodes.Ldarga;
			//OpCodes.Ldarga_S;
			//OpCodes.
		}

		public void Push(int Value)
		{
			if (TrackStack)
			{
				TypeStack.Push(typeof(int));
			}

			if (DoEmit)
			{
				switch (Value)
				{
					case -1: ILGenerator.Emit(OpCodes.Ldc_I4_M1); break;
					case 0: ILGenerator.Emit(OpCodes.Ldc_I4_0); break;
					case 1: ILGenerator.Emit(OpCodes.Ldc_I4_1); break;
					case 2: ILGenerator.Emit(OpCodes.Ldc_I4_2); break;
					case 3: ILGenerator.Emit(OpCodes.Ldc_I4_3); break;
					case 4: ILGenerator.Emit(OpCodes.Ldc_I4_4); break;
					case 5: ILGenerator.Emit(OpCodes.Ldc_I4_5); break;
					case 6: ILGenerator.Emit(OpCodes.Ldc_I4_6); break;
					case 7: ILGenerator.Emit(OpCodes.Ldc_I4_7); break;
					case 8: ILGenerator.Emit(OpCodes.Ldc_I4_8); break;
					default:
						if ((int)(sbyte)Value == (int)Value)
						{
							ILGenerator.Emit(OpCodes.Ldc_I4_S, (sbyte)Value);
						}
						else
						{
							ILGenerator.Emit(OpCodes.Ldc_I4, Value);
						}
						break;
				}
			}
		}

		public void Push(long Value)
		{
			if (TrackStack)
			{
				TypeStack.Push(typeof(long));
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ldc_I8, Value);
			}
		}

		public void Push(float Value)
		{
			if (TrackStack)
			{
				TypeStack.Push(typeof(float));
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ldc_R4, Value);
			}
		}

		public void Push(double Value)
		{
			if (TrackStack)
			{
				TypeStack.Push(typeof(float));
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ldc_R8, Value);
			}
		}

		public void Push(string Value)
		{
			if (TrackStack)
			{
				TypeStack.Push(typeof(string));
			}

			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ldstr, Value);
			}
		}

		private void _LoadElement_Reference_Address(OpCode OpCode)
		{
			if (TrackStack)
			{
				var IndexType = TypeStack.Pop();
				var ArrayType = TypeStack.Pop();

				// @TODO: reference
				TypeStack.Push(typeof(object));
			}
			if (DoEmit)
			{
				ILGenerator.Emit(OpCode);
			}
		}

		public void LoadElementAddress()
		{
			_LoadElement_Reference_Address(OpCodes.Ldelema);
		}

		public void LoadElementReference()
		{
			_LoadElement_Reference_Address(OpCodes.Ldelem_Ref);
		}

		private void _LoadField_Address(OpCode OpCode)
		{
			if (TrackStack)
			{
				var FieldInfoType = TypeStack.Pop();
				var ObjectType = TypeStack.Pop();

				// @TODO: Field reference
				TypeStack.Push(typeof(object));
			}
			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ldfld);
			}
		}

		public void LoadField()
		{
			_LoadField_Address(OpCodes.Ldfld);
		}

		public void LoadFieldAddress()
		{
			_LoadField_Address(OpCodes.Ldflda);
		}

		public void LoadMethodAddress()
		{
			if (TrackStack)
			{
				var MethodInfoType = TypeStack.Pop();
				var ObjectType = TypeStack.Pop();

				// @TODO: Field reference
				TypeStack.Push(typeof(object));
			}
			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ldftn);
			}
			//OpCodes.Ldind_I
		}

		public void LoadLength()
		{
			if (TrackStack)
			{
				var ArrayType = TypeStack.Pop();
				TypeStack.Push(typeof(int));
			}
			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ldlen); 
			}
		}

		public void LoadLocal(LocalBuilder Local)
		{
			if (TrackStack)
			{
				TypeStack.Push(Local.LocalType);
			}
			if (DoEmit)
			{
				int LocalIndex = Local.LocalIndex;
				switch (Local.LocalIndex)
				{
					case 0: ILGenerator.Emit(OpCodes.Ldloc_0); break;
					case 1: ILGenerator.Emit(OpCodes.Ldloc_1); break;
					case 2: ILGenerator.Emit(OpCodes.Ldloc_2); break;
					case 3: ILGenerator.Emit(OpCodes.Ldloc_3); break;
					default: ILGenerator.Emit(((int)(byte)LocalIndex == (int)LocalIndex) ? OpCodes.Ldloc_S : OpCodes.Ldloc, Local); break;
				}
			}
		}

		public void LoadLocalAddress(LocalBuilder Local)
		{
			if (TrackStack)
			{
				// @TODO: Address
				TypeStack.Push(Local.LocalType);
			}
			if (DoEmit)
			{
				int LocalIndex = Local.LocalIndex;

				ILGenerator.Emit(((int)(byte)LocalIndex == (int)LocalIndex) ? OpCodes.Ldloca_S : OpCodes.Ldloca);
			}
		}

		public void LoadNull()
		{
			if (TrackStack)
			{
				TypeStack.Push(typeof(DBNull));
			}
			if (DoEmit)
			{
				ILGenerator.Emit(OpCodes.Ldnull);
			}
		}

		public void LoadIndirect(Type Type)
		{
			if (TrackStack)
			{
				var PointerType = TypeStack.Pop();
				TypeStack.Push(Type);
			}

			if (DoEmit)
			{
				while (true)
				{
					if (Type == typeof(bool)) { ILGenerator.Emit(OpCodes.Ldind_I); break; }
					if (Type == typeof(sbyte)) { ILGenerator.Emit(OpCodes.Ldind_I1); break; }
					if (Type == typeof(short)) { ILGenerator.Emit(OpCodes.Ldind_I2); break; }
					if (Type == typeof(int)) { ILGenerator.Emit(OpCodes.Ldind_I4); break; }
					if (Type == typeof(long)) { ILGenerator.Emit(OpCodes.Ldind_I8); break; }
					if (Type == typeof(float)) { ILGenerator.Emit(OpCodes.Ldind_R4); break; }
					if (Type == typeof(double)) { ILGenerator.Emit(OpCodes.Ldind_R8); break; }
					if (Type == typeof(byte)) { ILGenerator.Emit(OpCodes.Ldind_U1); break; }
					if (Type == typeof(ushort)) { ILGenerator.Emit(OpCodes.Ldind_U2); break; }
					if (Type == typeof(uint)) { ILGenerator.Emit(OpCodes.Ldind_U4); break; }
					//ILGenerator.Emit(OpCodes.Ldelem);
					throw (new NotImplementedException());
				}
			}
		}

		public void LoadElementFromArray(Type Type)
		{
			if (TrackStack)
			{
				var IndexType = TypeStack.Pop();
				var ArrayType = TypeStack.Pop();
				TypeStack.Push(Type);
			}

			if (DoEmit)
			{
				while (true)
				{
					if (Type == typeof(bool)) { ILGenerator.Emit(OpCodes.Ldelem_I); break; }
					if (Type == typeof(sbyte)) { ILGenerator.Emit(OpCodes.Ldelem_I1); break; }
					if (Type == typeof(short)) { ILGenerator.Emit(OpCodes.Ldelem_I2); break; }
					if (Type == typeof(int)) { ILGenerator.Emit(OpCodes.Ldelem_I4); break; }
					if (Type == typeof(long)) { ILGenerator.Emit(OpCodes.Ldelem_I8); break; }
					if (Type == typeof(float)) { ILGenerator.Emit(OpCodes.Ldelem_R4); break; }
					if (Type == typeof(double)) { ILGenerator.Emit(OpCodes.Ldelem_R8); break; }
					if (Type == typeof(byte)) { ILGenerator.Emit(OpCodes.Ldelem_U1); break; }
					if (Type == typeof(ushort)) { ILGenerator.Emit(OpCodes.Ldelem_U2); break; }
					if (Type == typeof(uint)) { ILGenerator.Emit(OpCodes.Ldelem_U4); break; }
					//ILGenerator.Emit(OpCodes.Ldelem);
					throw (new NotImplementedException());
				}
			}
		}

		public void LoadElementFromArray<TType>()
		{
			LoadElementFromArray(typeof(TType));
		}

		public void Push(uint Value)
		{
			Push(unchecked((int)Value));
		}

		public void Push(ulong Value)
		{
			Push(unchecked((long)Value));
		}

		public void PendingOpcodes()
		{
			//OpCodes.Endfilter;
			//OpCodes.Endfinally;
			//OpCodes.Initblk;
			//OpCodes.Initobj;
			//OpCodes.Isinst;
			//OpCodes.Newarr;
			//OpCodes.Newobj;
			//OpCodes.Prefix1;
			//OpCodes.Prefix2;
			//OpCodes.Prefix3;
			//OpCodes.Prefix4;
			//OpCodes.Prefix5;
			//OpCodes.Prefix6;
			//OpCodes.Prefix7;
			//OpCodes.Prefixref;
			//OpCodes.Readonly
			//OpCodes.Refanytype
			//OpCodes.Refanyval
			//OpCodes.Sizeof
			//OpCodes.Unaligned
			//OpCodes.Unbox
			//OpCodes.Volatile
			//OpCodes.Stobj
			//OpCodes.Stfld
			//OpCodes.Stind_Ref
			//OpCodes.Stelem_Ref
			//OpCodes.Ldobj;
			//OpCodes.Ldsfld;
			//OpCodes.Ldsflda;
			//OpCodes.Ldtoken;
			//OpCodes.Ldvirtftn;
			//OpCodes.Leave;
			//OpCodes.Leave_S;
			//OpCodes.Localloc;
			//OpCodes.Mkrefany;
			throw (new NotImplementedException());
		}
	}
}
