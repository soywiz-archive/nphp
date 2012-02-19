using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;
using System.Reflection.Emit;

namespace NPhp
{
	[Language("Php", "5.4", "PHP 5.4 grammar")]
	public class Php54Grammar : Grammar
	{
		public AstNodeCreator GetCreator<TType>() where TType : Node, new()
		{
			return (context, parseNode) => {
				var Item = new TType();
				Item.Init(context, parseNode);
				parseNode.AstNode = Item;
			};
		}

		public Php54Grammar()
			: base(caseSensitive: false)
		{
			this.GrammarComments = "PHP 5.4";
			NonGrammarTerminals.Add(new CommentTerminal("SingleLineComment", "//", "\r", "\n", "\u2085", "\u2028", "\u2029"));
			NonGrammarTerminals.Add(new CommentTerminal("DelimitedComment", "/*", "*/"));

			var StringSingleQuoteTerminal = new StringLiteral("StringLiteral", "'", StringOptions.AllowsAllEscapes);
			StringSingleQuoteTerminal.AstConfig.NodeCreator = GetCreator<StringNode>();

			var VariableTerminal = new IdentifierTerminal("identifier", IdOptions.None);
			VariableTerminal.AstConfig.NodeCreator = GetCreator<VariableNameNode>();
			VariableTerminal.AddPrefix("$", IdOptions.None);


			var Number = TerminalFactory.CreateCSharpNumber("Number");
			Number.AstConfig.NodeCreator = GetCreator<NumberNode>();
			var semi = ToTerm(";", "semi");
			//var semi_opt = new NonTerminal("semi?");

			var SpecialLiteral = new NonTerminal("SpecialLiteral", GetCreator<SpecialLiteralNode>());

			var sentence = new NonTerminal("sentence", GetCreator<IgnoreNode>());
			var sentence_list = new NonTerminal("sentences", GetCreator<IgnoreNode>());
			var base_sentence = new NonTerminal("base_sentence", GetCreator<IgnoreNode>());
			var echo_base_sentence = new NonTerminal("echo_base_sentence", GetCreator<EchoNode>());
			var curly_sentence = new NonTerminal("curly_sentence", GetCreator<IgnoreNode>());
			var if_sentence = new NonTerminal("if_sentence", GetCreator<IfNode>());
			var if_else_sentence = new NonTerminal("if_else_sentence", GetCreator<IfNode>());
			var while_sentence = new NonTerminal("while_sentence", GetCreator<WhileNode>());
			
			var expression_sentence = new NonTerminal("expression_sentence", GetCreator<IgnoreNode>());

			//var unary_op = new NonTerminal("unary_op", GetCreator<OperatorNode>());

			var bin_op = new NonTerminal("bin_op", GetCreator<BinaryOperatorNode>());
			var bin_op_expression = new NonTerminal("bin_op_expression", GetCreator<BinaryExpression>());
			var expr = new NonTerminal("expr", GetCreator<IgnoreNode>());
			var expr2 = new NonTerminal("expr2", GetCreator<IgnoreNode>());
			var literal = new NonTerminal("literal", GetCreator<IgnoreNode>());
			var assignment = new NonTerminal("assignment", GetCreator<AssignmentNode>());
			var GetVariable = new NonTerminal("get_variable", GetCreator<GetVariableNode>());

			var unary_op = new NonTerminal("unary_op", GetCreator<UnaryOperatorNode>());
			var unary_expr = new NonTerminal("unary_expr", GetCreator<UnaryExpression>());

			//semi_opt.Rule = Empty | semi;

			GetVariable.Rule = VariableTerminal;

			echo_base_sentence.Rule =
				"echo" + expr + ";"
			;

			expression_sentence.Rule =
				expr + ";"
			;

			curly_sentence.Rule = "{" + sentence_list + "}";

			if_else_sentence.Rule =
				ToTerm("if") + "(" + expr + ")" + sentence +
				PreferShiftHere() +
				ToTerm("else") + sentence
			;

			if_sentence.Rule =
				ToTerm("if") + "(" + expr + ")" +
				sentence;

			while_sentence.Rule =
				ToTerm("while") + "(" + expr + ")" +
				sentence
			;

			base_sentence.Rule =
				curly_sentence |
				echo_base_sentence |
				while_sentence |
				if_else_sentence |
				if_sentence |
				expression_sentence
			;

			sentence_list.Rule = MakePlusRule(sentence_list, sentence);

			sentence.Rule =
				base_sentence
			;

			bin_op_expression.Rule = expr + bin_op + expr;

			bin_op.Rule =
				ToTerm("<")
				| "||" | "&&" | "|" | "^" | "&" | "==" | "!=" | ">" | "<=" | ">=" | "<<" | ">>" | "+" | "-" | "*" | "/" | "%" | "."
				| "=" | "+=" | "-=" | "*=" | "/=" | "%=" | "&=" | "|=" | "^=" | "<<=" | ">>="
				| "is" | "as" | "??"
			;

			SpecialLiteral.Rule =
				ToTerm("true")
				| "false"
				| "null"
			;

			literal.Rule =
				Number
				| StringSingleQuoteTerminal
				| GetVariable
				| SpecialLiteral
			;

			RegisterOperators(10, "?");
			RegisterOperators(15, "&", "&&", "|", "||");
			RegisterOperators(20, "==", "<", "<=", ">", ">=", "!=");
			RegisterOperators(30, "+", "-");
			RegisterOperators(40, "*", "/");
			RegisterOperators(50, Associativity.Right, "**");

			MarkPunctuation("(", ")", "?", ":", "[", "]");
			RegisterBracePair("(", ")");
			RegisterBracePair("[", "]");
			//MarkTransient(Term, Expr, Statement, BinOp, UnOp, IncDecOp, AssignmentOp, ParExpr, ObjectRef);

			expr2.Rule = "(" + expr + ")";

			unary_op.Rule = ToTerm("+") | ToTerm("-") | ToTerm("!") | ToTerm("~");

			unary_expr.Rule = unary_op + expr;

			//assignment.Rule = VariableTerminal + "=" + expr;
			assignment.Rule = GetVariable + "=" + expr;

			//var expression = new NonTerminal("comma_opt", Empty | comma);
			expr.Rule = 
				literal |
				bin_op_expression |
				unary_expr |
				expr2 |
				assignment
			;

			Root = sentence_list;
			Root.AstConfig.DefaultNodeCreator = () => { return null; };

			LanguageFlags = LanguageFlags.CreateAst;
		}

		public override void BuildAst(LanguageData language, ParseTree parseTree)
		{
			base.BuildAst(language, parseTree);
		}
	}
}
