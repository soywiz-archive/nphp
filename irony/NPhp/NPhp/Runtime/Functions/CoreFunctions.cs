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

		static public void print_r(Php54Var Value)
		{
			switch (Value.Type)
			{
				case Php54Var.TypeEnum.Null:
					Console.WriteLine("");
					break;
				case Php54Var.TypeEnum.Int:
					Console.WriteLine("{0}", Value.IntegerValue);
					break;
				case Php54Var.TypeEnum.String:
					Console.WriteLine("{0}", Value.StringValue);
					break;
				case Php54Var.TypeEnum.Array:
					Console.WriteLine("Array");
					Console.WriteLine("(");
					int Index = 0;

					//if (!Value.ArrayValue.PureArray) throw(new NotImplementedException());

					foreach (var Pair in Value.ArrayValue.GetEnumerator())
					{
						Console.Write("    [{0}] => ", Pair.Key);
						print_r(Pair.Value);
						//Console.WriteLine("");
						Index++;
					}
					Console.WriteLine(")");
					break;
				default:
					throw(new NotImplementedException());
			}
		}
	}
}
