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

		public readonly StringLiteral StringSingleQuoteTerminal = new StringLiteral("StringLiteral", "'", StringOptions.AllowsAllEscapes);
		public readonly IdentifierTerminal IdTerminal = new IdentifierTerminal("identifier", IdOptions.None);
		public readonly IdentifierTerminal VariableTerminal = new IdentifierTerminal("variable", IdOptions.None);
		public readonly NumberLiteral Number = TerminalFactory.CreateCSharpNumber("Number");
		public readonly NonTerminal SpecialLiteral = new NonTerminal("SpecialLiteral", GetCreator<SpecialLiteralNode>());

		public readonly NonTerminal GetId = new NonTerminal("GetId", GetCreator<IdentifierNode>());
		public readonly NonTerminal Constant = new NonTerminal("constant", GetCreator<GetConstantNode>());

		public readonly NonTerminal Sentence = new NonTerminal("sentence", GetCreator<IgnoreNode>());
		public readonly NonTerminal SentenceList = new NonTerminal("sentences", GetCreator<IgnoreNode>());
		public readonly NonTerminal BaseSentence = new NonTerminal("base_sentence", GetCreator<IgnoreNode>());
		public readonly NonTerminal EchoSentence = new NonTerminal("echo_base_sentence", GetCreator<EchoNode>());
		public readonly NonTerminal EvalSentence = new NonTerminal("eval_base_sentence", GetCreator<EvalNode>());
		public readonly NonTerminal CurlySentence = new NonTerminal("curly_sentence", GetCreator<IgnoreNode>());
		public readonly NonTerminal IfSentence = new NonTerminal("if_sentence", GetCreator<IfNode>());
		public readonly NonTerminal IfElseSentence = new NonTerminal("if_else_sentence", GetCreator<IfNode>());
		public readonly NonTerminal WhileSentence = new NonTerminal("while_sentence", GetCreator<WhileNode>());
		public readonly NonTerminal ForSentence = new NonTerminal("for_sentence", GetCreator<ForNode>());

		public readonly NonTerminal ExpressionSentence = new NonTerminal("expression_sentence", GetCreator<IgnoreNode>());

		//var unary_op = new NonTerminal("unary_op", GetCreator<OperatorNode>());

		public readonly NonTerminal AssignmentOperator = new NonTerminal("assign_op", GetCreator<IgnoreNode>());
		public readonly NonTerminal BinaryOperator = new NonTerminal("bin_op", GetCreator<BinaryOperatorNode>());
		public readonly NonTerminal BinaryOperation = new NonTerminal("bin_op_expression", GetCreator<BinaryExpressionNode>());
		public readonly NonTerminal Expression = new NonTerminal("expr", GetCreator<IgnoreNode>());
		public readonly NonTerminal ExpressionOrEmpty = new NonTerminal("expr_or_empty", GetCreator<IgnoreNode>());
		public readonly NonTerminal Expression2 = new NonTerminal("expr2", GetCreator<IgnoreNode>());
		public readonly NonTerminal Literal = new NonTerminal("literal", GetCreator<IgnoreNode>());
		public readonly NonTerminal Assignment = new NonTerminal("assignment", GetCreator<AssignmentNode>());
		public readonly NonTerminal GetVariable = new NonTerminal("get_variable", GetCreator<GetVariableNode>());

		public readonly NonTerminal UnaryOperator = new NonTerminal("unary_op", GetCreator<UnaryOperatorNode>());
		public readonly NonTerminal UnaryExpression = new NonTerminal("unary_expr", GetCreator<UnaryExpressionNode>());

		public readonly NonTerminal PostOperator = new NonTerminal("post", GetCreator<UnaryPostOperationNode>());
		public readonly NonTerminal LeftValuePostOperation = new NonTerminal("literal_post", GetCreator<PostOperationNode>());

		public readonly NonTerminal PreOperator = new NonTerminal("pre", GetCreator<UnaryPreOperationNode>());
		public readonly NonTerminal LeftValuePreOperation = new NonTerminal("literal_pret", GetCreator<PreOperationNode>());

		public readonly NonTerminal NamedFunctionDeclarationSentence = new NonTerminal("named_func_decl", GetCreator<FunctionNamedDeclarationNode>());

		public readonly NonTerminal FunctionDeclarationArguments = new NonTerminal("func_decl_args", GetCreator<IgnoreNode>());
		public readonly NonTerminal FunctionCall = new NonTerminal("func_call", GetCreator<FunctionCallNode>());
		public readonly NonTerminal FunctionArguments = new NonTerminal("func_arguments", GetCreator<IgnoreNode>());
		public readonly NonTerminal ReturnSentence = new NonTerminal("return_sentence", GetCreator<ReturnNode>());
		public readonly NonTerminal IncludeSentence = new NonTerminal("include_sentence", GetCreator<IncludeNode>());
		public readonly NonTerminal IncludeKeyword = new NonTerminal("include_keyword", GetCreator<IgnoreNode>());

		public readonly NonTerminal NumberOrString = new NonTerminal("number_or_string", GetCreator<IgnoreNode>());
		public readonly NonTerminal ArrayKeyValueElement = new NonTerminal("array_key_value_element", GetCreator<IgnoreNode>());
		public readonly NonTerminal ArrayElement = new NonTerminal("array_element", GetCreator<IgnoreNode>());
		public readonly NonTerminal ArrayElements = new NonTerminal("array_elements", GetCreator<IgnoreNode>());
		public readonly NonTerminal ArrayExpression1 = new NonTerminal("array_expr", GetCreator<ArrayNode>());
		public readonly NonTerminal ArrayExpression2 = new NonTerminal("array_expr2", GetCreator<ArrayNode>());

		public readonly NonTerminal ForeachSentence = new NonTerminal("foreach_sentence", GetCreator<ForeachNode>());
		public readonly NonTerminal ForeachPairSentence = new NonTerminal("foreach_pair_sentence", GetCreator<ForeachNode>());
		

		public Php54Grammar()
			: base(caseSensitive: false)
		{
			var SemiColon = ToTerm(";", "semi");
			var Comma = ToTerm(",", "comma");


			this.GrammarComments = "PHP 5.4";
			NonGrammarTerminals.Add(new CommentTerminal("SingleLineComment", "//", "\r", "\n", "\u2085", "\u2028", "\u2029"));
			NonGrammarTerminals.Add(new CommentTerminal("DelimitedComment", "/*", "*/"));

			StringSingleQuoteTerminal.AstConfig.NodeCreator = GetCreator<StringNode>();
			IdTerminal.AstConfig.NodeCreator = GetCreator<IdentifierNameNode>();

			VariableTerminal.AstConfig.NodeCreator = GetCreator<VariableNameNode>();
			VariableTerminal.AddPrefix("$", IdOptions.None);


			Number.AstConfig.NodeCreator = GetCreator<NumberNode>();
			//var semi_opt = new NonTerminal("semi?");

			IncludeKeyword.Rule = ToTerm("include") | "include_once" | "require" | "require_once";
			IncludeSentence.Rule = IncludeKeyword + Expression + ";";

			ReturnSentence.Rule = "return" + Expression + ";";

			FunctionDeclarationArguments.Rule = MakeStarRule(FunctionDeclarationArguments, Comma, GetVariable);

			NamedFunctionDeclarationSentence.Rule = ToTerm("function") + GetId + "(" + FunctionDeclarationArguments + ")" + "{" + SentenceList + "}";

			GetId.Rule = IdTerminal;

			//semi_opt.Rule = Empty | semi;

			GetVariable.Rule = VariableTerminal;

			EchoSentence.Rule =
				"echo" + Expression + ";"
			;

			EvalSentence.Rule =
				"eval" + Expression + ";"
			;

			ExpressionSentence.Rule =
				Expression + ";"
			;

			CurlySentence.Rule = "{" + SentenceList + "}";

			IfElseSentence.Rule =
				ToTerm("if") + "(" + Expression + ")" + Sentence +
				PreferShiftHere() +
				ToTerm("else") + Sentence
			;

			IfSentence.Rule =
				ToTerm("if") + "(" + Expression + ")" +
				Sentence;

			WhileSentence.Rule =
				ToTerm("while") + "(" + Expression + ")" +
				Sentence
			;

			ForeachSentence.Rule =
				ToTerm("foreach") + "(" + Expression + "as" + GetVariable + ")" +
				Sentence
			;

			ForeachPairSentence.Rule =
				ToTerm("foreach") + "(" + Expression + "as" + GetVariable + "=>" + GetVariable + ")" +
				Sentence
			;

			ForSentence.Rule =
				ToTerm("for") + "(" + ExpressionOrEmpty + ";" + ExpressionOrEmpty + ";" + ExpressionOrEmpty + ")" +
				Sentence
			;

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

			Expression2.Rule = ToTerm("(") + Expression + ")";

			UnaryOperator.Rule = ToTerm("+") | ToTerm("-") | ToTerm("!") | ToTerm("~") | ToTerm("&");

			UnaryExpression.Rule = UnaryOperator + Expression;

			//assignment.Rule = VariableTerminal + "=" + expr;
			Assignment.Rule = GetVariable + AssignmentOperator + Expression;

			FunctionArguments.Rule = MakeStarRule(FunctionArguments, Comma, Expression);

			FunctionCall.Rule = GetId + "(" + FunctionArguments + ")";
			Constant.Rule = GetId;

			ArrayKeyValueElement.Rule = NumberOrString + "=>" + Expression;

			ArrayElement.Rule =
				ArrayKeyValueElement |
				Expression
			;
			ArrayElements.Rule = MakeStarRule(ArrayElements, Comma, ArrayElement);
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
				| Expression2
				| Assignment
				| Constant
			;

			ExpressionOrEmpty.Rule = Expression | Empty;

			Root = SentenceList;
			Root.AstConfig.DefaultNodeCreator = () => { return null; };

			LanguageFlags = LanguageFlags.CreateAst;
		}

		public override void BuildAst(LanguageData language, ParseTree parseTree)
		{
			base.BuildAst(language, parseTree);
		}
	}
}
