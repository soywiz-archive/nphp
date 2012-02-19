using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;

namespace NPhp.Codegen.Nodes
{
	public class UnaryExpressionNode : Node
	{
		ParseTreeNode UnaryOperator;
		ParseTreeNode Right;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			UnaryOperator = parseNode.ChildNodes[0];
			Right = parseNode.ChildNodes[1];
		}

		public override void Generate(NodeGenerateContext Context)
		{
			//base.Generate();
			((Node)Right.AstNode).Generate(Context);
			((Node)UnaryOperator.AstNode).Generate(Context);
		}
	}
}
