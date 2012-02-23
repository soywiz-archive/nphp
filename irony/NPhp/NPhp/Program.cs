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

namespace NPhp
{
	class Program
	{
		static void Main(string[] args)
		{
			var FunctionScope = new Php54FunctionScope();
			FunctionScope.LoadAllNativeFunctions();
			//FunctionScope.Functions["substr"] = Php54FunctionScope.CreateNativeWrapper(((Func<string, int, int, string>)StringFunctions.substr).Method);

			Debug.WriteLine("=========================");

			var Runtime = new Php54Runtime(FunctionScope);
			var Method = Runtime.CreateMethodFromCode(@"
				$array = [1, 2, 3, 4];
				$array[0] = 3;
				$array[2] = 0;
				$str = 'hello';
				echo $str[1];
				echo json_encode($array);
			", DumpTree: true, DoDebug: false);

			var Scope = new Php54Scope(Runtime);

			var Start = DateTime.UtcNow;
			Method(Scope);
			var End = DateTime.UtcNow;
			//FunctionScope.Functions["add"](new Php54Scope(Runtime));
			//Console.WriteLine(FunctionScope.Functions["add"]);
			Console.WriteLine("\nTime: {0}", End - Start);

			//Test();

			Console.ReadKey();
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
