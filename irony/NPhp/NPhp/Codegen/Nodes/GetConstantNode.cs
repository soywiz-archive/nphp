using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using NPhp.Runtime;

namespace NPhp.Codegen.Nodes
{
	public class GetConstantNode : Node
	{
		string ConstantName;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			ConstantName = parseNode.FindTokenAndGetText();
		}

		public override void PreGenerate(NodeGenerateContext Context)
		{
		}

		public override void Generate(NodeGenerateContext Context)
		{
			Context.MethodGenerator.LoadScope();
			Context.MethodGenerator.Push(ConstantName);
			Context.MethodGenerator.Call((Func<string, Php54Var>)Php54Scope.Methods.GetConstant);
		}
	}
}
