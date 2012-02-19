using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPhp.Runtime;

namespace NPhp.Codegen.Nodes
{
	public class EvalNode : IgnoreNode
	{
		public override void Generate(NodeGenerateContext Context)
		{
			//base.Generate(Context);
			//Php54Scope
			Context.MethodGenerator.LoadScope();
			base.Generate(Context);
			Context.MethodGenerator.Call((Action<Php54Scope, Php54Var>)Php54Runtime.Eval);
		}
	}
}
