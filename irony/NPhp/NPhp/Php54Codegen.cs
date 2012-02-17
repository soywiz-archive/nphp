using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using System.Reflection.Emit;
using System.Reflection;

namespace NPhp
{
	public class NodeGenerateContext
	{
		public NodeGenerateContext()
		{
			DynamicMethod = new DynamicMethod("unknown", typeof(void), new Type[] { });
			ILGenerator = DynamicMethod.GetILGenerator();
		}

		public int StackCount;

		public void Push(int Value)
		{
			//Console.WriteLine("Push");
			ILGenerator.Emit(OpCodes.Ldc_I4, Value);
			StackCount++;
		}

		public void Operator(string Operator)
		{
			//Console.WriteLine("Operator");
			switch (Operator)
			{
				case "+": ILGenerator.Emit(OpCodes.Add); break;
				case "-": ILGenerator.Emit(OpCodes.Sub); break;
				case "*": ILGenerator.Emit(OpCodes.Mul); break;
				case "/": ILGenerator.Emit(OpCodes.Div); break;
				default: throw (new NotImplementedException());
			}
			StackCount--;
		}

		public Label DefineLabel()
		{
			return ILGenerator.DefineLabel();
		}

		public void MarkLabel(Label Label)
		{
			ILGenerator.MarkLabel(Label);
		}

		public void BranchIfTrue(Label Label)
		{
			ILGenerator.Emit(OpCodes.Brtrue, Label);
			StackCount--;
		}

		public void BranchIfFalse(Label Label)
		{
			ILGenerator.Emit(OpCodes.Brfalse, Label);
			StackCount--;
		}

		public Action GenerateMethod()
		{
			while (StackCount-- > 0)
			{
				ILGenerator.Emit(OpCodes.Pop);
				//Console.WriteLine("Pop");
			}
			ILGenerator.Emit(OpCodes.Ret);

			return (Action)DynamicMethod.CreateDelegate(typeof(Action));
		}

		private DynamicMethod DynamicMethod;
		private ILGenerator ILGenerator;

		public void Call(MethodInfo MethodInfo)
		{
			//Console.WriteLine("Call");
			ILGenerator.Emit(OpCodes.Call, MethodInfo);
			foreach (var Param in MethodInfo.GetCurrentMethod().GetParameters()) {
				StackCount--;
			}
			if (MethodInfo.ReturnType != typeof(void)) {
				StackCount++;
			}
		}
	}

	public class Node : IAstNodeInit
	{
		public virtual void Init(AstContext context, ParseTreeNode parseNode)
		{
			//throw new NotImplementedException();
		}

		public Action CreateMethod()
		{
			var Context = new NodeGenerateContext();
			Generate(Context);
			return Context.GenerateMethod();
		}

		public virtual void Generate(NodeGenerateContext Context)
		{
			Console.WriteLine("Generate! : {0}", this.GetType());
		}
	}

	public class BinaryExpression : Node
	{
		ParseTreeNode Left;
		ParseTreeNode Operator;
		ParseTreeNode Right;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Left = parseNode.ChildNodes[0];
			Operator = parseNode.ChildNodes[1];
			Right = parseNode.ChildNodes[2];
		}

		public override void Generate(NodeGenerateContext Context)
		{
			//base.Generate();
			((Node)Left.AstNode).Generate(Context);
			((Node)Right.AstNode).Generate(Context);
			((Node)Operator.AstNode).Generate(Context);
		}
	}

	/*
	public class Literal : Node
	{
	}
	*/

	public class NumberNode : Node
	{
		int Value;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{

			Value = int.Parse(parseNode.FindTokenAndGetText());
		}

		public override void Generate(NodeGenerateContext Context)
		{
			Context.Push(Value);
			//Console.WriteLine("Value: '{0}'", Value);
		}
	}

	public class OperatorNode : Node
	{
		String Operator;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Operator = parseNode.FindTokenAndGetText();
		}

		public override void Generate(NodeGenerateContext Context)
		{
			Context.Operator(Operator);
			//Console.WriteLine("Operator: '{0}'", Operator);
		}
	}

	public class IgnoreNode : Node
	{
		ParseTreeNode parseNode;
		//ParseTreeNode Child;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			this.parseNode = parseNode;
			//Child = parseNode.ChildNodes[0];
		}

		public override void Generate(NodeGenerateContext Context)
		{
			foreach (var Node in parseNode.ChildNodes)
			{
				var AstNode = (Node)Node.AstNode;
				if (AstNode != null) AstNode.Generate(Context);
			}
		}
	}

	public class EchoNode : IgnoreNode
	{
		public override void Generate(NodeGenerateContext Context)
		{
			base.Generate(Context);
			Context.Call(((Action<int>)Php54Runtime.Echo).Method);
		}
	}

	public class IfNode : Node
	{
		ParseTreeNode ConditionExpresion;
		ParseTreeNode TrueSentence;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			ConditionExpresion = parseNode.ChildNodes[1];
			TrueSentence = parseNode.ChildNodes[2];
		}

		public override void Generate(NodeGenerateContext Context)
		{
			var SkipIfLabel = Context.DefineLabel();
			(ConditionExpresion.AstNode as Node).Generate(Context);
			Context.BranchIfFalse(SkipIfLabel);
			(TrueSentence.AstNode as Node).Generate(Context);
			Context.MarkLabel(SkipIfLabel);
			//base.Generate(Context);
		}
	}

	/*
	public class SentenceNode : Node
	{
	}
	*/
}
