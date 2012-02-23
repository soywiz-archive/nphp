using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
			switch (Variable.Type)
			{
				case Php54Var.TypeEnum.Bool: return Variable.BoolValue ? "true" : "false";
				case Php54Var.TypeEnum.Int: return Variable.IntegerValue.ToString();
				case Php54Var.TypeEnum.Null: return "null";
				case Php54Var.TypeEnum.String: return String.Format("\"{0}\"", Variable.StringValue);
				case Php54Var.TypeEnum.Array:
					{
						var ElementsArray = new List<string>();
						foreach (var Element in Variable.ArrayValue.Elements)
						{
							ElementsArray.Add(json_encode(Element));
						}
						return "[" + String.Join(",", ElementsArray) + "]";
					}
				default: throw(new NotImplementedException());
			}
		}
	}
}
