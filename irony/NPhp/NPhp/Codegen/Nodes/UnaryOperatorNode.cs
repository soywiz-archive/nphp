using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using NPhp.Runtime;

namespace NPhp.Codegen.Nodes
{
	public class UnaryOperatorNode : Node
	{
		String Operator;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Operator = parseNode.FindTokenAndGetText();
		}

		public override void Generate(NodeGenerateContext Context)
		{
			switch (Operator)
			{
				case "@":
					//Context.MethodGenerator.Call((Func<Php54Var, Php54Var>)Php54Var.UnarySilence);
				break;
				case "+": Context.MethodGenerator.Call((Func<Php54Var, Php54Var>)Php54Var.UnaryAdd); break;
				case "-": Context.MethodGenerator.Call((Func<Php54Var, Php54Var>)Php54Var.UnarySub); break;
				case "~": Context.MethodGenerator.Call((Func<Php54Var, Php54Var>)Php54Var.UnaryBitNot); break;
				case "!": Context.MethodGenerator.Call((Func<Php54Var, Php54Var>)Php54Var.UnaryLogicNot); break;
				case "&": Context.MethodGenerator.Call((Func<Php54Var, Php54Var>)Php54Var.CreateRef); break;
				case "int": Context.MethodGenerator.Call((Func<int>)Php54Var.Methods.CastToInt); break;
				case "bool": Context.MethodGenerator.Call((Func<bool>)Php54Var.Methods.CastToBool); break;
				case "float": Context.MethodGenerator.Call((Func<double>)Php54Var.Methods.CastToFloat); break;
				case "string": Context.MethodGenerator.Call((Func<string>)Php54Var.Methods.CastToString); break;
				//case "array": Context.MethodGenerator.Call((Func<int>)Php54Var.Methods.CastToArray); break;
				//case "object": Context.MethodGenerator.Call((Func<int>)Php54Var.Methods.CastToObject); break;
				//case "unset": Context.MethodGenerator.Call((Func<object>)Php54Var.Methods.CastToUnset); break;
				default: throw (new NotImplementedException("Not implemented operator '" + Operator + "'"));
			}
			//Context.Operator(Operator);
			//Console.WriteLine("Operator: '{0}'", Operator);
		}
	}
}
