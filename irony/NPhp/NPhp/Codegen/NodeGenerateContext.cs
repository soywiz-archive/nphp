using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPhp.Runtime;

namespace NPhp.Codegen
{
	public class NodeGenerateContext
	{
		public string FunctionName = "";
		public Php54FunctionScope FunctionScope { get; protected set; }
		public MethodGenerator MethodGenerator { get; protected set; }

		public NodeGenerateContext(Php54FunctionScope FunctionScope)
		{
			this.MethodGenerator = new MethodGenerator();
			this.FunctionScope = FunctionScope;
		}

		public Action<Php54Scope> GenerateFunction(Action Action)
		{
			var OldMethodGenerator = MethodGenerator;
			var NewMethodGenerator = new MethodGenerator();
			OldMethodGenerator = MethodGenerator;
			MethodGenerator = NewMethodGenerator;
			try
			{
				Action();
			}
			finally
			{
				MethodGenerator = OldMethodGenerator;
			}
			return NewMethodGenerator.GenerateMethod();
		}
	}
}
