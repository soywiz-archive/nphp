using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPhp.Runtime.Functions
{
	[Php54NativeLibrary]
	public class CoreFunctions
	{
		static public void define(Php54Scope Scope, string Name, Php54Var Value)
		//static public void define(string Name, int Value)
		{
			var LeftValue = Scope.Php54Runtime.ConstantScope.GetVariable(Name);
			Php54Var.Assign(LeftValue, Value);
		}
	}
}
