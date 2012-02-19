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
				$n = 9;
				while ($n > 0) {
					echo $n;
					$n = $n - 1;
				}
			", DumpTree: true);

			var Scope = new Php54Scope(Runtime);

			Method(Scope);

			Console.ReadKey();
		}
	}
}
