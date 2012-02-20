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

			var IdTerminal = new IdentifierTerminal("identifier", IdOptions.None);
			IdTerminal.AstConfig.NodeCreator = GetCreator<IdentifierNameNode>();

			var VariableTerminal = new IdentifierTerminal("variable", IdOptions.None);
			VariableTerminal.AstConfig.NodeCreator = GetCreator<VariableNameNode>();
			VariableTerminal.AddPrefix("$", IdOptions.None);


			var Number = TerminalFactory.CreateCSharpNumber("Number");
			Number.AstConfig.NodeCreator = GetCreator<NumberNode>();
			var semi = ToTerm(";", "semi");
			var comma = ToTerm(",", "comma");
			//var semi_opt = new NonTerminal("semi?");

			var SpecialLiteral = new NonTerminal("SpecialLiteral", GetCreator<SpecialLiteralNode>());

			var GetId = new NonTerminal("GetId", GetCreator<IdentifierNode>());
			var constant = new NonTerminal("constant", GetCreator<GetConstantNode>());

			var sentence = new NonTerminal("sentence", GetCreator<IgnoreNode>());
			var sentence_list = new NonTerminal("sentences", GetCreator<IgnoreNode>());
			var base_sentence = new NonTerminal("base_sentence", GetCreator<IgnoreNode>());
			var echo_base_sentence = new NonTerminal("echo_base_sentence", GetCreator<EchoNode>());
			var eval_base_sentence = new NonTerminal("eval_base_sentence", GetCreator<EvalNode>());
			var curly_sentence = new NonTerminal("curly_sentence", GetCreator<IgnoreNode>());
			var if_sentence = new NonTerminal("if_sentence", GetCreator<IfNode>());
			var if_else_sentence = new NonTerminal("if_else_sentence", GetCreator<IfNode>());
			var while_sentence = new NonTerminal("while_sentence", GetCreator<WhileNode>());
			var for_sentence = new NonTerminal("for_sentence", GetCreator<ForNode>());
			
			var expression_sentence = new NonTerminal("expression_sentence", GetCreator<IgnoreNode>());

			//var unary_op = new NonTerminal("unary_op", GetCreator<OperatorNode>());

			var assign_op = new NonTerminal("assign_op", GetCreator<IgnoreNode>());
			var bin_op = new NonTerminal("bin_op", GetCreator<BinaryOperatorNode>());
			var bin_op_expression = new NonTerminal("bin_op_expression", GetCreator<BinaryExpressionNode>());
			var expr = new NonTerminal("expr", GetCreator<IgnoreNode>());
			var expr_or_empty = new NonTerminal("expr_or_empty", GetCreator<IgnoreNode>());
			var expr2 = new NonTerminal("expr2", GetCreator<IgnoreNode>());
			var literal = new NonTerminal("literal", GetCreator<IgnoreNode>());
			var assignment = new NonTerminal("assignment", GetCreator<AssignmentNode>());
			var GetVariable = new NonTerminal("get_variable", GetCreator<GetVariableNode>());

			var unary_op = new NonTerminal("unary_op", GetCreator<UnaryOperatorNode>());
			var unary_expr = new NonTerminal("unary_expr", GetCreator<UnaryExpressionNode>());

			var post = new NonTerminal("post", GetCreator<UnaryPostOperationNode>());
			var literal_post = new NonTerminal("literal_post", GetCreator<PostOperationNode>());

			var pre = new NonTerminal("pre", GetCreator<UnaryPreOperationNode>());
			var literal_pre = new NonTerminal("literal_pret", GetCreator<PreOperationNode>());

			var named_func_decl = new NonTerminal("named_func_decl", GetCreator<FunctionNamedDeclarationNode>());
			
			var func_decl_args = new NonTerminal("func_decl_args", GetCreator<IgnoreNode>());
			var func_call = new NonTerminal("func_call", GetCreator<FunctionCallNode>());
			var func_arguments = new NonTerminal("func_arguments", GetCreator<IgnoreNode>());
			var return_sentence = new NonTerminal("return_sentence", GetCreator<ReturnNode>());
			var include_sentence = new NonTerminal("include_sentence", GetCreator<IncludeNode>());
			var include_keyword = new NonTerminal("include_keyword", GetCreator<IgnoreNode>());

			include_keyword.Rule = ToTerm("include") | "include_once" | "require" | "require_once";
			include_sentence.Rule = include_keyword + expr + ";";

			return_sentence.Rule = "return" + expr + ";";

			func_decl_args.Rule = MakeStarRule(func_decl_args, comma, GetVariable);

			named_func_decl.Rule = ToTerm("function") + GetId + "(" + func_decl_args + ")" + "{" + sentence_list + "}";

			GetId.Rule = IdTerminal;

			//semi_opt.Rule = Empty | semi;

			GetVariable.Rule = VariableTerminal;

			echo_base_sentence.Rule =
				"echo" + expr + ";"
			;

			eval_base_sentence.Rule =
				"eval" + expr + ";"
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

			for_sentence.Rule =
				ToTerm("for") + "(" + expr_or_empty + ";" + expr_or_empty + ";" + expr_or_empty + ")" +
				sentence
			;

			base_sentence.Rule =
				curly_sentence |
				echo_base_sentence |
				eval_base_sentence |
				while_sentence |
				for_sentence |
				if_else_sentence |
				if_sentence |
				include_sentence |
				expression_sentence |
				return_sentence |
				named_func_decl
			;

			sentence_list.Rule = MakeStarRule(sentence_list, sentence);

			sentence.Rule =
				base_sentence
			;

			bin_op_expression.Rule = expr + bin_op + expr;

			bin_op.Rule =
				ToTerm("<")
				| "||" | "&&" | "|" | "^" | "&" | "==" | "!=" | ">" | "<=" | ">=" | "<<" | ">>" | "+" | "-" | "*" | "/" | "%" | "."
				//| "=" | "+=" | "-=" | "*=" | "/=" | "%=" | "&=" | "|=" | "^=" | "<<=" | ">>="
				| "is" | "as" | "??"
			;

			assign_op.Rule =
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

			literal.Rule =
				Number
				| StringSingleQuoteTerminal
				| GetVariable
				| SpecialLiteral
			;

			post.Rule = ToTerm("++") | "--";

			literal_post.Rule =
				GetVariable + post
			;

			pre.Rule = ToTerm("++") | "--";

			literal_pre.Rule =
				pre + GetVariable
			;

			RegisterOperators(10, "?");
			RegisterOperators(15, "&", "&&", "|", "||");
			RegisterOperators(20, "==", "<", "<=", ">", ">=", "!=");
			RegisterOperators(30, "+", "-");
			RegisterOperators(40, "*", "/");
			RegisterOperators(50, Associativity.Right, "**");

			MarkPunctuation("(", ")", "?", ":", ";", "[", "]", "{", "}");
			RegisterBracePair("(", ")");
			RegisterBracePair("[", "]");
			//MarkTransient(Term, Expr, Statement, BinOp, UnOp, IncDecOp, AssignmentOp, ParExpr, ObjectRef);

			expr2.Rule = "(" + expr + ")";

			unary_op.Rule = ToTerm("+") | ToTerm("-") | ToTerm("!") | ToTerm("~") | ToTerm("&");

			unary_expr.Rule = unary_op + expr;

			//assignment.Rule = VariableTerminal + "=" + expr;
			assignment.Rule = GetVariable + assign_op + expr;

			func_arguments.Rule = MakeStarRule(func_arguments, comma, expr);

			func_call.Rule = GetId + "(" + func_arguments + ")";
			constant.Rule = GetId;

			//var expression = new NonTerminal("comma_opt", Empty | comma);
			expr.Rule =
				func_call |
				literal_pre |
				literal |
				literal_post |
				bin_op_expression |
				unary_expr |
				expr2 |
				assignment |
				constant
			;

			expr_or_empty.Rule = expr | Empty;

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
