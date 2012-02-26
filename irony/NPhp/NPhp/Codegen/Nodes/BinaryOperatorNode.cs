//#define OPTIMIZE_SPECIAL_TYPES

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using NPhp.Runtime;
using System.Diagnostics;

namespace NPhp.Codegen.Nodes
{
	public class BinaryOperatorNode : Node
	{
		String Operator;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			Operator = parseNode.FindTokenAndGetText();
		}

		public override void Generate(NodeGenerateContext Context)
		{
			throw(new NotImplementedException("Must call Generate(Left, Right, Context)"));
		}

		static private void GenerateAndCast<TLeft, TRight>(Node Left, Node Right, NodeGenerateContext Context)
		{
			Left.GenerateAs<TLeft>(Context);
			Right.GenerateAs<TRight>(Context);
		}

		public void Generate(Node Left, Node Right, NodeGenerateContext Context)
		{
#if OPTIMIZE_SPECIAL_TYPES
			var LeftType = Context.MethodGenerator.CaptureStackInformation(() =>
			{
				Left.Generate(Context);
			}).GetLastest();

			var RightType = Context.MethodGenerator.CaptureStackInformation(() =>
			{
				Right.Generate(Context);
			}).GetLastest();

			// Optimized version.
			if (LeftType == RightType)
			{
				var CommonType = LeftType;

				if (CommonType == typeof(int))
				{
					Left.Generate(Context);
					Right.Generate(Context);
					switch (Operator)
					{
						case "+": Context.MethodGenerator.BinaryOperation(SafeILGenerator.BinaryOperatorEnum.AdditionSigned); break;
						case "-": Context.MethodGenerator.BinaryOperation(SafeILGenerator.BinaryOperatorEnum.SubstractionSigned); break;
						case "*": Context.MethodGenerator.BinaryOperation(SafeILGenerator.BinaryOperatorEnum.MultiplySigned); break;
						case "/": Context.MethodGenerator.BinaryOperation(SafeILGenerator.BinaryOperatorEnum.DivideSigned); break;
						case "&": Context.MethodGenerator.BinaryOperation(SafeILGenerator.BinaryOperatorEnum.And); break;
						case "|": Context.MethodGenerator.BinaryOperation(SafeILGenerator.BinaryOperatorEnum.Or); break;
						case "^": Context.MethodGenerator.BinaryOperation(SafeILGenerator.BinaryOperatorEnum.Xor); break;
						case "==": Context.MethodGenerator.CompareBinary(SafeBinaryComparison.Equals); break;
						case "!=": Context.MethodGenerator.CompareBinary(SafeBinaryComparison.NotEquals); break;
						case ">=": Context.MethodGenerator.CompareBinary(SafeBinaryComparison.GreaterOrEqualSigned); break;
						case "<=": Context.MethodGenerator.CompareBinary(SafeBinaryComparison.LessOrEqualSigned); break;
						case ">": Context.MethodGenerator.CompareBinary(SafeBinaryComparison.GreaterThanSigned); break;
						case "<": Context.MethodGenerator.CompareBinary(SafeBinaryComparison.LessThanSigned); break;
						default: throw (new NotImplementedException("Not implemented operator '" + Operator + "'"));
					}
					return;
				}
			}

			Debug.WriteLine("BINARY_OPERATION: {0}, {1}", LeftType, RightType);
#endif

			switch (Operator)
			{
				case "+": GenerateAndCast<Php54Var, Php54Var>(Left, Right, Context); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Add); break;
				case "-": GenerateAndCast<Php54Var, Php54Var>(Left, Right, Context); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Sub); break;
				case ".": GenerateAndCast<Php54Var, Php54Var>(Left, Right, Context); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Concat); break;
				case "*": GenerateAndCast<Php54Var, Php54Var>(Left, Right, Context); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Mul); break;
				case "/": GenerateAndCast<Php54Var, Php54Var>(Left, Right, Context); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Div); break;
				case "%": GenerateAndCast<Php54Var, Php54Var>(Left, Right, Context); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Mod); break;

				case "&": GenerateAndCast<Php54Var, Php54Var>(Left, Right, Context); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.BitAnd); break;
				case "|": GenerateAndCast<Php54Var, Php54Var>(Left, Right, Context); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.BitOr); break;

				case "===": GenerateAndCast<Php54Var, Php54Var>(Left, Right, Context); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, bool>)Php54Var.CompareStrictEquals); break;
				case "!==": GenerateAndCast<Php54Var, Php54Var>(Left, Right, Context); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, bool>)Php54Var.CompareStrictNotEquals); break;

				case "==": GenerateAndCast<Php54Var, Php54Var>(Left, Right, Context); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, bool>)Php54Var.CompareEquals); break;
				case "!=": GenerateAndCast<Php54Var, Php54Var>(Left, Right, Context); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, bool>)Php54Var.CompareNotEquals); break;
				case ">": GenerateAndCast<Php54Var, Php54Var>(Left, Right, Context); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, bool>)Php54Var.CompareGreaterThan); break;
				case ">=": GenerateAndCast<Php54Var, Php54Var>(Left, Right, Context); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, bool>)Php54Var.CompareGreaterOrEqualThan); break;
				case "<=": GenerateAndCast<Php54Var, Php54Var>(Left, Right, Context); Context.MethodGenerator.Call((Func<Php54Var, Php54Var, bool>)Php54Var.CompareLessOrEqualThan); break;
				case "<":
					if (Context.MethodGenerator.StackTop == typeof(int))
					{
						GenerateAndCast<Php54Var, int>(Left, Right, Context);
						Context.MethodGenerator.Call((Func<Php54Var, int, bool>)Php54Var.CompareLessThan);
					}
					else
					{
						GenerateAndCast<Php54Var, Php54Var>(Left, Right, Context);
						Context.MethodGenerator.Call((Func<Php54Var, Php54Var, bool>)Php54Var.CompareLessThan);
					}
					break;
				case "&&": GenerateAndCast<bool, bool>(Left, Right, Context); Context.MethodGenerator.Call((Func<bool, bool, bool>)Php54Var.LogicalAnd); break;
				case "||": GenerateAndCast<bool, bool>(Left, Right, Context); Context.MethodGenerator.Call((Func<bool, bool, bool>)Php54Var.LogicalOr); break;
				default: throw (new NotImplementedException("Not implemented operator '" + Operator + "'"));
			}
			//Context.Operator(Operator);
			//Console.WriteLine("Operator: '{0}'", Operator);
		}
	}
}
