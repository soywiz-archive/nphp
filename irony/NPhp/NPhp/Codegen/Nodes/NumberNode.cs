using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using NPhp.Runtime;

namespace NPhp.Codegen.Nodes
{
	public class NumberNode : Node
	{
		int Value;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{

			Value = int.Parse(parseNode.FindTokenAndGetText());
		}

		public override void Generate(NodeGenerateContext Context)
		{
			Context.MethodGenerator.Push(Value);
			//Context.MethodGenerator.Call((Func<int, Php54Var>)Php54Var.FromInt);

			//Console.WriteLine("Value: '{0}'", Value);
		}
	}
}
