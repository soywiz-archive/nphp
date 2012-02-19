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
	public class StringNode : Node
	{
		String Value;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Value = Unquote(parseNode.FindTokenAndGetText());
		}

		static string Unquote(string Unquote)
		{
			Debug.Assert(Unquote[0] == Unquote[Unquote.Length - 1]);
			if (Unquote[0] == '\'')
			{
			}
			// @TODO fix unquotes.
			return Unquote.Substring(1, Unquote.Length - 2);
		}

		public override void Generate(NodeGenerateContext Context)
		{
			Context.MethodGenerator.Push(Value);
			Context.MethodGenerator.Call((Func<string, Php54Var>)Php54Var.FromString);
			//Console.WriteLine("Value: '{0}'", Value);
		}
	}
}
