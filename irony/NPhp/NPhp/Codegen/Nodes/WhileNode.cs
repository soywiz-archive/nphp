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
			var ContinueLabel = Context.MethodGenerator.DefineLabel("Loop");
			var BreakLabel = Context.MethodGenerator.DefineLabel("End");

			Context.PushContinueBreakNode(new ContinueBreakNode()
			{
				ContinueLabel = ContinueLabel,
				BreakLabel = BreakLabel,
			}, () =>
			{
				ContinueLabel.Mark();
				{
					(ConditionExpresion.AstNode as Node).Generate(Context);
					Context.MethodGenerator.ConvTo<bool>();
					Context.MethodGenerator.BranchIfFalse(BreakLabel);
				}
				{
					(LoopSentence.AstNode as Node).Generate(Context);
					Context.MethodGenerator.BranchAlways(ContinueLabel);
				}
				BreakLabel.Mark();
			});
		}
	}
}
