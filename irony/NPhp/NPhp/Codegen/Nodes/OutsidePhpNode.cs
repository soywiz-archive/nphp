using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPhp.Runtime;
using Irony.Ast;
using Irony.Parsing;
using System.Diagnostics;

namespace NPhp.Codegen.Nodes
{
	public class OutsidePhpNode : Node
	{
		public string Content;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			//throw(new NotImplementedException());
			this.Content = parseNode.FindToken().ValueString;
		}

		public override void PreGenerate(NodeGenerateContext Context)
		{
		}

		public override void Generate(NodeGenerateContext Context)
		{
			Context.MethodGenerator.LoadScope();
			{
				Context.MethodGenerator.Push(Content);
			}
			Context.MethodGenerator.ConvTo<Php54Var>();
			Context.MethodGenerator.Call((Action<Php54Scope, Php54Var>)Php54Runtime.Echo);
		}
	}
}
