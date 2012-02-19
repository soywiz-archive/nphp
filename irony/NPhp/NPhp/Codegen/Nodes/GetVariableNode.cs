using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using NPhp.Runtime;

namespace NPhp.Codegen.Nodes
{
	public class GetVariableNode : Node
	{
		String VariableName;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			VariableName = parseNode.FindTokenAndGetText();
		}

		public override void Generate(NodeGenerateContext Context)
		{
			Context.MethodGenerator.LoadScope();
			Context.MethodGenerator.Push(VariableName);
			Context.MethodGenerator.Call(typeof(Php54Scope).GetMethod("GetVariable"));
			//base.Generate(Context);
		}
	}
}
