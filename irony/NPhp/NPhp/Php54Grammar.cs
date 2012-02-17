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

			var Number = TerminalFactory.CreateCSharpNumber("Number");
			Number.AstConfig.NodeCreator = GetCreator<NumberNode>();
			//var semi = ToTerm(";", "semi");
			//var semi_opt = new NonTerminal("semi?");


			var sentence = new NonTerminal("sentence", GetCreator<IgnoreNode>());
			var base_sentence = new NonTerminal("base_sentence", GetCreator<IgnoreNode>());
			var echo_base_sentence = new NonTerminal("echo_base_sentence", GetCreator<EchoNode>());

			var bin_op = new NonTerminal("bin_op", GetCreator<OperatorNode>());
			var bin_op_expression = new NonTerminal("bin_op_expression", GetCreator<BinaryExpression>());
			var expr = new NonTerminal("expr", GetCreator<IgnoreNode>());
			var expr2 = new NonTerminal("expr2", GetCreator<IgnoreNode>());
			var literal = new NonTerminal("literal", GetCreator<IgnoreNode>());

			//semi_opt.Rule = Empty | semi;

			echo_base_sentence.Rule =
				"echo" + expr
			;

			base_sentence.Rule =
				echo_base_sentence
			;

			sentence.Rule =
				base_sentence + ";"
			;

			bin_op_expression.Rule = expr + bin_op + expr;

			bin_op.Rule =
				ToTerm("<")
				| "||" | "&&" | "|" | "^" | "&" | "==" | "!=" | ">" | "<=" | ">=" | "<<" | ">>" | "+" | "-" | "*" | "/" | "%"
				| "=" | "+=" | "-=" | "*=" | "/=" | "%=" | "&=" | "|=" | "^=" | "<<=" | ">>="
				| "is" | "as" | "??"
			;

			literal.Rule =
				Number
				//| StringLiteral
				//| CharLiteral
				| "true"
				| "false"
				| "null"
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

			//var expression = new NonTerminal("comma_opt", Empty | comma);
			expr.Rule = 
				literal |
				bin_op_expression |
				expr2
			;

			Root = sentence;
			Root.AstConfig.DefaultNodeCreator = () => { return null; };

			LanguageFlags = LanguageFlags.CreateAst;
		}

		public override void BuildAst(LanguageData language, ParseTree parseTree)
		{
			base.BuildAst(language, parseTree);
		}
	}
}
