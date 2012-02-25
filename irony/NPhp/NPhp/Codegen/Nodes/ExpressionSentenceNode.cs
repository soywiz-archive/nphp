using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace NPhp.Codegen.Nodes
{
	public class ExpressionSentenceNode : IgnoreNode
	{
		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			base.Init(context, parseNode);
		}

		public override void Generate(NodeGenerateContext Context)
		{
			base.Generate(Context);
			Context.MethodGenerator.ClearStack();
		}
	}
}
