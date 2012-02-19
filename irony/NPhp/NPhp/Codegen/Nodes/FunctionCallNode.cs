using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace NPhp.Codegen.Nodes
{
	public class FunctionCallNode : Node
	{
		string FunctionName;
		ParseTreeNode Parameters;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			FunctionName = parseNode.ChildNodes[0].FindTokenAndGetText();
			Parameters = parseNode.ChildNodes[1];
		}

		public override void Generate(NodeGenerateContext Context)
		{
			Context.MethodGenerator.LoadScope();
			throw (new NotImplementedException());
		}
	}
}
