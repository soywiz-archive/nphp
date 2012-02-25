using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPhp.Runtime;
using Irony.Ast;
using Irony.Parsing;

namespace NPhp.Codegen.Nodes
{
	public class IncludeNode : Node
	{
		String IncludeType;
		Node ExpressionNode;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			IncludeType = parseNode.ChildNodes[0].FindTokenAndGetText();
			ExpressionNode = (parseNode.ChildNodes[1].AstNode as Node);
		}

		public override void PreGenerate(NodeGenerateContext Context)
		{
			ExpressionNode.PreGenerate(Context);
		}

		public override void Generate(NodeGenerateContext Context)
		{
			Context.MethodGenerator.LoadScope();
			ExpressionNode.GenerateAs<string>(Context);
			bool IsRequire = false;
			bool IsOnce = false;
			switch (IncludeType)
			{
				case "include": IsRequire = false; IsOnce = false; break;
				case "include_once": IsRequire = false; IsOnce = true; break;
				case "require": IsRequire = true; IsOnce = false; break;
				case "require_once": IsRequire = true; IsOnce = true; break;
			}
			Context.MethodGenerator.Push(IsRequire);
			Context.MethodGenerator.Push(IsOnce);
			Context.MethodGenerator.Call((Action<Php54Scope, string, bool, bool>)Php54Runtime.Include);
		}
	}
}
