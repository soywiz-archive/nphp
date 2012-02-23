#define CACHE_VARIABLES

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
		ParseTreeNode RanksParseNode;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			VariableName = parseNode.ChildNodes[0].FindTokenAndGetText();
			RanksParseNode = parseNode.ChildNodes[1];
		}

		public override void Generate(NodeGenerateContext Context)
		{
#if CACHE_VARIABLES
			bool Cached;
			var Local = Context.MethodGenerator.GetCachedLocal(VariableName, out Cached);

			if (!Cached)
			{
				//Console.WriteLine("Cache local variable");
				Context.MethodGenerator.LoadScope();
				Context.MethodGenerator.Push(VariableName);
				Context.MethodGenerator.Call((Func<string, Php54Var>)Php54Scope.Methods.GetVariable);

				Context.MethodGenerator.StoreToLocal(Local);
			}

			Context.MethodGenerator.LoadLocal(Local);

			foreach (var RankTree in RanksParseNode.ChildNodes)
			{
				(RankTree.AstNode as Node).Generate(Context);
				Context.MethodGenerator.ConvTo<Php54Var>();
				Context.MethodGenerator.Call((Func<Php54Var, Php54Var>)Php54Var.Methods.Access);
			}
#else
				Context.MethodGenerator.LoadScope();
				Context.MethodGenerator.Push(VariableName);
				Context.MethodGenerator.Call((Func<string, Php54Var>)Php54Scope.Methods.GetVariable);
#endif
		}
	}
}
