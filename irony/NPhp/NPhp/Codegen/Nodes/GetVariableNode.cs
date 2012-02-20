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

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			VariableName = parseNode.FindTokenAndGetText();
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
#else
				Context.MethodGenerator.LoadScope();
				Context.MethodGenerator.Push(VariableName);
				Context.MethodGenerator.Call((Func<string, Php54Var>)Php54Scope.Methods.GetVariable);
#endif
		}
	}
}
