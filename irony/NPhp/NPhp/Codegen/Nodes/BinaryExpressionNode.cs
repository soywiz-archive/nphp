using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;

namespace NPhp.Codegen.Nodes
{
	public class BinaryExpressionNode : Node
	{
		ParseTreeNode Left;
		ParseTreeNode BinaryOperator;
		ParseTreeNode Right;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Left = parseNode.ChildNodes[0];
			BinaryOperator = parseNode.ChildNodes[1];
			Right = parseNode.ChildNodes[2];
		}

		public override void Generate(NodeGenerateContext Context)
		{
			//base.Generate();
			((Node)Left.AstNode).Generate(Context);
			((Node)Right.AstNode).Generate(Context);
			((Node)BinaryOperator.AstNode).Generate(Context);
		}
	}
}
