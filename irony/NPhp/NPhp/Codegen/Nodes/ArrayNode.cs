using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using System.Diagnostics;
using NPhp.Runtime;

namespace NPhp.Codegen.Nodes
{
	public class ArrayNode : Node
	{
		Node[] Childs;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			IEnumerable<ParseTreeNode> _Childs;
			_Childs = parseNode.ChildNodes;

			if (parseNode.ChildNodes.Count > 0)
			{
				if (parseNode.ChildNodes[0].FindTokenAndGetText() == "array")
				{
					_Childs = _Childs.Skip(1);
				}
			}
			Childs = _Childs.ElementAt(0).ChildNodes.Select(Item => Item.AstNode as Node).ToArray();
		}

		public override void Generate(NodeGenerateContext Context)
		{
			Context.MethodGenerator.Call((Func<Php54Var>)Php54Var.FromNewArray);
			foreach (var Node in Childs)
			{
				Context.MethodGenerator.Dup();
				Node.Generate(Context);
				Context.MethodGenerator.ConvTo<Php54Var>();
				Context.MethodGenerator.Call((Action<Php54Var>)Php54Var.Methods.AddElement);
				//Context.MethodGenerator.Pop();
			}
		}
	}
}
