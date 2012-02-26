using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Irony.Parsing;
using Irony.Ast;
using NPhp.Runtime;

namespace NPhp.Codegen.Nodes
{
	public class GlobalStaticNode : Node
	{
		String Type;
		String VariableName;
		ParseTreeNode InitializeNode;

		public override void Init(AstContext Context, ParseTreeNode ParseNode)
		{
			Type = ParseNode.ChildNodes[0].FindTokenAndGetText();
			VariableName = ParseNode.ChildNodes[1].FindTokenAndGetText();
			try
			{
				InitializeNode = ParseNode.ChildNodes[1].ChildNodes[0].ChildNodes[2];
			}
			catch
			{
			}
		}

		public override void Generate(NodeGenerateContext Context)
		{
			var StaticVariable = Context.MethodGenerator.CreateLocal<Php54Var>("StaticVariable");

			// local
			Context.MethodGenerator.LoadScope();
			Context.MethodGenerator.Push((string)VariableName);
			Context.MethodGenerator.Call((Func<string, Php54Var>)Php54Scope.Methods.GetVariable);
			Context.MethodGenerator.StoreToLocal(StaticVariable);

			// local
			Context.MethodGenerator.LoadLocal(StaticVariable);

			// global
			Context.MethodGenerator.LoadScope();
			Context.MethodGenerator.Push((string)VariableName);
			switch (Type)
			{
				case "global":
					Context.MethodGenerator.Call((Func<Php54Scope, string, Php54Var>)Php54Runtime.GetGlobal);
					break;
				case "static":
					Context.MethodGenerator.Call((Func<Php54Scope, string, Php54Var>)Php54Runtime.GetStatic);
					break;
				default:
					throw (new NotImplementedException());
			}

			// create reference and assign to local
			Context.MethodGenerator.Call((Func<Php54Var, Php54Var>)Php54Var.CreateRef);
			Context.MethodGenerator.Call((Action<Php54Var, Php54Var>)Php54Var.Assign);

			if (InitializeNode != null)
			{
				var SkipInitialize = Context.MethodGenerator.DefineLabel("SkipInitialize");

				// Check if null
				Context.MethodGenerator.LoadLocal(StaticVariable);
				Context.MethodGenerator.Call((Func<bool>)Php54Var.Methods.IsNull);
				Context.MethodGenerator.BranchIfFalse(SkipInitialize);

				// Initialize.
				{
					Context.MethodGenerator.LoadLocal(StaticVariable);
					(InitializeNode.AstNode as Node).GenerateAs<Php54Var>(Context);
					Context.MethodGenerator.Call((Action<Php54Var, Php54Var>)Php54Var.Assign);
				}

				SkipInitialize.Mark();
			}
		}
	}
}
