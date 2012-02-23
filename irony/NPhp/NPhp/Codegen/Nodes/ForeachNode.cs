using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using System.Diagnostics;
using NPhp.Runtime;
using System.Collections;

namespace NPhp.Codegen.Nodes
{
	public class ForeachNode : Node
	{
		ParseTreeNode IterableExpressionParseNode;
		ParseTreeNode VariableGetParseNode;
		ParseTreeNode IterationCodeNode;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Debug.Assert(parseNode.ChildNodes[0].FindTokenAndGetText() == "foreach");
			IterableExpressionParseNode = parseNode.ChildNodes[1];
			VariableGetParseNode = parseNode.ChildNodes[2];
			IterationCodeNode = parseNode.ChildNodes[3];
		}

		public override void Generate(NodeGenerateContext Context)
		{
			var IteratorLocal = Context.MethodGenerator.CreateLocal<IEnumerator<Php54Var>>("IteratorLocal");
			var StartLoopLabel = Context.MethodGenerator.DefineLabel("StartLoopLabel");
			var EndLoopLabel = Context.MethodGenerator.DefineLabel("EndtLoopLabel");

			(IterableExpressionParseNode.AstNode as Node).Generate(Context);
			Context.MethodGenerator.ConvTo<Php54Var>();
			Context.MethodGenerator.Call((Func<IEnumerator<Php54Var>>)Php54Var.Methods.GetValuesIterator);
			Context.MethodGenerator.StoreToLocal(IteratorLocal);

			StartLoopLabel.Mark();
			{
				// while (iterator.MoveNext())
				Context.MethodGenerator.LoadLocal(IteratorLocal);
				Context.MethodGenerator.Call((Func<IEnumerator<Php54Var>, bool>)Php54Var.IteratorMoveNext);
				Context.MethodGenerator.BranchIfFalse(EndLoopLabel);

				// as $var
				(VariableGetParseNode.AstNode as Node).Generate(Context);
				Context.MethodGenerator.ConvTo<Php54Var>();

				// iterator.Current
				Context.MethodGenerator.LoadLocal(IteratorLocal);
				Context.MethodGenerator.Call((Func<IEnumerator<Php54Var>, Php54Var>)Php54Var.IteratorGetCurrent);
				Context.MethodGenerator.ConvTo<Php54Var>();

				// $var = iterator.Current
				Context.MethodGenerator.Call((Action<Php54Var, Php54Var>)Php54Var.Assign);
				
				// <iteration code>
				(IterationCodeNode.AstNode as Node).Generate(Context);

				Context.MethodGenerator.BranchAlways(StartLoopLabel);
			}
			EndLoopLabel.Mark();

			Context.MethodGenerator.ClearStack();
		}
	}
}
