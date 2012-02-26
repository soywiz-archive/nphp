using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using NPhp.Runtime;

namespace NPhp.Codegen.Nodes
{
	public class TernaryOperationNode : Node
	{
		public ParseTreeNode ConditionParseNode;
		public ParseTreeNode TrueParseNode;
		public ParseTreeNode FalseParseNode;

		public override void Init(AstContext Context, ParseTreeNode ParseNode)
		{
			ConditionParseNode = ParseNode.ChildNodes[0];
			TrueParseNode = ParseNode.ChildNodes[1];
			FalseParseNode = ParseNode.ChildNodes[2];
		}

		public override void Generate(NodeGenerateContext Context)
		{
			var EndLabel = Context.MethodGenerator.DefineLabel("End");
			var TrueLabel = Context.MethodGenerator.DefineLabel("True");
			var FalseLabel = Context.MethodGenerator.DefineLabel("False");
			var ConditionVar = Context.MethodGenerator.CreateLocal<Php54Var>("ConditionVar");

			// Check condition
			(ConditionParseNode.AstNode as Node).GenerateAs<bool>(Context);
			Context.MethodGenerator.BranchIfFalse(FalseLabel);

			TrueLabel.Mark();
			(TrueParseNode.AstNode as Node).GenerateAs<Php54Var>(Context);
			Context.MethodGenerator.StoreToLocal(ConditionVar);
			Context.MethodGenerator.BranchAlways(EndLabel);
			
			FalseLabel.Mark();
			(FalseParseNode.AstNode as Node).GenerateAs<Php54Var>(Context);
			Context.MethodGenerator.StoreToLocal(ConditionVar);
			Context.MethodGenerator.BranchAlways(EndLabel);

			EndLabel.Mark();

			Context.MethodGenerator.LoadLocal(ConditionVar);
		}
	}
}
