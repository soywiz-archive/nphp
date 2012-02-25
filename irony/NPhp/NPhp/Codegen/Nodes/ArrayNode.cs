using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using System.Diagnostics;
using NPhp.Runtime;
using NPhp.LanguageGrammar;

namespace NPhp.Codegen.Nodes
{
	public class ArrayNode : Node
	{
		Php54Grammar Php54Grammar;
		ParseTreeNode[] Childs;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Php54Grammar = (Php54Grammar)context.Language.Grammar;
			IEnumerable<ParseTreeNode> _Childs;
			_Childs = parseNode.ChildNodes;

			if (parseNode.ChildNodes.Count > 0)
			{
				if (parseNode.ChildNodes[0].FindTokenAndGetText() == "array")
				{
					_Childs = _Childs.Skip(1);
				}
			}
			Childs = _Childs.ElementAt(0).ChildNodes.Select(Item => Item).ToArray();
		}

		public override void PreGenerate(NodeGenerateContext Context)
		{
			foreach (var ParseNode in Childs)
			{
				(ParseNode.AstNode as Node).PreGenerate(Context);
			}
		}

		public override void Generate(NodeGenerateContext Context)
		{
			Context.MethodGenerator.Call((Func<Php54Var>)Php54Var.FromNewArray);
			foreach (var ParseNode in Childs)
			{
				if (ParseNode.ChildNodes[0].Term.Name == Php54Grammar.ArrayKeyValueElement.Name)
				{
					//ParseNode.ChildNodes[0].ChildNodes[2]

					Context.MethodGenerator.Dup();
					(ParseNode.ChildNodes[0].ChildNodes[0].AstNode as Node).Generate(Context);
					Context.MethodGenerator.ConvTo<Php54Var>();
					(ParseNode.ChildNodes[0].ChildNodes[1].AstNode as Node).Generate(Context);
					Context.MethodGenerator.ConvTo<Php54Var>();
					Context.MethodGenerator.Call((Action<Php54Var, Php54Var>)Php54Var.Methods.AddKeyValuePair);
					//Context.MethodGenerator.Pop();
				}
				else
				{
					Context.MethodGenerator.Dup();
					(ParseNode.AstNode as Node).Generate(Context);
					Context.MethodGenerator.ConvTo<Php54Var>();
					Context.MethodGenerator.Call((Action<Php54Var>)Php54Var.Methods.AddElement);
					//Context.MethodGenerator.Pop();
				}
			}
			//throw(new Exception());
		}
	}
}
