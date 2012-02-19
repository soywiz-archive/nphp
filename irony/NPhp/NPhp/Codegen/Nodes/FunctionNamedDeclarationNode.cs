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
	public class FunctionNamedDeclarationNode : Node
	{
		String FunctionName;
		ParseTreeNode ParametersDeclaration;
		Node CodeStatements;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Debug.Assert(parseNode.ChildNodes[0].FindTokenAndGetText() == "function");
			FunctionName = parseNode.ChildNodes[1].FindTokenAndGetText();
			ParametersDeclaration = parseNode.ChildNodes[2];
			CodeStatements = (parseNode.ChildNodes[3].AstNode as Node);

			//throw(new NotImplementedException());
		}

		public override void Generate(NodeGenerateContext Context)
		{
			var Function = Context.GenerateFunction(() =>
			{
				int ArgumentIndex = 0;
				// Load parameters?
				foreach (var ParameterDeclartion in ParametersDeclaration.ChildNodes)
				{
					var ArgumentName = ParameterDeclartion.FindTokenAndGetText();
					{
						//Context.MethodGenerator.Dup();
						Context.MethodGenerator.LoadScope();
						Context.MethodGenerator.Push(ArgumentName);
						Context.MethodGenerator.Push(ArgumentIndex);
						Context.MethodGenerator.Call(typeof(Php54Scope).GetMethod("LoadArgument"));
					}
					ArgumentIndex++;
				}

				//Context.MethodGenerator.Pop();

				// Code
				CodeStatements.Generate(Context);
			});
			Context.FunctionScope.Functions[FunctionName] = Function;
			//throw(new NotImplementedException());
			//base.Generate(Context);
		}
	}
}
