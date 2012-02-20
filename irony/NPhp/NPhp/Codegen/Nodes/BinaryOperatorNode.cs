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
			throw(new NotImplementedException("Must call Generate(Left, Right, Context)"));
		}

		public void Generate(Node Left, Node Right, NodeGenerateContext Context)
		{
			//base.Generate();
			Left.Generate(Context);
			//Context.MethodGenerator.StackTop
			Context.MethodGenerator.ConvTo<Php54Var>();

			Right.Generate(Context);

			switch (Operator)
			{
				case "+": Context.MethodGenerator.ConvTo<Php54Var>(); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Add); break;
				case "-": Context.MethodGenerator.ConvTo<Php54Var>(); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Sub); break;
				case ".": Context.MethodGenerator.ConvTo<Php54Var>(); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Concat); break;
				case "*": Context.MethodGenerator.ConvTo<Php54Var>(); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Mul); break;
				case "/": Context.MethodGenerator.ConvTo<Php54Var>(); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Div); break;

				case "&": Context.MethodGenerator.ConvTo<Php54Var>(); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.BitAnd); break;
				case "|": Context.MethodGenerator.ConvTo<Php54Var>(); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.BitOr); break;

				case "==": Context.MethodGenerator.ConvTo<Php54Var>(); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, bool>)Php54Var.CompareEquals); break;
				case ">": Context.MethodGenerator.ConvTo<Php54Var>(); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, bool>)Php54Var.CompareGreaterThan); break;
				case "<":
					if (Context.MethodGenerator.StackTop == typeof(int))
					{
						Context.MethodGenerator.Call((Func<Php54Var, int, bool>)Php54Var.CompareLessThan);
					}
					else
					{
						Context.MethodGenerator.ConvTo<Php54Var>();
						Context.MethodGenerator.Call((Func<Php54Var, Php54Var, bool>)Php54Var.CompareLessThan);
					}
				break;
				case "!=": Context.MethodGenerator.ConvTo<Php54Var>(); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, bool>)Php54Var.CompareNotEquals); break;
				case "&&": Context.MethodGenerator.ConvTo<Php54Var>(); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, bool>)Php54Var.LogicalAnd); break;
				default: throw (new NotImplementedException("Not implemented operator '" + Operator + "'"));
			}
			//Context.Operator(Operator);
			//Console.WriteLine("Operator: '{0}'", Operator);
		}
	}
}
