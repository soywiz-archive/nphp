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
			var Runtime = new Php54Runtime();
			var Method = Runtime.CreateMethodFromCode(@"
				$a = 3 + 2;
				echo $a;
			", DumpTree: true);

			var Scope = new Php54Scope(Runtime);

			Method(Scope);

			Console.ReadKey();
		}
	}
}
