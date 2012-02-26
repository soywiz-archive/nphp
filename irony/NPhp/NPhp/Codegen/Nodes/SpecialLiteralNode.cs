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
			this.Type = parseNode.FindTokenAndGetText().ToUpperInvariant();
		}

		public override void Generate(NodeGenerateContext Context)
		{
			switch (Type)
			{
				//case "true": Context.MethodGenerator.Call((Func<Php54Var>)Php54Var.FromTrue); break;
				case "TRUE": Context.MethodGenerator.Push(true); break;
				case "FALSE": Context.MethodGenerator.Push(false); break;
				case "NULL": Context.MethodGenerator.Call((Func<Php54Var>)Php54Var.FromNull); break;
				case "__DIR__": Context.MethodGenerator.Push(Context.CurrentDirectory); break;
				case "__FILE__": Context.MethodGenerator.Push(Context.CurrentFile); break;
				case "__LINE__": Context.MethodGenerator.Push(parseNode.Span.Location.Line + 1); break;
				case "__FUNCTION__": Context.MethodGenerator.Push(Context.FunctionName); break;
				default: throw (new NotImplementedException("Can't handle special id '" + Type + "'"));
			}
		}
	}
}
