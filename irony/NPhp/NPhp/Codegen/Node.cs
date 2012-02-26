using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using NPhp.Runtime;
using System.IO;

namespace NPhp.Codegen
{
	abstract public class Node : IAstNodeInit
	{
		abstract public void Init(AstContext Context, ParseTreeNode ParseNode);

		public IPhp54Function CreateMethod(string FileName, ParseTreeNode ParseNode, Php54FunctionScope FunctionScope, bool DoDebug)
		{
			var Context = new NodeGenerateContext(FunctionScope, DoDebug);
			Context.CurrentFile = FileName;
			Context.CurrentDirectory = Directory.GetCurrentDirectory();
			try
			{
				var FileInfo = new FileInfo(FileName);
				if (FileInfo.Exists)
				{
					Context.CurrentFile = FileInfo.FullName;
					Context.CurrentDirectory = FileInfo.Directory.FullName;
				}
			}
			catch
			{
			}
			PreGenerate(ParseNode, Context);
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

		public void PreGenerate(ParseTreeNode ParseNode, NodeGenerateContext Context)
		{
			foreach (var Child in ParseNode.ChildNodes)
			{
				var Node = (Child.AstNode as Node);
				if (Node != null)
				{
					Node.PreGenerate(Child, Context);
					Node.PreGenerateNode(Context);
				}
			}
		}
		virtual public void PreGenerateNode(NodeGenerateContext Context)
		{
		}
		abstract public void Generate(NodeGenerateContext Context);
	}
}
