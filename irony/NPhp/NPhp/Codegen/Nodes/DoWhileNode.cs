using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;

namespace NPhp.Codegen.Nodes
{
	public class DoWhileNode : Node
	{
		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			throw(new NotImplementedException("Not implemented do..while syntax"));
		}
	}
}
