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
			Context.MethodGenerator.Call((Func<Php54Scope>)Php54Scope.Methods.NewScope);
			Context.MethodGenerator.Dup();
			Context.MethodGenerator.Push(ArgumentsCount);
			Context.MethodGenerator.Call((Action<int>)Php54Scope.Methods.SetArgumentCount);

			for (int n = 0; n < ArgumentsCount; n++)
			{
				Context.MethodGenerator.Dup();
				Context.MethodGenerator.Push(n);
				{
					(Parameters.ChildNodes[n].AstNode as Node).Generate(Context);
					Context.MethodGenerator.ConvTo<Php54Var>();
				}
				Context.MethodGenerator.Call((Action<int, Php54Var>)Php54Scope.Methods.SetArgument);
			}

			Context.MethodGenerator.Dup();
			Context.MethodGenerator.Push(FunctionName);
			Context.MethodGenerator.Call((Action<string>)Php54Scope.Methods.CallFunctionByName);

			Context.MethodGenerator.Call((Func<Php54Var>)Php54Scope.Methods.GetReturnValue);

			//throw (new NotImplementedException());
		}
	}
}
