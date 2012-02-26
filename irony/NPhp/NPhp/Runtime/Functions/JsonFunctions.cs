using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPhp.Common;
using System.Diagnostics;

namespace NPhp.Runtime.Functions
{
	/// <summary>
	/// 
	/// </summary>
	[Php54NativeLibrary]
	public class JsonFunctions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="Variable"></param>
		/// <returns></returns>
		static public string json_encode(Php54Var Variable)
		{
			switch (Variable.ReferencedType)
			{
				case Php54Var.TypeEnum.Bool: return Variable.BooleanValue ? "true" : "false";
				case Php54Var.TypeEnum.Int: return Variable.IntegerValue.ToString();
				case Php54Var.TypeEnum.Null: return "null";
				case Php54Var.TypeEnum.String: return Php54Utils.StringQuote(Variable.StringValue);
				case Php54Var.TypeEnum.Array:
					//Debug.WriteLine((object)Variable.DynamicValue);
					if (Variable.ArrayValue.PureArray)
					{
						var ElementsArray = new List<string>();
						foreach (var Pair in Variable.ArrayValue.GetEnumerator())
						{
							ElementsArray.Add(json_encode(Pair.Value));
						}
						return "[" + String.Join(",", ElementsArray) + "]";
					}
					else
					{
						var ElementsArray = new List<string>();
						foreach (var Pair in Variable.ArrayValue.GetEnumerator())
						{
							ElementsArray.Add(
								Php54Utils.StringQuote(Pair.Key.ToString()) +
								":" +
								json_encode(Pair.Value)
							);
						}
						return "{" + String.Join(",", ElementsArray) + "}";
					}
				default: throw(new NotImplementedException());
			}
		}
	}
}
