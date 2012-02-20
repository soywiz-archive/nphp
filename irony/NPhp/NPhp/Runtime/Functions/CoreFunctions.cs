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

		static public string gettype(Php54Var Value)
		{
			switch (Value.Type)
			{
				case Php54Var.TypeEnum.Bool: return "boolean";
				case Php54Var.TypeEnum.Int: return "integer";
				case Php54Var.TypeEnum.Double: return "double";
				case Php54Var.TypeEnum.String: return "string";
				case Php54Var.TypeEnum.Null: return "NULL";
			}
			throw(new NotImplementedException());
		}
	}
}
