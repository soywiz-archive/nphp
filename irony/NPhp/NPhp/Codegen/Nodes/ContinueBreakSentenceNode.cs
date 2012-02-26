using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using System.Diagnostics;

namespace NPhp.Codegen.Nodes
{
	public class ContinueBreakSentenceNode : Node
	{
		public string Type;
		public int JumpCount;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Type = parseNode.ChildNodes[0].FindTokenAndGetText();
			try
			{
				var JumpCountString = (parseNode.ChildNodes[1] != null) ? parseNode.ChildNodes[1].FindTokenAndGetText() : "";
				JumpCount = (JumpCountString == "") ? 1 : int.Parse(JumpCountString);
			}
			catch
			{
				JumpCount = 1;
			}
		}

		public override void Generate(NodeGenerateContext Context)
		{
			var ContinueBreak = Context.GetContinueBreakNodeAt(JumpCount);
			switch (Type)
			{
				case "continue": Context.MethodGenerator.BranchAlways(ContinueBreak.ContinueLabel); break;
				case "break": Context.MethodGenerator.BranchAlways(ContinueBreak.BreakLabel); break;
				default: throw(new InvalidOperationException());
			}
			
		}
	}
}
