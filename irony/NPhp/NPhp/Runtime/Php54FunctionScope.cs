using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPhp.Codegen;
using System.Reflection;

namespace NPhp.Runtime
{
	class Php54FunctionLazyNative : IPhp54Function
	{
		Func<IPhp54Function> CodeGenerator;
		IPhp54Function CachedFunction;

		public Php54FunctionLazyNative(Func<IPhp54Function> CodeGenerator)
		{
			this.CodeGenerator = CodeGenerator;
		}

		public void Execute(Php54Scope Scope)
		{
			if (CachedFunction == null)
			{
				CachedFunction = CodeGenerator();
			}
			CachedFunction.Execute(Scope);
		}
	}

	public class Php54FunctionScope
	{
		public Dictionary<string, IPhp54Function> Functions { get; protected set; }

		public Php54FunctionScope()
		{
			Functions = new Dictionary<string, IPhp54Function>();
		}

		static public IPhp54Function CreateNativeWrapper(MethodInfo MethodInfo)
		{
			//var MethodInfo = Delegate.Method;
			var Context = new NodeGenerateContext(null, false);
			var MethodGenerator = Context.MethodGenerator;
			{
				MethodGenerator.LoadScope();

				int ParameterIndex = 0;
				foreach (var Parameter in MethodInfo.GetParameters())
				{
					var ParameterType = Parameter.ParameterType;
					{
						if (ParameterType == typeof(Php54Scope))
						{
							MethodGenerator.LoadScope();
						}
						else
						{
							MethodGenerator.LoadScope();
							MethodGenerator.Push(ParameterIndex);
							//Php54Var.metho
							MethodGenerator.Call((Func<int, Php54Var>)Php54Scope.Methods.GetArgument);
							if (ParameterType == typeof(double))
							{
								MethodGenerator.Push((Parameter.DefaultValue != DBNull.Value) ? (double)Parameter.DefaultValue : 0);
								MethodGenerator.Call((Func<double, double>)Php54Var.Methods.GetDoubleOrDefault);
							}
							else if (ParameterType == typeof(string))
							{
								MethodGenerator.Push((Parameter.DefaultValue != DBNull.Value) ? (string)Parameter.DefaultValue : "");
								MethodGenerator.Call((Func<string, string>)Php54Var.Methods.GetStringOrDefault);
							}
							else if (ParameterType == typeof(int))
							{
								MethodGenerator.Push((Parameter.DefaultValue != DBNull.Value) ? (int)Parameter.DefaultValue : 0);
								MethodGenerator.Call((Func<int, int>)Php54Var.Methods.GetIntegerOrDefault);
							}
							else if (ParameterType == typeof(bool))
							{
								MethodGenerator.Push((Parameter.DefaultValue != DBNull.Value) ? (bool)Parameter.DefaultValue : false);
								MethodGenerator.Call((Func<bool, bool>)Php54Var.Methods.GetBooleanOrDefault);
							}
							else if (ParameterType == typeof(Php54Var))
							{
							}
							else
							{
								throw (new NotImplementedException());
							}
							ParameterIndex++;
						}
					}
				}

				MethodGenerator.Call(MethodInfo);

				if (MethodInfo.ReturnType != typeof(void))
				{
					MethodGenerator.Box(MethodInfo.ReturnType);
					MethodGenerator.Call((Action<object>)Php54Scope.Methods.SetReturnValueObject);
				}
				else
				{
					//MethodGenerator.Pop();
				}
			}
			return Context.MethodGenerator.GenerateMethod();
		}

		private void AddFunction(MethodInfo Method)
		{
			Functions[Method.Name] = new Php54FunctionLazyNative(() => CreateNativeWrapper(Method));
		}

		public void LoadAllNativeFunctions()
		{
			var CurrentAssembly = Assembly.GetAssembly(this.GetType());
			foreach (var Type in CurrentAssembly.GetTypes())
			{
				var Attributes = Type.GetCustomAttributes(typeof(Php54NativeLibraryAttribute), true);
				if (Attributes.Length > 0)
				{
					foreach (var Method in Type.GetMethods())
					{
						if (!Method.IsStatic) continue;
						AddFunction(Method);
						//Console.WriteLine(Method.Name);
					}
				}
			}
			//throw new NotImplementedException();
		}
	}
}
