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
		ParseTreeNode parseNode;
		string Type;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			this.parseNode = parseNode;
			this.Type = parseNode.FindTokenAndGetText();
		}

		public override void Generate(NodeGenerateContext Context)
		{
			switch (Type)
			{
				//case "true": Context.MethodGenerator.Call((Func<Php54Var>)Php54Var.FromTrue); break;
				case "true": Context.MethodGenerator.Push(true); break;
				case "false": Context.MethodGenerator.Push(false); break;
				case "null": Context.MethodGenerator.Call((Func<Php54Var>)Php54Var.FromNull); break;
				case "__LINE__": Context.MethodGenerator.Push(parseNode.Span.Location.Line + 1); break;
				case "__FUNCTION__": Context.MethodGenerator.Push(Context.FunctionName); break;
				default: throw (new NotImplementedException("Can't handle special id '" + Type + "'"));
			}
		}
	}
}
