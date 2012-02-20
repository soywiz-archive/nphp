using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using NPhp.Runtime;

namespace NPhp.Codegen
{
	public class Node : IAstNodeInit
	{
		public virtual void Init(AstContext context, ParseTreeNode parseNode)
		{
			//throw new NotImplementedException();
		}

		public Action<Php54Scope> CreateMethod(Php54FunctionScope FunctionScope)
		{
			var Context = new NodeGenerateContext(FunctionScope);
			Generate(Context);
			return Context.MethodGenerator.GenerateMethod();
		}

		public void GenerateAs<TType>(NodeGenerateContext Context)
		{
			Generate(Context);
			Context.MethodGenerator.ConvTo<TType>();
		}

		public virtual void Generate(NodeGenerateContext Context)
		{
			Console.WriteLine("Generate! : {0}", this.GetType());
			throw (new NotImplementedException());
		}
	}
}
