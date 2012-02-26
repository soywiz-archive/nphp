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
	public class ReturnNode : Node
	{
		ParseTreeNode ReturnExpression;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Debug.Assert(parseNode.ChildNodes[0].FindTokenAndGetText() == "return");
			ReturnExpression = parseNode.ChildNodes[1];
		}

		public override void Generate(NodeGenerateContext Context)
		{
			Context.MethodGenerator.LoadScope();
			(ReturnExpression.AstNode as Node).GenerateAs<Php54Var>(Context);
			Context.MethodGenerator.Call((Action<Php54Var>)Php54Scope.Methods.SetReturnValue);
			Context.MethodGenerator.Return();
		}
	}
}
