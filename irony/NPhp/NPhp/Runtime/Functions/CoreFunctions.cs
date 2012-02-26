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
			switch (Value.ReferencedType)
			{
				case Php54Var.TypeEnum.Bool: return "boolean";
				case Php54Var.TypeEnum.Int: return "integer";
				case Php54Var.TypeEnum.Double: return "double";
				case Php54Var.TypeEnum.String: return "string";
				case Php54Var.TypeEnum.Null: return "NULL";
			}
			throw(new NotImplementedException());
		}

		static public void error_reporting(int Value)
		{
		}

		static public void print_r(Php54Var Value, int IndentLevel = 0)
		{
			switch (Value.ReferencedType)
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

//					IndentLevel++;

					Console.WriteLine(new String(' ', 4 * (IndentLevel + 0)) + "(");
					int Index = 0;

					//if (!Value.ArrayValue.PureArray) throw(new NotImplementedException());

					foreach (var Pair in Value.ArrayValue.GetEnumerator())
					{
						Console.Write(new String(' ', 4 * (IndentLevel + 1)) + "[{0}] => ", Pair.Key);
						print_r(Pair.Value, IndentLevel + 2);
						//Console.WriteLine("");
						Index++;
					}
					Console.WriteLine(new String(' ', 4 * (IndentLevel + 0)) + ")");
					Console.WriteLine();
					break;
				default:
					throw(new NotImplementedException());
			}
		}
	}
}
