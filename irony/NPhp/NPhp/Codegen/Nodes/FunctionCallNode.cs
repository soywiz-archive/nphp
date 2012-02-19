using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using NPhp.Runtime;

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
			int ArgumentsCount = Parameters.ChildNodes.Count;

			Context.MethodGenerator.Comment("Call " + FunctionName);

			Context.MethodGenerator.LoadScope();
			Context.MethodGenerator.Call(typeof(Php54Scope).GetMethod("NewScope"));
			Context.MethodGenerator.Dup();
			Context.MethodGenerator.Push(ArgumentsCount);
			Context.MethodGenerator.Call(typeof(Php54Scope).GetMethod("SetArgumentCount"));

			for (int n = 0; n < ArgumentsCount; n++)
			{
				Context.MethodGenerator.Dup();
				Context.MethodGenerator.Push(n);
				{
					(Parameters.ChildNodes[n].AstNode as Node).Generate(Context);
				}
				Context.MethodGenerator.Call(typeof(Php54Scope).GetMethod("SetArgument"));
			}

			Context.MethodGenerator.Dup();
			Context.MethodGenerator.Push(FunctionName);
			Context.MethodGenerator.Call(typeof(Php54Scope).GetMethod("CallFunctionByName"));

			Context.MethodGenerator.Call(typeof(Php54Scope).GetMethod("GetReturnValue"));

			//throw (new NotImplementedException());
		}
	}
}
