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
			var LoopLabel = Context.MethodGenerator.DefineLabel();
			var EndLabel = Context.MethodGenerator.DefineLabel();

			Context.MethodGenerator.Comment("InitialSentence");
			(InitialSentence.AstNode as Node).Generate(Context);
			Context.MethodGenerator.ClearStack();

			Context.MethodGenerator.MarkLabel(LoopLabel);
			{
				Context.MethodGenerator.Comment("ConditionExpresion");
				(ConditionExpresion.AstNode as Node).Generate(Context);
				Context.MethodGenerator.Call((Func<bool>)Php54Var.Methods.ToBool);
				Context.MethodGenerator.BranchIfFalse(EndLabel);
			}
			{
				Context.MethodGenerator.Comment("LoopSentence");
				(LoopSentence.AstNode as Node).Generate(Context);

				Context.MethodGenerator.Comment("PostSentence");
				(PostSentence.AstNode as Node).Generate(Context);
				Context.MethodGenerator.ClearStack();
				Context.MethodGenerator.BranchAlways(LoopLabel);
			}
			Context.MethodGenerator.MarkLabel(EndLabel);
		}
	}

}
