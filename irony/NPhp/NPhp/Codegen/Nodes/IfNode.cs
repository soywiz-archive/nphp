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

		public override void Generate(NodeGenerateContext Context)
		{
			var EndLabel = Context.MethodGenerator.DefineLabel();
			var FalseLabel = Context.MethodGenerator.DefineLabel();
			// Check condition
			{
				(ConditionExpresion.AstNode as Node).Generate(Context);
				Context.MethodGenerator.Call((Func<Php54Var, bool>)Php54Var.ToBool);
				Context.MethodGenerator.BranchIfFalse(FalseLabel);
			}
			// True
			{
				(TrueSentence.AstNode as Node).Generate(Context);
				Context.MethodGenerator.BranchAlways(EndLabel);
			}
			// False
			Context.MethodGenerator.MarkLabel(FalseLabel);
			if (FalseSentence != null)
			{
				(FalseSentence.AstNode as Node).Generate(Context);
				Context.MethodGenerator.BranchAlways(EndLabel);
			}
			Context.MethodGenerator.MarkLabel(EndLabel);
			//base.Generate(Context);
		}
	}
}
