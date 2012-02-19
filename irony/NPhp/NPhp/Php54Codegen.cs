using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using System.Reflection.Emit;
using System.Reflection;
using System.Diagnostics;

namespace NPhp
{
	public class NodeGenerateContext
	{
		public NodeGenerateContext()
		{
			DynamicMethod = new DynamicMethod(
				"unknown",
				typeof(void),
				new Type[] { typeof(Php54Scope) }
			);
			ILGenerator = DynamicMethod.GetILGenerator();
		}

		public int StackCount;

		public void Push(int Value)
		{
			switch (Value)
			{
				case -1: ILGenerator.Emit(OpCodes.Ldc_I4_M1); break;
				case 0: ILGenerator.Emit(OpCodes.Ldc_I4_0); break;
				case 1: ILGenerator.Emit(OpCodes.Ldc_I4_1); break;
				case 2: ILGenerator.Emit(OpCodes.Ldc_I4_2); break;
				case 3: ILGenerator.Emit(OpCodes.Ldc_I4_3); break;
				case 4: ILGenerator.Emit(OpCodes.Ldc_I4_4); break;
				case 5: ILGenerator.Emit(OpCodes.Ldc_I4_5); break;
				case 6: ILGenerator.Emit(OpCodes.Ldc_I4_6); break;
				case 7: ILGenerator.Emit(OpCodes.Ldc_I4_7); break;
				case 8: ILGenerator.Emit(OpCodes.Ldc_I4_8); break;
				default:
					if ((int)(sbyte)Value == (int)Value)
					{
						ILGenerator.Emit(OpCodes.Ldc_I4_S, (sbyte)Value);
					}
					else
					{
						ILGenerator.Emit(OpCodes.Ldc_I4, Value);
					}
				break;
			}
			
			StackCount++;
		}

		public void Push(string Value)
		{
			ILGenerator.Emit(OpCodes.Ldstr, Value);

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

		public Action<Php54Scope> GenerateMethod()
		{
			while (StackCount-- > 0)
			{
				ILGenerator.Emit(OpCodes.Pop);
				//Console.WriteLine("Pop");
			}
			ILGenerator.Emit(OpCodes.Ret);

			return (Action<Php54Scope>)DynamicMethod.CreateDelegate(typeof(Action<Php54Scope>));
		}

		private DynamicMethod DynamicMethod;
		private ILGenerator ILGenerator;

		public void Call(Delegate Delegate)
		{
			Call(Delegate.Method);
		}

		public void Call(MethodInfo MethodInfo)
		{
			ILGenerator.Emit(OpCodes.Call, MethodInfo);
			int ArgumentCount = MethodInfo.GetParameters().Length;
			int ReturnCount = (MethodInfo.ReturnType != typeof(void)) ? 1 : 0;
			StackCount -= ArgumentCount;
			StackCount += ReturnCount;
			//Console.WriteLine("Call: {0} : {1}, {2} -> {3}", MethodInfo.Name, ArgumentCount, ReturnCount, StackCount);
		}

		public void BranchAlways(Label Label)
		{
			ILGenerator.Emit(OpCodes.Br, Label);
		}

		internal void LoadArgument(int ArgumentIndex)
		{
			switch (ArgumentIndex)
			{
				case 0: ILGenerator.Emit(OpCodes.Ldarg_0); break;
				case 1: ILGenerator.Emit(OpCodes.Ldarg_1); break;
				case 2: ILGenerator.Emit(OpCodes.Ldarg_2); break;
				case 3: ILGenerator.Emit(OpCodes.Ldarg_3); break;
				default: ILGenerator.Emit(OpCodes.Ldarg, ArgumentIndex); break;
			}
			
			//throw new NotImplementedException();
		}

		public void Box<TType>()
		{
			ILGenerator.Emit(OpCodes.Box, typeof(TType));
		}

		public void Unbox<TType>()
		{
			ILGenerator.Emit(OpCodes.Unbox, typeof(TType));
		}
	}

	public class Node : IAstNodeInit
	{
		public virtual void Init(AstContext context, ParseTreeNode parseNode)
		{
			//throw new NotImplementedException();
		}

		public Action<Php54Scope> CreateMethod()
		{
			var Context = new NodeGenerateContext();
			Generate(Context);
			return Context.GenerateMethod();
		}

		public virtual void Generate(NodeGenerateContext Context)
		{
			Console.WriteLine("Generate! : {0}", this.GetType());
			throw(new NotImplementedException());
		}
	}

	public class UnaryExpression : Node
	{
		ParseTreeNode UnaryOperator;
		ParseTreeNode Right;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			UnaryOperator = parseNode.ChildNodes[0];
			Right = parseNode.ChildNodes[1];
		}

		public override void Generate(NodeGenerateContext Context)
		{
			//base.Generate();
			((Node)Right.AstNode).Generate(Context);
			((Node)UnaryOperator.AstNode).Generate(Context);
		}
	}

	public class BinaryExpression : Node
	{
		ParseTreeNode Left;
		ParseTreeNode BinaryOperator;
		ParseTreeNode Right;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Left = parseNode.ChildNodes[0];
			BinaryOperator = parseNode.ChildNodes[1];
			Right = parseNode.ChildNodes[2];
		}

