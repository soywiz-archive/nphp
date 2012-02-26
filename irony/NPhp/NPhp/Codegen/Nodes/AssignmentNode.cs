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
		String Operator;
		ParseTreeNode ValueNode;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			LeftValueNode = parseNode.ChildNodes[0];
			Operator = parseNode.ChildNodes[1].FindTokenAndGetText();
			ValueNode = parseNode.ChildNodes[2];
		}

		public override void Generate(NodeGenerateContext Context)
		{
			(LeftValueNode.AstNode as Node).Generate(Context);
			(ValueNode.AstNode as Node).Generate(Context);
			Context.MethodGenerator.ConvTo<Php54Var>();

			switch (Operator) {
				case "=": Context.MethodGenerator.Call((Action<Php54Var, Php54Var>)Php54Var.Assign); break;
				case "+=": Context.MethodGenerator.Call((Action<Php54Var, Php54Var>)Php54Var.AssignAdd); break;
				case "-=": Context.MethodGenerator.Call((Action<Php54Var, Php54Var>)Php54Var.AssignSub); break;
				default:
					throw(new NotImplementedException("Not Implemented Assignment Operator: " + Operator));
			}
		}
	}
}
