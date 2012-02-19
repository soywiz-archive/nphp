using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using System.Diagnostics;

namespace NPhp.Codegen.Nodes
{
	public class NamedFunctionDeclarationNode : Node
	{
		String FunctionName;
		Node ParametersDeclaration;
		Node Code;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Debug.Assert(parseNode.ChildNodes[0].FindTokenAndGetText() == "function");
			FunctionName = parseNode.ChildNodes[1].FindTokenAndGetText();
			ParametersDeclaration = (parseNode.ChildNodes[2].AstNode as Node);
			Code = (parseNode.ChildNodes[3].AstNode as Node);

			//throw(new NotImplementedException());
		}

		public override void Generate(NodeGenerateContext Context)
		{
			var Function = Context.GenerateFunction(() =>
			{
				// Load parameters?
				// Code
				Code.Generate(Context);
			});
			Context.FunctionScope.Functions[FunctionName] = Function;
			//throw(new NotImplementedException());
			//base.Generate(Context);
		}
	}
}
