using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using System.Reflection.Emit;
using NPhp.Codegen.Nodes;
using NPhp.Codegen;

namespace NPhp.LanguageGrammar
{
	[Language("Php", "5.4", "PHP 5.4 grammar")]
	public class Php54Grammar : Grammar
	{
		static public AstNodeCreator GetCreator<TType>() where TType : Node, new()
		{
			return (context, parseNode) => {
				var Item = new TType();
				Item.Init(context, parseNode);
				parseNode.AstNode = Item;
			};
		}

		public readonly StringLiteral StringSingleQuoteTerminal = new StringLiteral("StringSingleQuoteTerminal", "'", StringOptions.AllowsAllEscapes);
		public readonly IdentifierTerminal IdTerminal = new IdentifierTerminal("IdTerminal", IdOptions.None);
		public readonly IdentifierTerminal VariableTerminal = new IdentifierTerminal("VariableTerminal", IdOptions.None);
		public readonly NumberLiteral Number = TerminalFactory.CreateCSharpNumber("Number");
		public readonly NonTerminal SpecialLiteral = new NonTerminal("SpecialLiteral", GetCreator<SpecialLiteralNode>());

		public readonly NonTerminal GetId = new NonTerminal("GetId", GetCreator<IdentifierNode>());
		public readonly NonTerminal Constant = new NonTerminal("Constant", GetCreator<GetConstantNode>());

		public readonly NonTerminal Sentence = new NonTerminal("Sentence", GetCreator<IgnoreNode>());
		public readonly NonTerminal SentenceList = new NonTerminal("SentenceList", GetCreator<IgnoreNode>());
		public readonly NonTerminal BaseSentence = new NonTerminal("BaseSentence", GetCreator<IgnoreNode>());
		public readonly NonTerminal EchoSentence = new NonTerminal("EchoSentence", GetCreator<EchoNode>());
		public readonly NonTerminal EvalSentence = new NonTerminal("EvalSentence", GetCreator<EvalNode>());
		public readonly NonTerminal CurlySentence = new NonTerminal("CurlySentence", GetCreator<IgnoreNode>());
		public readonly NonTerminal IfSentence = new NonTerminal("IfSentence", GetCreator<IfNode>());
		public readonly NonTerminal IfElseSentence = new NonTerminal("IfElseSentence", GetCreator<IfNode>());
		public readonly NonTerminal WhileSentence = new NonTerminal("WhileSentence", GetCreator<WhileNode>());
		public readonly NonTerminal ForSentence = new NonTerminal("ForSentence", GetCreator<ForNode>());

		public readonly NonTerminal ExpressionSentence = new NonTerminal("ExpressionSentence", GetCreator<IgnoreNode>());

		//var unary_op = new NonTerminal("unary_op", GetCreator<OperatorNode>());

		public readonly NonTerminal AssignmentOperator = new NonTerminal("AssignmentOperator", GetCreator<IgnoreNode>());
		public readonly NonTerminal BinaryOperator = new NonTerminal("BinaryOperator", GetCreator<BinaryOperatorNode>());
		public readonly NonTerminal BinaryOperation = new NonTerminal("BinaryOperation", GetCreator<BinaryExpressionNode>());
		public readonly NonTerminal Expression = new NonTerminal("Expression", GetCreator<IgnoreNode>());
		public readonly NonTerminal ExpressionOrEmpty = new NonTerminal("ExpressionOrEmpty", GetCreator<IgnoreNode>());
		public readonly NonTerminal SubExpression = new NonTerminal("SubExpression", GetCreator<IgnoreNode>());
		public readonly NonTerminal Literal = new NonTerminal("Literal", GetCreator<IgnoreNode>());
		public readonly NonTerminal Assignment = new NonTerminal("Assignment", GetCreator<AssignmentNode>());
		public readonly NonTerminal GetVariable = new NonTerminal("GetVariable", GetCreator<GetVariableNode>());

