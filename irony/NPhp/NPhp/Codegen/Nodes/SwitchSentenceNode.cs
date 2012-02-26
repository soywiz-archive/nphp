using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using System.Diagnostics;
using NPhp.Runtime;

namespace NPhp.Codegen.Nodes
{
	public class SwitchSentenceNode : Node
	{
		ParseTreeNode Expression;
		ParseTreeNode[] Sentences;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Debug.Assert(parseNode.ChildNodes[0].FindTokenAndGetText() == "switch");
			this.Expression = parseNode.ChildNodes[1];
			this.Sentences = parseNode.ChildNodes[2].ChildNodes[0].ChildNodes.ToArray();
		}

		public override void Generate(NodeGenerateContext Context)
		{
			var EndSwitchLabel = Context.MethodGenerator.DefineLabel("EndSwitch");

			Context.PushContinueBreakNode(new ContinueBreakNode() { BreakLabel = EndSwitchLabel }, () =>
			{
				var ExpressionLocal = Context.MethodGenerator.CreateLocal<Php54Var>("SwitchExpression");
				{
					(Expression.AstNode as Node).GenerateAs<Php54Var>(Context);
				}
				Context.MethodGenerator.StoreToLocal(ExpressionLocal);

				var DefaultLabel = EndSwitchLabel;

				foreach (var Sentence in Sentences)
				{
					var CurrentNode = (Sentence.AstNode as Node).GetNonIgnoredNode();
					//Console.WriteLine(CurrentNode);
					var CaseNode = (CurrentNode as SwitchCaseSentenceNode);
					if (CaseNode != null)
					{
						if (CaseNode.IsDefault)
						{
							DefaultLabel = CaseNode.Label;
						}
						else
						{
							Context.MethodGenerator.LoadLocal(ExpressionLocal);
							(CaseNode.Value.AstNode as Node).GenerateAs<Php54Var>(Context);
							Context.MethodGenerator.Call((Func<Php54Var, Php54Var, bool>)Php54Var.CompareEquals);
							Context.MethodGenerator.BranchIfTrue(CaseNode.Label);
						}
					}
				}

				Context.MethodGenerator.BranchAlways(DefaultLabel);

				foreach (var Sentence in Sentences)
				{
					(Sentence.AstNode as Node).Generate(Context);
				}

				EndSwitchLabel.Mark();
				//base.Generate(Context);
			});
		}
	}
}
