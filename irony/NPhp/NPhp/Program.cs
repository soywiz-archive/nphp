using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;

namespace NPhp
{
	class Program
	{
		static void Main(string[] args)
		{
			var FunctionScope = new Php54FunctionScope();
			var Runtime = new Php54Runtime(FunctionScope);
			var Method = Runtime.CreateMethodFromCode(@"
				$a = -1;
				$b = -2;
				function add($a, $b) {
					echo $a + $b;
				}
				add(1, 2);

				//for ($n = 0; $n < 100000; $n++) {}
				//$a = 1;
				//$b = &$a;
				//$b = 2;
				//echo $a;
			", DumpTree: true);

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
