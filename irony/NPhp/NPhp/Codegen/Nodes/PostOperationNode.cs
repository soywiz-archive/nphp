using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace NPhp.Codegen.Nodes
{
	public class PostOperationNode : Node
	{
		Node VariableNode;
		UnaryPostOperationNode UnaryPostOperationNode;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			VariableNode = (parseNode.ChildNodes[0].AstNode as Node);
			UnaryPostOperationNode = (parseNode.ChildNodes[1].AstNode as UnaryPostOperationNode);
		}

		public override void PreGenerate(NodeGenerateContext Context)
		{
			VariableNode.PreGenerate(Context);
			UnaryPostOperationNode.PreGenerate(Context);
		}

		public override void Generate(NodeGenerateContext Context)
		{
			VariableNode.Generate(Context);
			UnaryPostOperationNode.Generate(Context);
		}
	}
}
