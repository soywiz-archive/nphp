using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace NPhp.Codegen.Nodes
{
	public class PreOperationNode : Node
	{
		UnaryPreOperationNode UnaryPreOperationNode;
		Node VariableNode;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			UnaryPreOperationNode = (parseNode.ChildNodes[0].AstNode as UnaryPreOperationNode);
			VariableNode = (parseNode.ChildNodes[1].AstNode as Node);
		}

		public override void Generate(NodeGenerateContext Context)
		{
			VariableNode.Generate(Context);
			UnaryPreOperationNode.Generate(Context);
		}
	}
}
