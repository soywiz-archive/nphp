using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using NPhp.Runtime;

namespace NPhp.Codegen.Nodes
{
	public class SpecialLiteralNode : Node
	{
		string Type;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Type = parseNode.FindTokenAndGetText();
		}

		public override void Generate(NodeGenerateContext Context)
		{
			switch (Type)
			{
				case "true": Context.MethodGenerator.Call((Func<Php54Var>)Php54Var.FromTrue); break;
				case "false": Context.MethodGenerator.Call((Func<Php54Var>)Php54Var.FromFalse); break;
				case "null": Context.MethodGenerator.Call((Func<Php54Var>)Php54Var.FromNull); break;
				default: throw (new NotImplementedException());
			}
		}
	}
}
