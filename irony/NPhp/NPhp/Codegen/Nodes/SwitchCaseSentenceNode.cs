using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace NPhp.Codegen.Nodes
{
	public class SwitchCaseSentenceNode : Node
	{
		public ParseTreeNode Value;
		public bool IsDefault;
		public SafeLabel Label;

		public override void Init(AstContext Context, ParseTreeNode ParseNode)
		{
			IsDefault = (ParseNode.ChildNodes[0].FindTokenAndGetText() == "default");
			if (!IsDefault)
			{
				Value = ParseNode.ChildNodes[1];
			}
		}

		public override void PreGenerate(NodeGenerateContext Context)
		{
			//Console.WriteLine("Pregenerate case!");
			Label = Context.MethodGenerator.DefineLabel("case");
		}

		public override void Generate(NodeGenerateContext Context)
		{
			Label.Mark();
			//base.Generate(Context);
		}
	}
}
