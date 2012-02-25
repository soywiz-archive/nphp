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
		private SafeILGenerator SafeILGenerator;
		public int StackCount
		{
			get
			{
				return SafeILGenerator.StackCount;
			}
		}
		public Type StackTop
		{
			get
			{
				return SafeILGenerator.GetCurrentTypeStack().GetLastest();
			}
		}

		public SafeTypeStack CaptureStackInformation(Action Action)
		{
			return SafeILGenerator.CaptureStackInformation(Action);
		}

		public MethodGenerator(bool DoDebug)
		{
			//MethodAttributes.
			DynamicMethod = new DynamicMethod(
				"unknown",
				typeof(void),
				new Type[] { typeof(Php54Scope) }
			);
			SafeILGenerator = new SafeILGenerator(DynamicMethod.GetILGenerator(), CheckTypes: true, DoDebug : DoDebug);
		}

		Dictionary<string, LocalBuilder> Locals = new Dictionary<string,LocalBuilder>();

		public LocalBuilder GetCachedLocal(string Name, out bool Cached)
		{
			LocalBuilder LocalBuilder;
			if (!Locals.TryGetValue(Name, out LocalBuilder))
			{
				Locals[Name] = LocalBuilder = DeclareLocalPhp54Var(Name);
				Cached = false;
			}
			else
			{
				Cached = true;
			}
			return LocalBuilder;
		}

		private LocalBuilder DeclareLocalPhp54Var(string Name)
		{
			return DeclareLocal<Php54Var>(Name);
		}

		private LocalBuilder DeclareLocal<TType>(string Name)
		{
			//return SafeILGenerator.DeclareLocal<TType>(Name);
			return SafeILGenerator.DeclareLocal<TType>(null);
		}

		public IPhp54Function GenerateMethod()
		{
			ClearStack();
			SafeILGenerator.Return();

			SafeILGenerator.CheckAndFinalize();

			return new Php54Function((Action<Php54Scope>)DynamicMethod.CreateDelegate(typeof(Action<Php54Scope>)));
		}

		public void Push(double Value)
		{
			SafeILGenerator.Push(Value);
		}

		public void Push(bool Value)
		{
			SafeILGenerator.Push(Value);
		}

		public void Push(int Value)
		{
			SafeILGenerator.Push(Value);
		}

		public void Push(string Value)
		{
			SafeILGenerator.Push(Value);
		}

		public void BinaryOperation(SafeBinaryOperator Operator)
		{
			SafeILGenerator.BinaryOperation(Operator);
		}

		public void CompareBinary(SafeBinaryComparison Comparison)
		{
			SafeILGenerator.CompareBinary(Comparison);
		}

		public SafeLabel DefineLabel(string Name)
		{
			return SafeILGenerator.DefineLabel(Name);
		}

		public LocalBuilder CreateLocal<TType>(string Name)
		{
			return SafeILGenerator.DeclareLocal<TType>(Name);
		}

		public void BranchIfTrue(SafeLabel Label)
		{
			ConvTo<bool>();
			SafeILGenerator.BranchIfTrue(Label);
		}

		public void BranchIfFalse(SafeLabel Label)
		{
			ConvTo<bool>();
			SafeILGenerator.BranchIfFalse(Label);
		}

		public void Pop(int Count)
		{
			for (int n = 0; n < Count; n++)
			{
				SafeILGenerator.Pop();
			}
		}

		public void ClearStack()
		{
			Pop(StackCount);
		}

		public void Call(Delegate Delegate)
		{
			Call(Delegate.Method);
		}

		public void Call(MethodInfo MethodInfo)
		{
			SafeILGenerator.Call(MethodInfo);
		}

		public void BranchAlways(SafeLabel Label)
		{
			SafeILGenerator.BranchAlways(Label);
		}

		public void LoadScope()
		{
			LoadArgument<Php54Scope>(0);
		}

		private void LoadArgument<TType>(int ArgumentIndex)
		{
			SafeILGenerator.LoadArgument<TType>(ArgumentIndex);
		}

		public void Box<TType>()
		{
			Box(typeof(TType));
		}

		public void Box(Type Type)
		{
			SafeILGenerator.Box(Type);
		}

		public void Unbox<TType>()
		{
			SafeILGenerator.Unbox(typeof(TType));
		}

		public void Comment(string Comment)
		{
			SafeILGenerator.Comment(Comment);
		}

		public void Dup()
		{
			SafeILGenerator.Duplicate();
		}

		public void Pop()
		{
			SafeILGenerator.Pop();
		}

		public void Return()
		{
			SafeILGenerator.Return();
		}

		public void ConvTo<TType>()
		{
			Type ExpectedType = typeof(TType);
			Type StackType = StackTop;

			//Context.MethodGenerator.Call((Func<bool>)Php54Var.Methods.ToBool);
			if (ExpectedType == StackType)
			{
				return;
			}

			if (StackType == null) throw(new NullReferenceException("Argument on the stack is null!"));

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
				//if (StackType == typeof(object)) { SafeILGenerator.CastClass<Php54Var>(); return; }

				throw (new NotImplementedException());
			}

			if (ExpectedType == typeof(int))
			{
				SafeILGenerator.ConvertTo<int>();
				return;
			}

			if (ExpectedType == typeof(bool))
			{
				SafeILGenerator.ConvertTo<bool>();
				return;
			}

			throw (new NotImplementedException());
		}

		public void StoreToLocal(LocalBuilder Local)
		{
			SafeILGenerator.StoreLocal(Local);
		}

		public void LoadLocal(LocalBuilder Local)
		{
			SafeILGenerator.LoadLocal(Local);
		}
	}
}