		public readonly NonTerminal UnaryOperator = new NonTerminal("UnaryOperator", GetCreator<UnaryOperatorNode>());
		public readonly NonTerminal UnaryExpression = new NonTerminal("UnaryExpression", GetCreator<UnaryExpressionNode>());

		public readonly NonTerminal PostOperator = new NonTerminal("PostOperator", GetCreator<UnaryPostOperationNode>());
		public readonly NonTerminal LeftValuePostOperation = new NonTerminal("LeftValuePostOperation", GetCreator<PostOperationNode>());

		public readonly NonTerminal PreOperator = new NonTerminal("PreOperator", GetCreator<UnaryPreOperationNode>());
		public readonly NonTerminal LeftValuePreOperation = new NonTerminal("LeftValuePreOperation", GetCreator<PreOperationNode>());

		public readonly NonTerminal NamedFunctionDeclarationSentence = new NonTerminal("NamedFunctionDeclarationSentence", GetCreator<FunctionNamedDeclarationNode>());

		public readonly NonTerminal FunctionDeclarationArguments = new NonTerminal("FunctionDeclarationArguments", GetCreator<IgnoreNode>());
		public readonly NonTerminal FunctionCall = new NonTerminal("FunctionCall", GetCreator<FunctionCallNode>());
		public readonly NonTerminal FunctionArguments = new NonTerminal("FunctionArguments", GetCreator<IgnoreNode>());
		public readonly NonTerminal ReturnSentence = new NonTerminal("ReturnSentence", GetCreator<ReturnNode>());
		public readonly NonTerminal IncludeSentence = new NonTerminal("IncludeSentence", GetCreator<IncludeNode>());
		public readonly NonTerminal IncludeKeyword = new NonTerminal("IncludeKeyword", GetCreator<IgnoreNode>());

		public readonly NonTerminal NumberOrString = new NonTerminal("NumberOrString", GetCreator<IgnoreNode>());
		public readonly NonTerminal ArrayKeyValueElement = new NonTerminal("ArrayKeyValueElement", GetCreator<IgnoreNode>());
		public readonly NonTerminal ArrayElement = new NonTerminal("ArrayElement", GetCreator<IgnoreNode>());
		public readonly NonTerminal ArrayElements = new NonTerminal("ArrayElements", GetCreator<IgnoreNode>());
		public readonly NonTerminal ArrayExpression1 = new NonTerminal("ArrayExpression1", GetCreator<ArrayNode>());
		public readonly NonTerminal ArrayExpression2 = new NonTerminal("ArrayExpression2", GetCreator<ArrayNode>());

		public readonly NonTerminal ForeachSentence = new NonTerminal("ForeachSentence", GetCreator<ForeachNode>());
		public readonly NonTerminal ForeachPairSentence = new NonTerminal("ForeachPairSentence", GetCreator<ForeachNode>());
		

		public readonly NonTerminal GetVariableRankElement = new NonTerminal("GetVariableRankElement", GetCreator<IgnoreNode>());
		public readonly NonTerminal GetVariableRank = new NonTerminal("GetVariableRank", GetCreator<IgnoreNode>());
		public readonly NonTerminal GetVariableRankOpt = new NonTerminal("GetVariableRankOpt", GetCreator<IgnoreNode>());
		public readonly NonTerminal GetVariableBase = new NonTerminal("GetVariableBase", GetCreator<IgnoreNode>());

		public readonly RawContentTerminal RawContentTerminal  = new RawContentTerminal("RawContentTerminal");
		public readonly NonTerminal RawContent = new NonTerminal("RawContent", GetCreator<OutsidePhpNode>());
		public readonly NonTerminal PhpFile = new NonTerminal("PhpFile", GetCreator<IgnoreNode>());

