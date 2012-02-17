using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;

namespace NPhp
{
	public class Php54Runtime
	{
		Php54Grammar Grammar;
		LanguageData LanguageData;
		Parser Parser;

		public Php54Runtime()
		{
			Grammar = new Php54Grammar();
			LanguageData = new LanguageData(Grammar);
			Parser = new Parser(LanguageData);
			Parser.Context.TracingEnabled = true;
		}

		public Action CreateMethodFromCode(string Code, string File = "<source>", bool DumpTree = false)
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
			var Action = (Tree.Root.AstNode as Node).CreateMethod();
			return Action;
		}

		static public void Echo(int Value)
		{
			Console.Write(Value);
		}
	}
}
