using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using NPhp.Runtime;
using NPhp.Runtime.Functions;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using System.IO;

namespace NPhp
{
	class Program
	{
		static void Main(string[] args)
		{
			//FunctionScope.Functions["substr"] = Php54FunctionScope.CreateNativeWrapper(((Func<string, int, int, string>)StringFunctions.substr).Method);

			if (args.Length > 0)
			{
				try
				{
					var Runtime = new Php54Runtime(InteractiveErrors: false);
					Runtime.FunctionScope.LoadAllNativeFunctions();

					var Function = Runtime.CreateMethodFromPhpFile(File.ReadAllText(args[0]), File: args[0], DumpTree: false, DoDebug: false);

					Function.Execute(Runtime.GlobalScope);
					Runtime.Shutdown();
				}
				catch (Exception Exception)
				{
					Console.Error.WriteLine(Exception);
				}
			}
			else
			{
				var Runtime = new Php54Runtime(InteractiveErrors: true);
				Runtime.FunctionScope.LoadAllNativeFunctions();

				var Function = Runtime.CreateMethodFromPhpCode(@"
					$f = fopen('test.txt', 'wb');
					fclose($f);
				".Trim(), DumpTree: true, DoDebug: false);

				var Start = DateTime.UtcNow;
				{
					Function.Execute(Runtime.GlobalScope);
					Runtime.Shutdown();
				}
				var End = DateTime.UtcNow;
				//FunctionScope.Functions["add"](new Php54Scope(Runtime));
				//Console.WriteLine(FunctionScope.Functions["add"]);
				Console.WriteLine("\nTime: {0}", End - Start);

				//Test();

				Console.ReadKey();
			}
		}

		/*
		static void Test()
		{
			Action<int> Del = (Value) =>
			{
				Console.WriteLine(Value + 1000);
			};
			Del(1);
		}
		*/
	}
}
