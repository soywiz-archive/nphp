using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using System.Diagnostics;
using NPhp.Runtime;

namespace NPhp.Codegen.Nodes
{
	public class IfNode : Node
	{
		ParseTreeNode ConditionExpresion;
		ParseTreeNode TrueSentence;
		ParseTreeNode FalseSentence;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Debug.Assert("if" == parseNode.ChildNodes[0].FindTokenAndGetText());
			ConditionExpresion = parseNode.ChildNodes[1];
			TrueSentence = parseNode.ChildNodes[2];
			if (parseNode.ChildNodes.Count > 3)
			{
				Debug.Assert("else" == parseNode.ChildNodes[3].FindTokenAndGetText());
				FalseSentence = parseNode.ChildNodes[4];
			}
		}

		public override void PreGenerate(NodeGenerateContext Context)
		{
			(ConditionExpresion.AstNode as Node).PreGenerate(Context);
			(TrueSentence.AstNode as Node).PreGenerate(Context);
			if (FalseSentence != null) (FalseSentence.AstNode as Node).PreGenerate(Context);
		}

		public override void Generate(NodeGenerateContext Context)
		{
			var EndLabel = Context.MethodGenerator.DefineLabel("End");
			var FalseLabel = Context.MethodGenerator.DefineLabel("False");
			// Check condition
			{
				(ConditionExpresion.AstNode as Node).Generate(Context);
				//Context.MethodGenerator.ConvTo<bool>();
				Context.MethodGenerator.BranchIfFalse(FalseLabel);
			}
			// True
			{
				(TrueSentence.AstNode as Node).Generate(Context);
				Context.MethodGenerator.BranchAlways(EndLabel);
			}
			// False
			FalseLabel.Mark();
			if (FalseSentence != null)
			{
				(FalseSentence.AstNode as Node).Generate(Context);
				Context.MethodGenerator.BranchAlways(EndLabel);
			}
			EndLabel.Mark();
			//base.Generate(Context);
		}
	}
}
