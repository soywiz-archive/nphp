using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPhp.Runtime.Functions
{
	[Php54NativeLibrary]
	public class CoreFunctions
	{
		/// <summary>
		/// The current PHP "major" version as an integer (e.g., int(5) from version "5.2.7-extra"). Available since PHP 5.2.7.
		/// </summary>
		public const int PHP_MAJOR_VERSION = 5;

		/// <summary>
		/// The current PHP "minor" version as an integer (e.g., int(2) from version "5.2.7-extra"). Available since PHP 5.2.7.
		/// </summary>
		public const int PHP_MINOR_VERSION = 4;

		/// <summary>
		/// The current PHP "release" version as an integer (e.g., int(7) from version "5.2.7-extra"). Available since PHP 5.2.7.
		/// </summary>
		public const int PHP_RELEASE_VERSION = 0;

		/// <summary>
		/// The current PHP "extra" version as a string (e.g., '-extra' from version "5.2.7-extra"). Often used by distribution vendors to indicate a package version. Available since PHP 5.2.7.
		/// </summary>
		public const string PHP_EXTRA_VERSION = "-NPHP";

		/// <summary>
		/// The current PHP version as a string in "major.minor.release[extra]" notation.
		/// </summary>
		static public string PHP_VERSION
		{
			get
			{
				return PHP_MAJOR_VERSION + "." + PHP_MINOR_VERSION + "." + PHP_RELEASE_VERSION + PHP_EXTRA_VERSION;
			}
		}

		/// <summary>
		/// The current PHP version as an integer, useful for version comparisons (e.g., int(50207) from version "5.2.7-extra"). Available since PHP 5.2.7.
		/// </summary>
		static public int PHP_VERSION_ID
		{
			get
			{
				return PHP_MAJOR_VERSION * 10000 + PHP_MINOR_VERSION * 100 + PHP_RELEASE_VERSION;
			}
		}
		
		static public void define(Php54Scope Scope, string Name, Php54Var Value)
		//static public void define(string Name, int Value)
		{
			var LeftValue = Scope.Runtime.ConstantScope.GetVariable(Name);
			Php54Var.Assign(LeftValue, Value);
		}

		static public string phpversion()
		{
			return "5.4.0-NPHP";
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

		static public void register_shutdown_function(Php54Scope Scope, Php54Var ShutdownFunction)
		{
			Scope.Runtime.ShutdownFunction = ShutdownFunction;
		}

		static public void var_export(Php54Var Value)
		{
			switch (Value.ReferencedType)
			{
				case Php54Var.TypeEnum.Null: Console.WriteLine("NULL"); break;
				case Php54Var.TypeEnum.Int: Console.WriteLine(Value.IntegerValue); break;
				default: throw (new NotImplementedException());
			}
		}

		static public void var_dump(Php54Var Value)
		{
			switch (Value.ReferencedType)
			{
				case Php54Var.TypeEnum.Null: Console.WriteLine("NULL"); break;
				case Php54Var.TypeEnum.Int: Console.WriteLine("int({0})", Value.IntegerValue); break;
				case Php54Var.TypeEnum.Bool: Console.WriteLine("bool({0})", Value.BooleanValue ? "true" : "false"); break;
				default: throw (new NotImplementedException());
			}
		}

		static public void print_r(Php54Var Value, bool ReturnString = false, int IndentLevel = 0)
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
						print_r(Pair.Value, ReturnString, IndentLevel + 2);
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
