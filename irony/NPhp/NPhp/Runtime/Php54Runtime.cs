using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using System.IO;
using NPhp.Codegen;
using NPhp.LanguageGrammar;
using System.Threading;
using System.Globalization;
using System.Diagnostics;

namespace NPhp.Runtime
{
	public class Php54Runtime
	{
		Php54Grammar Grammar;
		LanguageData LanguageData;
		public Php54FunctionScope FunctionScope;
		Parser Parser;
		TextWriter TextWriter;

		public Php54Runtime(Php54FunctionScope FunctionScope)
		{
			if (Thread.CurrentThread.CurrentCulture.Name != "en-US")
			{
				Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
				Debug.WriteLine("Changed CultureInfo to en-US");
			}

			this.Grammar = new Php54Grammar();
			this.LanguageData = new LanguageData(Grammar);
			this.Parser = new Parser(LanguageData);
			this.Parser.Context.TracingEnabled = true;
			this.TextWriter = Console.Out;
			this.FunctionScope = FunctionScope;
		}

		public Action<Php54Scope> CreateMethodFromCode(string Code, string File = "<source>", bool DumpTree = false)
		{
			var Tree = Parser.Parse(Code, File);

			//Console.WriteLine(Tree);
			if (Tree.HasErrors())
			{
				var Errors = "";
				foreach (var Message in Tree.ParserMessages)
				{
					Errors += String.Format("Error: {0} at {1}", Message.Message, Message.Location);
				}
				Console.Error.WriteLine(Errors);
				throw (new Exception(Errors));
			}

			if (DumpTree)
			{
				Console.WriteLine(Tree.ToXml());
			}
			//Console.WriteLine("'{0}'", Tree.Root.Term.AstConfig.NodeType);
			//Console.WriteLine("'{0}'", Tree.Root.AstNode);
			var Action = (Tree.Root.AstNode as Node).CreateMethod(FunctionScope);
			return Action;
		}

		static public void Echo(Php54Scope Scope, Php54Var Variable)
		{
			//Scope.Php54Runtime.TextWriter.Write(Variable);
			Console.Out.Write(Variable);
		}

		static public void Eval(Php54Scope Scope, Php54Var Variable)
		{
			var Action = Scope.Php54Runtime.CreateMethodFromCode(Variable.StringValue, "eval()");
			Action(Scope);
		}
	}

}
