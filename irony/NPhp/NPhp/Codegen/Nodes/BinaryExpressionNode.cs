using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using NPhp.Runtime;

namespace NPhp.Codegen.Nodes
{
	public class BinaryExpressionNode : Node
	{
		Node Left;
		BinaryOperatorNode BinaryOperator;
		Node Right;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Left = parseNode.ChildNodes[0].AstNode as Node;
			BinaryOperator = (parseNode.ChildNodes[1].AstNode as BinaryOperatorNode);
			Right = parseNode.ChildNodes[2].AstNode as Node;
		}

		public override void PreGenerate(NodeGenerateContext Context)
		{
			Left.PreGenerate(Context);
			Right.PreGenerate(Context);
			BinaryOperator.PreGenerate(Context);
		}

		public override void Generate(NodeGenerateContext Context)
		{
			BinaryOperator.Generate(Left, Right, Context);
		}
	}
}