		public Php54Grammar()
			: base(caseSensitive: false)
		{
			this.GrammarComments = "PHP 5.4";
			NonGrammarTerminals.Add(new CommentTerminal("SingleLineComment", "//", "\r", "\n", "\u2085", "\u2028", "\u2029"));
			NonGrammarTerminals.Add(new CommentTerminal("DelimitedComment", "/*", "*/"));

			StringSingleQuoteTerminal.AstConfig.NodeCreator = GetCreator<StringNode>();
			IdTerminal.AstConfig.NodeCreator = GetCreator<IdentifierNameNode>();

			VariableTerminal.AstConfig.NodeCreator = GetCreator<VariableNameNode>();
			VariableTerminal.AddPrefix("$", IdOptions.None);


			Number.AstConfig.NodeCreator = GetCreator<NumberNode>();

			IncludeKeyword.Rule = ToTerm("include") | "include_once" | "require" | "require_once";
			IncludeSentence.Rule = IncludeKeyword + Expression + ";";

			ReturnSentence.Rule = ToTerm("return") + Expression + ";";

			FunctionDeclarationArguments.Rule = MakeStarRule(FunctionDeclarationArguments, ToTerm(","), GetVariable);

			NamedFunctionDeclarationSentence.Rule = ToTerm("function") + GetId + "(" + FunctionDeclarationArguments + ")" + "{" + SentenceList + "}";

			GetId.Rule = IdTerminal;

			//semi_opt.Rule = Empty | semi;

			GetVariableRankElement.Rule = ToTerm("[") + Expression + ToTerm("]");
			GetVariableRank.Rule = MakeStarRule(GetVariableRank, GetVariableRankElement);
			//GetVariableRankOpt.Rule = GetVariableRank.Q();
			GetVariableBase.Rule = VariableTerminal;
			GetVariable.Rule = GetVariableBase + GetVariableRank;

			EchoSentence.Rule = ToTerm("echo") + Expression + ToTerm(";");
			EvalSentence.Rule = ToTerm("eval") + Expression + ToTerm(";");
			ExpressionSentence.Rule = Expression + ToTerm(";");
			CurlySentence.Rule = ToTerm("{") + SentenceList + ToTerm("}");

			IfElseSentence.Rule = ToTerm("if") + "(" + Expression + ")" + Sentence + PreferShiftHere() + ToTerm("else") + Sentence;
			IfSentence.Rule = ToTerm("if") + ToTerm("(") + Expression + ToTerm(")") + Sentence;
			WhileSentence.Rule = ToTerm("while") + "(" + Expression + ")" + Sentence;
			ForeachSentence.Rule = ToTerm("foreach") + "(" + Expression + "as" + GetVariable + ")" + Sentence;
			ForeachPairSentence.Rule = ToTerm("foreach") + "(" + Expression + "as" + GetVariable + "=>" + GetVariable + ")" + Sentence;
			ForSentence.Rule = ToTerm("for") + "(" + ExpressionOrEmpty + ";" + ExpressionOrEmpty + ";" + ExpressionOrEmpty + ")" + Sentence;

			BaseSentence.Rule =
				CurlySentence
				| EchoSentence
				| EvalSentence
				| WhileSentence
				| ForeachSentence
				| ForeachPairSentence
				| ForSentence
				| IfSentence
				| IfElseSentence
				| IncludeSentence
				| ExpressionSentence
				| ReturnSentence
				| NamedFunctionDeclarationSentence
			;

			SentenceList.Rule = MakeStarRule(SentenceList, Sentence);

			Sentence.Rule =
				BaseSentence
			;

			BinaryOperation.Rule = Expression + BinaryOperator + Expression;

			BinaryOperator.Rule =
				ToTerm("<")
				| "||" | "&&" | "|" | "^" | "&" | "==" | "!=" | ">" | "<=" | ">=" | "<<" | ">>" | "+" | "-" | "*" | "/" | "%" | "."
				//| "=" | "+=" | "-=" | "*=" | "/=" | "%=" | "&=" | "|=" | "^=" | "<<=" | ">>="
				| "is" | "as" | "??"
			;

			AssignmentOperator.Rule =
				ToTerm("=") | "+=" | "-=" | "*=" | "/=" | "%=" | "&=" | "|=" | "^=" | "<<=" | ">>="
			;

			SpecialLiteral.Rule =
				ToTerm("true")
				| "false"
				| "null"
				| "__DIR__"
				| "__FILE__"
				| "__FUNCTION__"
				| "__CLASS__"
				| "__METHOD__"
				| "__LINE__"
				| "__NAMESPACE__"
			;

			NumberOrString.Rule =
				Number
				| StringSingleQuoteTerminal
			;


			Literal.Rule =
				NumberOrString
				| GetVariable
				| SpecialLiteral
			;

			PostOperator.Rule = ToTerm("++") | "--";

			LeftValuePostOperation.Rule =
				GetVariable + PostOperator
			;

			PreOperator.Rule = ToTerm("++") | "--";

			LeftValuePreOperation.Rule =
				PreOperator + GetVariable
			;

			RegisterOperators(10, "?");
			RegisterOperators(15, "&", "&&", "|", "||");
			RegisterOperators(20, "==", "<", "<=", ">", ">=", "!=");
			RegisterOperators(30, "+", "-");
			RegisterOperators(40, "*", "/");
			RegisterOperators(50, Associativity.Right, "**");

			MarkPunctuation("(", ")", "?", ":", ";", "[", "]", "{", "}", "=>", "as");
			RegisterBracePair("(", ")");
			RegisterBracePair("[", "]");
			//MarkTransient(Term, Expr, Statement, BinOp, UnOp, IncDecOp, AssignmentOp, ParExpr, ObjectRef);

			SubExpression.Rule = ToTerm("(") + Expression + ")";

			UnaryOperator.Rule = ToTerm("+") | ToTerm("-") | ToTerm("!") | ToTerm("~") | ToTerm("&");

			UnaryExpression.Rule = UnaryOperator + Expression;

			//assignment.Rule = VariableTerminal + "=" + expr;
			Assignment.Rule = GetVariable + AssignmentOperator + Expression;

			FunctionArguments.Rule = MakeStarRule(FunctionArguments, ToTerm(","), Expression);

			FunctionCall.Rule = GetId + "(" + FunctionArguments + ")";
			Constant.Rule = GetId;

			ArrayKeyValueElement.Rule = NumberOrString + "=>" + Expression;

			ArrayElement.Rule =
				ArrayKeyValueElement |
				Expression
			;
			ArrayElements.Rule = MakeStarRule(ArrayElements, ToTerm(","), ArrayElement);
			ArrayExpression1.Rule = ToTerm("array") + PreferShiftHere() + "(" + ArrayElements + ")";
			ArrayExpression2.Rule = ToTerm("[") + PreferShiftHere() + ArrayElements + "]";

			//var expression = new NonTerminal("comma_opt", Empty | comma);
			Expression.Rule =
				FunctionCall
				| ArrayExpression1
				| ArrayExpression2
				| LeftValuePreOperation
				| Literal
				| LeftValuePostOperation
				| BinaryOperation
				| UnaryExpression
				| SubExpression
				| Assignment
				| Constant
			;

			ExpressionOrEmpty.Rule = Expression | Empty;

			PhpFile.Rule = SentenceList;

			Root = PhpFile;
			Root.AstConfig.DefaultNodeCreator = () => { return null; };

			LanguageFlags = LanguageFlags.CreateAst;
		}

		public override void BuildAst(LanguageData language, ParseTree parseTree)
		{
			base.BuildAst(language, parseTree);
		}
	}

	public class RawContentTerminal : Terminal
	{
		public RawContentTerminal(string Name)
			: base(Name)
		{
		}

		public override Token TryMatch(ParsingContext context, ISourceStream source)
		{
			var Text = source.Text;
			return null;
		}
	}
}
