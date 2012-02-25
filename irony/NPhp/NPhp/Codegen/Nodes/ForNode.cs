using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Irony.Parsing;
using Irony.Ast;
using NPhp.Runtime;

namespace NPhp.Codegen.Nodes
{
	public class ForNode : Node
	{
		ParseTreeNode InitialSentence;
		ParseTreeNode ConditionExpresion;
		ParseTreeNode PostSentence;
		ParseTreeNode LoopSentence;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Debug.Assert("for" == parseNode.ChildNodes[0].FindTokenAndGetText());
			InitialSentence = parseNode.ChildNodes[1];
			ConditionExpresion = parseNode.ChildNodes[2];
			PostSentence = parseNode.ChildNodes[3];
			LoopSentence = parseNode.ChildNodes[4];
		}

		public override void Generate(NodeGenerateContext Context)
		{
			var LoopLabel = Context.MethodGenerator.DefineLabel("Loop");
			var ContinueLabel = Context.MethodGenerator.DefineLabel("Continue");
			var BreakLabel = Context.MethodGenerator.DefineLabel("Break");
			Context.PushContinueBreakNode(new ContinueBreakNode()
			{
				ContinueLabel = ContinueLabel,
				BreakLabel = BreakLabel,
			}, () =>
			{
				Context.MethodGenerator.Comment("InitialSentence");
				(InitialSentence.AstNode as Node).Generate(Context);
				Context.MethodGenerator.ClearStack();

				LoopLabel.Mark();
				{
					Context.MethodGenerator.Comment("ConditionExpresion");
					(ConditionExpresion.AstNode as Node).Generate(Context);
					//Context.MethodGenerator.ConvTo<bool>();
					Context.MethodGenerator.BranchIfFalse(BreakLabel);
				}
				{
					Context.MethodGenerator.Comment("LoopSentence");
					(LoopSentence.AstNode as Node).Generate(Context);

					ContinueLabel.Mark();
					Context.MethodGenerator.Comment("PostSentence");
					(PostSentence.AstNode as Node).Generate(Context);
					Context.MethodGenerator.ClearStack();
					Context.MethodGenerator.BranchAlways(LoopLabel);
				}
				BreakLabel.Mark();
			});
		}
	}

}
