using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using System.Diagnostics;
using NPhp.Runtime;
using System.Collections;
using NPhp.LanguageGrammar;

namespace NPhp.Codegen.Nodes
{
	public class ForeachNode : Node
	{
		ParseTreeNode IterableExpressionParseNode;
		ParseTreeNode VariableKeyGetParseNode;
		ParseTreeNode VariableValueGetParseNode;
		ParseTreeNode IterationCodeNode;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Debug.Assert(parseNode.ChildNodes[0].FindTokenAndGetText() == "foreach");
			IterableExpressionParseNode = parseNode.ChildNodes[1];
			if (parseNode.ChildNodes[3].Term.Name == (context.Language.Grammar as Php54Grammar).GetVariable.Name)
			{
				VariableKeyGetParseNode = parseNode.ChildNodes[2];
				VariableValueGetParseNode = parseNode.ChildNodes[3];
				IterationCodeNode = parseNode.ChildNodes[4];
			}
			else
			{
				VariableValueGetParseNode = parseNode.ChildNodes[2];
				IterationCodeNode = parseNode.ChildNodes[3];
			}
		}

		public override void Generate(NodeGenerateContext Context)
		{
			var IteratorLocal = Context.MethodGenerator.CreateLocal<IEnumerator<KeyValuePair<Php54Var, Php54Var>>>("IteratorLocal");
			var StartLoopLabel = Context.MethodGenerator.DefineLabel("StartLoopLabel");
			var EndLoopLabel = Context.MethodGenerator.DefineLabel("EndtLoopLabel");

			Context.PushContinueBreakNode(new ContinueBreakNode()
			{
				ContinueLabel = StartLoopLabel,
				BreakLabel = EndLoopLabel,
			}, () =>
			{

				(IterableExpressionParseNode.AstNode as Node).Generate(Context);
				Context.MethodGenerator.ConvTo<Php54Var>();
				Context.MethodGenerator.Call((Func<IEnumerator<KeyValuePair<Php54Var, Php54Var>>>)Php54Var.Methods.GetArrayIterator);
				Context.MethodGenerator.StoreToLocal(IteratorLocal);

				StartLoopLabel.Mark();
				{
					// while (iterator.MoveNext())
					Context.MethodGenerator.LoadLocal(IteratorLocal);
					Context.MethodGenerator.Call((Func<IEnumerator<KeyValuePair<Php54Var, Php54Var>>, bool>)Php54Var.IteratorMoveNext);
					Context.MethodGenerator.BranchIfFalse(EndLoopLabel);

					if (VariableKeyGetParseNode != null)
					{
						// as $key
						(VariableKeyGetParseNode.AstNode as Node).Generate(Context);
						Context.MethodGenerator.ConvTo<Php54Var>();
						// iterator.Current
						Context.MethodGenerator.LoadLocal(IteratorLocal);
						Context.MethodGenerator.Call((Func<IEnumerator<KeyValuePair<Php54Var, Php54Var>>, Php54Var>)Php54Var.IteratorGetCurrentKey);
						Context.MethodGenerator.ConvTo<Php54Var>();
						// $var = iterator.Current
						Context.MethodGenerator.Call((Action<Php54Var, Php54Var>)Php54Var.Assign);
					}

					// $value
					(VariableValueGetParseNode.AstNode as Node).Generate(Context);
					Context.MethodGenerator.ConvTo<Php54Var>();
					// iterator.Current
					Context.MethodGenerator.LoadLocal(IteratorLocal);
					Context.MethodGenerator.Call((Func<IEnumerator<KeyValuePair<Php54Var, Php54Var>>, Php54Var>)Php54Var.IteratorGetCurrentValue);
					Context.MethodGenerator.ConvTo<Php54Var>();
					// $var = iterator.Current
					Context.MethodGenerator.Call((Action<Php54Var, Php54Var>)Php54Var.Assign);

					// <iteration code>
					(IterationCodeNode.AstNode as Node).Generate(Context);

					Context.MethodGenerator.BranchAlways(StartLoopLabel);
				}
				EndLoopLabel.Mark();

				Context.MethodGenerator.ClearStack();
			});
		}
	}
}