		public override void Generate(NodeGenerateContext Context)
		{
			//base.Generate();
			((Node)Left.AstNode).Generate(Context);
			((Node)Right.AstNode).Generate(Context);
			((Node)BinaryOperator.AstNode).Generate(Context);
		}
	}

	/*
	public class Literal : Node
	{
	}
	*/

	public class StringNode : Node
	{
		String Value;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Value = Unquote(parseNode.FindTokenAndGetText());
		}

		static string Unquote(string Unquote)
		{
			Debug.Assert(Unquote[0] == Unquote[Unquote.Length - 1]);
			if (Unquote[0] == '\'')
			{
			}
			// @TODO fix unquotes.
			return Unquote.Substring(1, Unquote.Length - 2);
		}

		public override void Generate(NodeGenerateContext Context)
		{
			Context.Push(Value);
			Context.Call((Func<string, Php54Var>)Php54Var.FromString);
			//Console.WriteLine("Value: '{0}'", Value);
		}
	}

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
			Context.Call((Func<int, Php54Var>)Php54Var.FromInt);
			//Console.WriteLine("Value: '{0}'", Value);
		}
	}

	public class VariableNameNode : IgnoreNode
	{
	}

	public class GetVariableNode : Node
	{
		String VariableName;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			VariableName = parseNode.FindTokenAndGetText();
		}

		public override void Generate(NodeGenerateContext Context)
		{
			Context.LoadArgument(0);
			Context.Push(VariableName);
			Context.Call(typeof(Php54Scope).GetMethod("GetVariable"));
			//base.Generate(Context);
		}
	}

	public class UnaryOperatorNode : Node
	{
		String Operator;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Operator = parseNode.FindTokenAndGetText();
		}

		public override void Generate(NodeGenerateContext Context)
		{
			switch (Operator)
			{
				case "+": Context.Call((Func<Php54Var, Php54Var>)Php54Var.UnaryAdd); break;
				case "-": Context.Call((Func<Php54Var, Php54Var>)Php54Var.UnarySub); break;
				default: throw (new NotImplementedException("Not implemented operator '" + Operator + "'"));
			}
			//Context.Operator(Operator);
			//Console.WriteLine("Operator: '{0}'", Operator);
		}
	}

	public class BinaryOperatorNode : Node
	{
		String Operator;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Operator = parseNode.FindTokenAndGetText();
		}

		public override void Generate(NodeGenerateContext Context)
		{
			switch (Operator)
			{
				case "+": Context.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Add); break;
				case "-": Context.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Sub); break;
				case "*": Context.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Mul); break;
				case "/": Context.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Div); break;
				case "==": Context.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.CompareEquals); break;
				case "!=": Context.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.CompareNotEquals); break;
				case "&&": Context.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.LogicalAnd); break;
				default: throw(new NotImplementedException("Not implemented operator '" + Operator + "'"));
			}
			//Context.Operator(Operator);
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

	public class AssignmentNode : Node
	{
		ParseTreeNode LeftValueNode;
		ParseTreeNode ValueNode;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			LeftValueNode = parseNode.ChildNodes[0];
			Debug.Assert(parseNode.ChildNodes[1].FindTokenAndGetText() == "=");
			ValueNode = parseNode.ChildNodes[2];
		}

		public override void Generate(NodeGenerateContext Context)
		{
			(LeftValueNode.AstNode as Node).Generate(Context);
			(ValueNode.AstNode as Node).Generate(Context);
			Context.Call((Action<Php54Var, Php54Var>)Php54Var.Assign);
		}
	}

	public class EchoNode : IgnoreNode
	{
		public override void Generate(NodeGenerateContext Context)
		{
			base.Generate(Context);
			Context.Call((Action<Php54Var>)Php54Runtime.Echo);
		}
	}

	public class IfNode : Node
	{
		ParseTreeNode ConditionExpresion;
		ParseTreeNode TrueSentence;
		ParseTreeNode FalseSentence;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Debug.Assert("if" == parseNode.ChildNodes[0].FindTokenAndGetText());
			ConditionExpresion = parseNode.ChildNodes[1];
			TrueSentence = parseNode.ChildNodes[2];
			if (parseNode.ChildNodes.Count > 3)
			{
				Debug.Assert("else" == parseNode.ChildNodes[3].FindTokenAndGetText());
				FalseSentence = parseNode.ChildNodes[4];
			}
		}

		public override void Generate(NodeGenerateContext Context)
		{
			var EndLabel = Context.DefineLabel();
			var FalseLabel = Context.DefineLabel();
			// Check condition
			{
				(ConditionExpresion.AstNode as Node).Generate(Context);
				Context.Call((Func<Php54Var, bool>)Php54Var.ToBool);
				Context.BranchIfFalse(FalseLabel);
			}
			// True
			{
				(TrueSentence.AstNode as Node).Generate(Context);
				Context.BranchAlways(EndLabel);
			}
			// False
			Context.MarkLabel(FalseLabel);
			if (FalseSentence != null)
			{
				(FalseSentence.AstNode as Node).Generate(Context);
				Context.BranchAlways(EndLabel);
			}
			Context.MarkLabel(EndLabel);
			//base.Generate(Context);
		}
	}

	/*
	public class SentenceNode : Node
	{
	}
	*/
}
