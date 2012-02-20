using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using System.Diagnostics;
using Irony.Ast;
using NPhp.Runtime;

namespace NPhp.Codegen.Nodes
{
	public class WhileNode : Node
	{
		ParseTreeNode ConditionExpresion;
		ParseTreeNode LoopSentence;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Debug.Assert("while" == parseNode.ChildNodes[0].FindTokenAndGetText());
			ConditionExpresion = parseNode.ChildNodes[1];
			LoopSentence = parseNode.ChildNodes[2];
		}

		public override void Generate(NodeGenerateContext Context)
		{
			var LoopLabel = Context.MethodGenerator.DefineLabel("Loop");
			var EndLabel = Context.MethodGenerator.DefineLabel("End");

			LoopLabel.Mark();
			{
				(ConditionExpresion.AstNode as Node).Generate(Context);
				Context.MethodGenerator.ConvTo<bool>();
				Context.MethodGenerator.BranchIfFalse(EndLabel);
			}
			{
				(LoopSentence.AstNode as Node).Generate(Context);
				Context.MethodGenerator.BranchAlways(LoopLabel);
			}
			EndLabel.Mark();
		}
	}
}
