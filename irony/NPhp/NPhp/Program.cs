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
				eval('echo 1;');
			", DumpTree: true);

			var Scope = new Php54Scope(Runtime);

			var Start = DateTime.UtcNow;
			Method(Scope);
			var End = DateTime.UtcNow;
			Console.WriteLine("\nTime: {0}", End - Start);

			Console.ReadKey();
		}
	}
}
