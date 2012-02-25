using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;

namespace NPhp.Codegen.Nodes
{
	public class ContinueBreakSentenceNode : Node
	{
		public string Type;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Type = parseNode.ChildNodes[0].FindTokenAndGetText();
		}

		public override void Generate(NodeGenerateContext Context)
		{
			var ContinueBreak = Context.ContinueBreakNodeList[Context.ContinueBreakNodeList.Count - 1];
			switch (Type)
			{
				case "continue": Context.MethodGenerator.BranchAlways(ContinueBreak.ContinueLabel); break;
				case "break": Context.MethodGenerator.BranchAlways(ContinueBreak.BreakLabel); break;
				default: throw(new InvalidOperationException());
			}
			
		}
	}
}
