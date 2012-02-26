using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using NPhp.Runtime;

namespace NPhp.Codegen.Nodes
{
	public class UnaryPreOperationNode : Node
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
				case "++": Context.MethodGenerator.Push(+1); Context.MethodGenerator.Call((Func<Php54Var, int, Php54Var>)Php54Var.UnaryPreInc); break;
				case "--": Context.MethodGenerator.Push(-1); Context.MethodGenerator.Call((Func<Php54Var, int, Php54Var>)Php54Var.UnaryPreInc); break;
				default: throw (new NotImplementedException("Not implemented operator '" + Operator + "'"));
			}
		}
	}
}
