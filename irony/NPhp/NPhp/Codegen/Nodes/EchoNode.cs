using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPhp.Runtime;

namespace NPhp.Codegen.Nodes
{
	public class EchoNode : IgnoreNode
	{
		public override void Generate(NodeGenerateContext Context)
		{
			Context.MethodGenerator.LoadScope();
			base.Generate(Context);
			Context.MethodGenerator.Call((Action<Php54Scope, Php54Var>)Php54Runtime.Echo);
		}
	}
}
