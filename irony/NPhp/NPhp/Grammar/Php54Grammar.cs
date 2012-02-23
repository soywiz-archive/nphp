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
		public readonly NonTerminal constant = new NonTerminal("constant", GetCreator<GetConstantNode>());

		public readonly NonTerminal sentence = new NonTerminal("sentence", GetCreator<IgnoreNode>());
		public readonly NonTerminal sentence_list = new NonTerminal("sentences", GetCreator<IgnoreNode>());
		public readonly NonTerminal base_sentence = new NonTerminal("base_sentence", GetCreator<IgnoreNode>());
		public readonly NonTerminal echo_base_sentence = new NonTerminal("echo_base_sentence", GetCreator<EchoNode>());
		public readonly NonTerminal eval_base_sentence = new NonTerminal("eval_base_sentence", GetCreator<EvalNode>());
		public readonly NonTerminal curly_sentence = new NonTerminal("curly_sentence", GetCreator<IgnoreNode>());
		public readonly NonTerminal if_sentence = new NonTerminal("if_sentence", GetCreator<IfNode>());
		public readonly NonTerminal if_else_sentence = new NonTerminal("if_else_sentence", GetCreator<IfNode>());
		public readonly NonTerminal while_sentence = new NonTerminal("while_sentence", GetCreator<WhileNode>());
		public readonly NonTerminal for_sentence = new NonTerminal("for_sentence", GetCreator<ForNode>());

		public readonly NonTerminal expression_sentence = new NonTerminal("expression_sentence", GetCreator<IgnoreNode>());

		//var unary_op = new NonTerminal("unary_op", GetCreator<OperatorNode>());

		public readonly NonTerminal assign_op = new NonTerminal("assign_op", GetCreator<IgnoreNode>());
		public readonly NonTerminal bin_op = new NonTerminal("bin_op", GetCreator<BinaryOperatorNode>());
		public readonly NonTerminal bin_op_expression = new NonTerminal("bin_op_expression", GetCreator<BinaryExpressionNode>());
		public readonly NonTerminal expr = new NonTerminal("expr", GetCreator<IgnoreNode>());
		public readonly NonTerminal expr_or_empty = new NonTerminal("expr_or_empty", GetCreator<IgnoreNode>());
		public readonly NonTerminal expr2 = new NonTerminal("expr2", GetCreator<IgnoreNode>());
		public readonly NonTerminal literal = new NonTerminal("literal", GetCreator<IgnoreNode>());
		public readonly NonTerminal assignment = new NonTerminal("assignment", GetCreator<AssignmentNode>());
		public readonly NonTerminal GetVariable = new NonTerminal("get_variable", GetCreator<GetVariableNode>());

		public readonly NonTerminal unary_op = new NonTerminal("unary_op", GetCreator<UnaryOperatorNode>());
		public readonly NonTerminal unary_expr = new NonTerminal("unary_expr", GetCreator<UnaryExpressionNode>());

		public readonly NonTerminal post = new NonTerminal("post", GetCreator<UnaryPostOperationNode>());
		public readonly NonTerminal literal_post = new NonTerminal("literal_post", GetCreator<PostOperationNode>());

		public readonly NonTerminal pre = new NonTerminal("pre", GetCreator<UnaryPreOperationNode>());
		public readonly NonTerminal literal_pre = new NonTerminal("literal_pret", GetCreator<PreOperationNode>());

		public readonly NonTerminal named_func_decl = new NonTerminal("named_func_decl", GetCreator<FunctionNamedDeclarationNode>());

		public readonly NonTerminal func_decl_args = new NonTerminal("func_decl_args", GetCreator<IgnoreNode>());
		public readonly NonTerminal func_call = new NonTerminal("func_call", GetCreator<FunctionCallNode>());
		public readonly NonTerminal func_arguments = new NonTerminal("func_arguments", GetCreator<IgnoreNode>());
		public readonly NonTerminal return_sentence = new NonTerminal("return_sentence", GetCreator<ReturnNode>());
		public readonly NonTerminal include_sentence = new NonTerminal("include_sentence", GetCreator<IncludeNode>());
		public readonly NonTerminal include_keyword = new NonTerminal("include_keyword", GetCreator<IgnoreNode>());

		public readonly NonTerminal number_or_string = new NonTerminal("number_or_string", GetCreator<IgnoreNode>());
		public readonly NonTerminal array_key_value_element = new NonTerminal("array_key_value_element", GetCreator<IgnoreNode>());
		public readonly NonTerminal array_element = new NonTerminal("array_element", GetCreator<IgnoreNode>());
		public readonly NonTerminal array_elements = new NonTerminal("array_elements", GetCreator<IgnoreNode>());
		public readonly NonTerminal array_expr = new NonTerminal("array_expr", GetCreator<ArrayNode>());
		public readonly NonTerminal array_expr2 = new NonTerminal("array_expr2", GetCreator<ArrayNode>());

		public readonly NonTerminal foreach_sentence = new NonTerminal("foreach_sentence", GetCreator<ForeachNode>());
		public readonly NonTerminal foreach_pair_sentence = new NonTerminal("foreach_pair_sentence", GetCreator<ForeachNode>());
		

		public Php54Grammar()
			: base(caseSensitive: false)
		{
			var semi = ToTerm(";", "semi");
			var comma = ToTerm(",", "comma");


			this.GrammarComments = "PHP 5.4";
			NonGrammarTerminals.Add(new CommentTerminal("SingleLineComment", "//", "\r", "\n", "\u2085", "\u2028", "\u2029"));
			NonGrammarTerminals.Add(new CommentTerminal("DelimitedComment", "/*", "*/"));

			StringSingleQuoteTerminal.AstConfig.NodeCreator = GetCreator<StringNode>();
			IdTerminal.AstConfig.NodeCreator = GetCreator<IdentifierNameNode>();

			VariableTerminal.AstConfig.NodeCreator = GetCreator<VariableNameNode>();
			VariableTerminal.AddPrefix("$", IdOptions.None);


			Number.AstConfig.NodeCreator = GetCreator<NumberNode>();
			//var semi_opt = new NonTerminal("semi?");

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

			foreach_sentence.Rule =
				ToTerm("foreach") + "(" + expr + "as" + GetVariable + ")" +
				sentence
			;

			foreach_pair_sentence.Rule =
				ToTerm("foreach") + "(" + expr + "as" + GetVariable + "=>" + GetVariable + ")" +
				sentence
			;

			for_sentence.Rule =
				ToTerm("for") + "(" + expr_or_empty + ";" + expr_or_empty + ";" + expr_or_empty + ")" +
				sentence
			;

			base_sentence.Rule =
				curly_sentence
				| echo_base_sentence
				| eval_base_sentence
				| while_sentence
				| foreach_sentence
				| foreach_pair_sentence
				| for_sentence
				| if_else_sentence
				| if_sentence
				| include_sentence
				| expression_sentence
				| return_sentence
				| named_func_decl
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

			number_or_string.Rule =
				Number
				| StringSingleQuoteTerminal
			;


			literal.Rule =
				number_or_string
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

			MarkPunctuation("(", ")", "?", ":", ";", "[", "]", "{", "}", "=>", "as");
			RegisterBracePair("(", ")");
			RegisterBracePair("[", "]");
			//MarkTransient(Term, Expr, Statement, BinOp, UnOp, IncDecOp, AssignmentOp, ParExpr, ObjectRef);

			expr2.Rule = ToTerm("(") + expr + ")";

			unary_op.Rule = ToTerm("+") | ToTerm("-") | ToTerm("!") | ToTerm("~") | ToTerm("&");

			unary_expr.Rule = unary_op + expr;

			//assignment.Rule = VariableTerminal + "=" + expr;
			assignment.Rule = GetVariable + assign_op + expr;

			func_arguments.Rule = MakeStarRule(func_arguments, comma, expr);

			func_call.Rule = GetId + "(" + func_arguments + ")";
			constant.Rule = GetId;

			array_key_value_element.Rule = number_or_string + "=>" + expr;

			array_element.Rule =
				array_key_value_element |
				expr
			;
			array_elements.Rule = MakeStarRule(array_elements, comma, array_element);
			array_expr.Rule = ToTerm("array") + PreferShiftHere() + "(" + array_elements + ")";
			array_expr2.Rule = ToTerm("[") + PreferShiftHere() + array_elements + "]";

			//var expression = new NonTerminal("comma_opt", Empty | comma);
			expr.Rule =
				func_call
				| array_expr
				| array_expr2
				| literal_pre
				| literal
				| literal_post
				| bin_op_expression
				| unary_expr
				| expr2
				| assignment
				| constant
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
