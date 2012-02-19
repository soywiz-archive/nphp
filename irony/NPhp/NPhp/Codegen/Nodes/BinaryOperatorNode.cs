using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using NPhp.Runtime;

namespace NPhp.Codegen.Nodes
{
	public class BinaryOperatorNode : Node
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
				case "+": Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Add); break;
				case "-": Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Sub); break;
				case ".": Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Concat); break;
				case "*": Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Mul); break;
				case "/": Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Div); break;
				case "==": Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.CompareEquals); break;
				case ">": Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.CompareGreaterThan); break;
				case "<": Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.CompareLessThan); break;
				case "!=": Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.CompareNotEquals); break;
				case "&&": Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.LogicalAnd); break;
				default: throw (new NotImplementedException("Not implemented operator '" + Operator + "'"));
			}
			//Context.Operator(Operator);
			//Console.WriteLine("Operator: '{0}'", Operator);
		}
	}
}
