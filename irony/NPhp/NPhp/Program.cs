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
				echo(1+(2+3)*2)/11;
				//echo 5;
				//echo 2;
				if (0) {
					echo 0;
				}
				if (1) {
					echo 1;
				}
			", DumpTree: true);
			Method();
			Console.ReadKey();
		}
	}
}
