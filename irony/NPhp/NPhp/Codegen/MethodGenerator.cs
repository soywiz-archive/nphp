#define CODEGEN_TRACE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Diagnostics;
using System.Reflection;
using NPhp.Runtime;

namespace NPhp.Codegen
{
	public class MethodGenerator
	{
		private DynamicMethod DynamicMethod;
		private ILGenerator ILGenerator;
		public int StackCount;

		public MethodGenerator()
		{
			DynamicMethod = new DynamicMethod(
				"unknown",
				typeof(void),
				new Type[] { typeof(Php54Scope) }
			);
			ILGenerator = DynamicMethod.GetILGenerator();
		}

		public Action<Php54Scope> GenerateMethod()
		{
			ClearStack();
			ILGenerator.Emit(OpCodes.Ret);

			return (Action<Php54Scope>)DynamicMethod.CreateDelegate(typeof(Action<Php54Scope>));
		}

		public void Push(double Value)
		{
			ILGenerator.Emit(OpCodes.Ldc_R8, Value);

			StackCount++;

#if CODEGEN_TRACE
			Debug.WriteLine("PUSH<double>: {0} -> Stack: {1}", Value, StackCount);
#endif

		}

		public void Push(int Value)
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

			StackCount++;

#if CODEGEN_TRACE
			Debug.WriteLine("PUSH<int>: {0} -> Stack: {1}", Value, StackCount);
#endif
		}

		public void Push(string Value)
		{
			ILGenerator.Emit(OpCodes.Ldstr, Value);

			StackCount++;

#if CODEGEN_TRACE
			Debug.WriteLine("PUSH<string>: '{0}' -> Stack: {1}", Value, StackCount);
#endif
		}

		public void Operator(string Operator)
		{
			//Console.WriteLine("Operator");
			switch (Operator)
			{
				case "+": ILGenerator.Emit(OpCodes.Add); break;
				case "-": ILGenerator.Emit(OpCodes.Sub); break;
				case "*": ILGenerator.Emit(OpCodes.Mul); break;
				case "/": ILGenerator.Emit(OpCodes.Div); break;
				default: throw (new NotImplementedException());
			}
			StackCount--;

#if CODEGEN_TRACE
			Debug.WriteLine("Operator: '{0}' -> Stack: {1}", Operator, StackCount);
#endif
		}

		public Label DefineLabel()
		{
			return ILGenerator.DefineLabel();
		}

		public void MarkLabel(Label Label)
		{
#if CODEGEN_TRACE
			Debug.WriteLine("MarkLabel: '{0}'", Label);
#endif
			ILGenerator.MarkLabel(Label);
		}

		public void BranchIfTrue(Label Label)
		{
			ILGenerator.Emit(OpCodes.Brtrue, Label);
			StackCount--;
#if CODEGEN_TRACE
			Debug.WriteLine("BranchIfTrue: '{0}' -> Stack: {1}", Label, StackCount);
#endif
		}

		public void BranchIfFalse(Label Label)
		{

			ILGenerator.Emit(OpCodes.Brfalse, Label);
			StackCount--;
#if CODEGEN_TRACE
			Debug.WriteLine("BranchIfFalse: '{0}' -> Stack: {1}", Label, StackCount);
#endif
		}

		public void ClearStack()
		{
			for (; StackCount > 0; StackCount--)
			{
				ILGenerator.Emit(OpCodes.Pop);
			}
#if CODEGEN_TRACE
			Debug.WriteLine("ClearStack -> Stack: {0}", StackCount);
#endif
		}

		public void Call(Delegate Delegate)
		{
			Call(Delegate.Method);
		}

		public void Call(MethodInfo MethodInfo)
		{
			int PrevStackCount = StackCount;

			ILGenerator.Emit(OpCodes.Call, MethodInfo);
			int ThisCount = (!MethodInfo.IsStatic) ? 1 : 0;
			int ArgumentCount = MethodInfo.GetParameters().Length;
			int ReturnCount = (MethodInfo.ReturnType != typeof(void)) ? 1 : 0;
			StackCount -= ThisCount;
			StackCount -= ArgumentCount;
			StackCount += ReturnCount;

#if CODEGEN_TRACE
			Debug.WriteLine(
				"Call: {0}.{1} (this:{2}, args:{3}, ret:{4}) -> Stack: {5} -> {6}",
				MethodInfo.DeclaringType.Name,
				MethodInfo.Name,
				ThisCount,
				ArgumentCount,
				ReturnCount,
				PrevStackCount,
				StackCount
			);
#endif
		}

		public void BranchAlways(Label Label)
		{
			ILGenerator.Emit(OpCodes.Br, Label);
#if CODEGEN_TRACE
			Debug.WriteLine("BranchAlways: '{0}'", Label);
#endif
		}

		public void LoadScope()
		{
			LoadArgument<Php54Scope>(0);
		}

		private void LoadArgument<TType>(int ArgumentIndex)
		{
			switch (ArgumentIndex)
			{
				case 0: ILGenerator.Emit(OpCodes.Ldarg_0); break;
				case 1: ILGenerator.Emit(OpCodes.Ldarg_1); break;
				case 2: ILGenerator.Emit(OpCodes.Ldarg_2); break;
				case 3: ILGenerator.Emit(OpCodes.Ldarg_3); break;
				default: ILGenerator.Emit(OpCodes.Ldarg, ArgumentIndex); break;
			}

			StackCount++;

#if CODEGEN_TRACE
			Debug.WriteLine("LoadArgument<{0}>: {1} -> Stack: {2}", typeof(TType).Name, ArgumentIndex, StackCount);
#endif
			//throw new NotImplementedException();
		}

		public void Box<TType>()
		{
			Box(typeof(TType));
		}

		public void Box(Type Type)
		{
#if CODEGEN_TRACE
			Debug.WriteLine("Box: " + Type.Name);
#endif
			ILGenerator.Emit(OpCodes.Box, Type);
		}

		public void Unbox<TType>()
		{
#if CODEGEN_TRACE
			Debug.WriteLine("Unbox: " + typeof(TType).Name);
#endif

			ILGenerator.Emit(OpCodes.Unbox, typeof(TType));
		}

		public void Comment(string Comment)
		{
#if CODEGEN_TRACE
			Debug.WriteLine("--- " + Comment);
#endif
		}

		public void Dup()
		{
			ILGenerator.Emit(OpCodes.Dup);
			StackCount++;
#if CODEGEN_TRACE
			Debug.WriteLine("Dup -> Stack: {0}", StackCount);
#endif
		}

		public void Pop()
		{
			StackCount--;

#if CODEGEN_TRACE
			Debug.WriteLine("Pop -> Stack: {0}", StackCount);
#endif
		}

		public void Return()
		{
			ILGenerator.Emit(OpCodes.Ret);
#if CODEGEN_TRACE
			Debug.WriteLine("Ret");
#endif
		}

		public void ConvTo<TType>()
		{
#if CODEGEN_TRACE
			Debug.WriteLine("ConvTo: {0}", typeof(TType).Name);
#endif

			if (typeof(TType) == typeof(int))
			{
				ILGenerator.Emit(OpCodes.Conv_I4);
			}
			else
			{
				throw(new NotImplementedException());
			}
		}
	}
}
