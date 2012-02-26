using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace NPhp.Codegen.Nodes
{
	public class IgnoreNode : Node
	{
		ParseTreeNode parseNode;
		//ParseTreeNode Child;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			this.parseNode = parseNode;
			//Child = parseNode.ChildNodes[0];
		}

		public override Node GetNonIgnoredNode()
		{
			if (parseNode.ChildNodes.Count == 1) return (parseNode.ChildNodes[0].AstNode as Node).GetNonIgnoredNode();
			return this;
		}

		public override void Generate(NodeGenerateContext Context)
		{
			foreach (var Node in parseNode.ChildNodes)
			{
				var AstNode = (Node)Node.AstNode;
				if (AstNode != null) AstNode.Generate(Context);
			}
		}
	}
}
