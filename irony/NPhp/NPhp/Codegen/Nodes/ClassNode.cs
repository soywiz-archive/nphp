using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using System.Diagnostics;

namespace NPhp.Codegen.Nodes
{
	public class ClassNode : Node
	{
		String ClassName;
		ParseTreeNode ClassDeclarations;

		public override void Init(AstContext Context, ParseTreeNode ParseNode)
		{
			Debug.Assert(ParseNode.ChildNodes[0].FindTokenAndGetText() == "class");
			ClassName = ParseNode.ChildNodes[1].FindTokenAndGetText();
			ClassDeclarations = ParseNode.ChildNodes[2];
		}

		public override void Generate(NodeGenerateContext Context)
		{
			throw new NotImplementedException();
		}
	}
}
