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
	public class AssignmentNode : Node
	{
		ParseTreeNode LeftValueNode;
		ParseTreeNode ValueNode;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			LeftValueNode = parseNode.ChildNodes[0];
			Debug.Assert(parseNode.ChildNodes[1].FindTokenAndGetText() == "=");
			ValueNode = parseNode.ChildNodes[2];
		}

		public override void Generate(NodeGenerateContext Context)
		{
			(LeftValueNode.AstNode as Node).Generate(Context);
			
			(ValueNode.AstNode as Node).Generate(Context);
			Context.MethodGenerator.ConvTo<Php54Var>();

			Context.MethodGenerator.Call((Action<Php54Var, Php54Var>)Php54Var.Assign);
		}
	}
}
