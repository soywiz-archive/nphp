using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPhp.Codegen;
using System.Reflection;

namespace NPhp.Runtime
{
	public class Php54FunctionScope
	{
		public Dictionary<string, Action<Php54Scope>> Functions { get; protected set; }

		static public Action<Php54Scope> CreateNativeWrapper(MethodInfo MethodInfo)
		{
			//var MethodInfo = Delegate.Method;
			var Context = new NodeGenerateContext(null);
			var MethodGenerator = Context.MethodGenerator;
			{
				MethodGenerator.LoadScope();

				int ParameterIndex = 0;
				foreach (var Parameter in MethodInfo.GetParameters())
				{
					var ParameterType = Parameter.ParameterType;
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
						else
						{
							throw (new NotImplementedException());
						}
					}
					ParameterIndex++;
				}

				MethodGenerator.Call(MethodInfo);

				if (MethodInfo.ReturnType != typeof(void))
				{
					MethodGenerator.Box(MethodInfo.ReturnType);
					MethodGenerator.Call((Action<object>)Php54Scope.Methods.SetReturnValueObject);
				}
				else
				{
					MethodGenerator.Pop();
				}
			}
			return Context.MethodGenerator.GenerateMethod();
		}

		public Php54FunctionScope()
		{
			Functions = new Dictionary<string, Action<Php54Scope>>();
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
						Functions[Method.Name] = CreateNativeWrapper(Method);
						//Console.WriteLine(Method.Name);
					}
				}
			}
			//throw new NotImplementedException();
		}
	}
}
