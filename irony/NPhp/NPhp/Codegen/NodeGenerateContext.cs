using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPhp.Runtime;

namespace NPhp.Codegen
{
	public class ContinueBreakNode
	{
		public SafeLabel ContinueLabel;
		public SafeLabel BreakLabel;
	}

	public class NodeGenerateContext
	{
		public string CurrentFile = "";
		public string CurrentDirectory = "";
		public string FunctionName = "";
		public Php54FunctionScope FunctionScope { get; protected set; }
		public MethodGenerator MethodGenerator { get; protected set; }

		public bool DoDebug { get; private set; }

		private List<ContinueBreakNode> ContinueBreakNodeList = new List<ContinueBreakNode>();
		//private Php54Runtime Runtime;

		public ContinueBreakNode GetContinueBreakNodeAt(int Index)
		{
			return ContinueBreakNodeList[ContinueBreakNodeList.Count - Index];
		}

		public void PushContinueBreakNode(ContinueBreakNode ContinueBreakNode, Action Action)
		{
			ContinueBreakNodeList.Add(ContinueBreakNode);
			try
			{
				Action();
			}
			finally
			{
				ContinueBreakNodeList.RemoveAt(ContinueBreakNodeList.Count - 1);
			}
		}

		public NodeGenerateContext(Php54FunctionScope FunctionScope, bool DoDebug)
		{
			//this.Runtime = Runtime;
			this.DoDebug = DoDebug;
			this.MethodGenerator = new MethodGenerator(DoDebug);
			this.FunctionScope = FunctionScope;
		}

		public IPhp54Function GenerateFunction(Action Action, bool DoDebug)
		{
			var OldMethodGenerator = MethodGenerator;
			var NewMethodGenerator = new MethodGenerator(DoDebug);
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
