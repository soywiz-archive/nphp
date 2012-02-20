//#define CODEGEN_TRACE

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
		public int StackCount
		{
			get
			{
				return _TypeStack.Count;
			}
		}
		public Stack<Type> _TypeStack = new Stack<Type>();

		public Type StackTop
		{
			get
			{
				var Type = _TypeStack.Pop();
				_TypeStack.Push(Type);
				return Type;
			}
		}

		public MethodGenerator()
		{
			//MethodAttributes.
			DynamicMethod = new DynamicMethod(
				"unknown",
				typeof(void),
				new Type[] { typeof(Php54Scope) }
			);
			ILGenerator = DynamicMethod.GetILGenerator();
		}

		Dictionary<string, LocalBuilder> Locals = new Dictionary<string,LocalBuilder>();

		public LocalBuilder GetCachedLocal(string Name, out bool Cached)
		{
			LocalBuilder LocalBuilder;
			if (!Locals.TryGetValue(Name, out LocalBuilder))
			{
				Locals[Name] = LocalBuilder = DeclareLocalPhp54Var();
				Cached = false;
			}
			else
			{
				Cached = true;
			}
			return LocalBuilder;
		}

		private LocalBuilder DeclareLocalPhp54Var()
		{
			return DeclareLocal<Php54Var>();
		}

		private LocalBuilder DeclareLocal<TType>()
		{
			return ILGenerator.DeclareLocal(typeof(TType));
		}

		public Action<Php54Scope> GenerateMethod()
		{
			ClearStack();
			ILGenerator.Emit(OpCodes.Ret);

			var Method = (Action<Php54Scope>)DynamicMethod.CreateDelegate(typeof(Action<Php54Scope>));
			
			//Delegate.
			return Method;
		}

		public void Push(double Value)
		{
			ILGenerator.Emit(OpCodes.Ldc_R8, Value);

			StackCountIncrement(typeof(double));

#if CODEGEN_TRACE
			Debug.WriteLine("PUSH<double>: {0} -> Stack: {1}", Value, StackCount);
#endif

		}

		public void Push(bool Value)
		{
			ILGenerator.Emit(Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);

			StackCountIncrement(typeof(bool));

#if CODEGEN_TRACE
			Debug.WriteLine("PUSH<bool>: {0} -> Stack: {1}", Value, StackCount);
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

			StackCountIncrement(typeof(int));

#if CODEGEN_TRACE
			Debug.WriteLine("PUSH<int>: {0} -> Stack: {1}", Value, StackCount);
#endif
		}

		public void Push(string Value)
		{
			ILGenerator.Emit(OpCodes.Ldstr, Value);

			StackCountIncrement(typeof(string));

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

			StackCountDecrement(1);

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
			ConvTo<bool>();
			ILGenerator.Emit(OpCodes.Brtrue, Label);
			StackCountDecrement(1);

#if CODEGEN_TRACE
			Debug.WriteLine("BranchIfTrue: '{0}' -> Stack: {1}", Label, StackCount);
#endif
		}

		public void BranchIfFalse(Label Label)
		{
			ConvTo<bool>();
			ILGenerator.Emit(OpCodes.Brfalse, Label);
			StackCountDecrement(1);

#if CODEGEN_TRACE
			Debug.WriteLine("BranchIfFalse: '{0}' -> Stack: {1}", Label, StackCount);
#endif
		}

		public void Pop(int Count)
		{
			for (int n = 0; n < Count; n++)
			{
				ILGenerator.Emit(OpCodes.Pop);
				StackCountDecrement(1);
			}
		}

		public void ClearStack()
		{
			Pop(StackCount);

#if CODEGEN_TRACE
			Debug.WriteLine("ClearStack -> Stack: {0}", StackCount);
#endif
		}

		public void Call(Delegate Delegate)
		{
			Call(Delegate.Method);
		}

		private void StackCountDecrement(int Count)
		{
			for (int n = 0; n < Count; n++)
			{
				//StackCountDecrement(1);
				_TypeStack.Pop();
			}
		}

		private void StackCountIncrement(Type Type)
		{
			_TypeStack.Push(Type);
			//StackCount++;
		}

		public void Call(MethodInfo MethodInfo)
		{
			int PrevStackCount = StackCount;

			ILGenerator.Emit(OpCodes.Call, MethodInfo);
			int ThisCount = (!MethodInfo.IsStatic) ? 1 : 0;
			int ArgumentCount = MethodInfo.GetParameters().Length;
			int ReturnCount = (MethodInfo.ReturnType != typeof(void)) ? 1 : 0;
			StackCountDecrement(ThisCount);
			StackCountDecrement(ArgumentCount);
			if (MethodInfo.ReturnType != typeof(void))
			{
				StackCountIncrement(MethodInfo.ReturnType);
			}


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

			StackCountIncrement(typeof(TType));

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
			StackCountIncrement(StackTop);

#if CODEGEN_TRACE
			Debug.WriteLine("Dup -> Stack: {0}", StackCount);
#endif
		}

		public void Pop()
		{
			StackCountDecrement(1);

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
			Debug.WriteLine("ConvTo: " + typeof(TType).Name);
#endif
			Type ExpectedType = typeof(TType);
			Type StackType = StackTop;

			//Context.MethodGenerator.Call((Func<bool>)Php54Var.Methods.ToBool);
			if (ExpectedType == StackType)
			{
				return;
			}


			if (StackType == typeof(Php54Var))
			{
				if (ExpectedType == typeof(bool)) { Call((Func<bool>)Php54Var.Methods.ToBool); return; }
				if (ExpectedType == typeof(int)) { Call((Func<int>)Php54Var.Methods.ToInt); return; }
				if (ExpectedType == typeof(string)) { Call((Func<string>)Php54Var.Methods.ToString); return; }

				throw (new NotImplementedException());
			}

			if (ExpectedType == typeof(Php54Var))
			{
				if (StackType == typeof(bool)) { Call((Func<bool, Php54Var>)Php54Var.FromBool); return; }
				if (StackType == typeof(int)) { Call((Func<int, Php54Var>)Php54Var.FromInt); return; }
				if (StackType == typeof(string)) { Call((Func<string, Php54Var>)Php54Var.FromString); return; }

				throw (new NotImplementedException());
			}

			if (ExpectedType == typeof(int))
			{
				ILGenerator.Emit(OpCodes.Conv_I4);
				return;
			}

			if (ExpectedType == typeof(bool))
			{
				ILGenerator.Emit(OpCodes.Conv_I4);
				return;
			}

			throw (new NotImplementedException());
		}

		public void StoreToLocal(LocalBuilder Local)
		{
			ILGenerator.Emit(OpCodes.Stloc, Local);

			StackCountDecrement(1);

#if CODEGEN_TRACE
			Debug.WriteLine("StoreToLocal({0}:{1}) -> Stack: {2}", Local.LocalType.Name, Local.LocalIndex, StackCount);
#endif
		}

		public void LoadLocal(LocalBuilder Local)
		{
			ILGenerator.Emit(OpCodes.Ldloc, Local);

			StackCountIncrement(Local.LocalType);

#if CODEGEN_TRACE
			Debug.WriteLine("LoadLocal({0}:{1}) -> Stack: {2}", Local.LocalType.Name, Local.LocalIndex, StackCount);
#endif
		}
	}
}
