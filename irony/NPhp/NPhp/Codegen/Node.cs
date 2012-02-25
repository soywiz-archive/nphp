using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using NPhp.Runtime;

namespace NPhp.Codegen
{
	abstract public class Node : IAstNodeInit
	{
		public virtual void Init(AstContext Context, ParseTreeNode ParseNode)
		{
			//throw new NotImplementedException();
		}

		public IPhp54Function CreateMethod(Php54FunctionScope FunctionScope, bool DoDebug)
		{
			var Context = new NodeGenerateContext(FunctionScope, DoDebug);
			PreGenerate(Context);
			Generate(Context);
			return Context.MethodGenerator.GenerateMethod();
		}

		public void GenerateAs<TType>(NodeGenerateContext Context)
		{
			Generate(Context);
			Context.MethodGenerator.ConvTo<TType>();
		}

		virtual public Node GetNonIgnoredNode()
		{
			return this;
		}

		abstract public void PreGenerate(NodeGenerateContext Context);
		abstract public void Generate(NodeGenerateContext Context);
	}
}
