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
		public Php54Scope ConstantScope;
		Parser Parser;
		TextWriter TextWriter;
		bool InteractiveErrors;

		public Php54Runtime(Php54FunctionScope FunctionScope, bool InteractiveErrors = false)
		{
			if (Thread.CurrentThread.CurrentCulture.Name != "en-US")
			{
				Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
				Debug.WriteLine("Changed CultureInfo to en-US");
			}

			this.InteractiveErrors = InteractiveErrors;
			this.Grammar = new Php54Grammar();
			this.LanguageData = new LanguageData(Grammar);
			this.Parser = new Parser(LanguageData);
			this.Parser.Context.TracingEnabled = true;
			this.Parser.Context.MaxErrors = 5;
			this.TextWriter = Console.Out;
			this.FunctionScope = FunctionScope;
			this.ConstantScope = new Php54Scope(this);
		}

		public IPhp54Function CreateMethodFromPhpCode(string Code, string File = "<source>", bool DumpTree = false, bool DoDebug = false)
		{
#if true
			return InternalCreateMethodFromCode("<?php " + Code + " ?>", File, DumpTree, DoDebug);
#else
			return InternalCreateMethodFromCode(Code, File, DumpTree, DoDebug);
#endif
		}

		public IPhp54Function CreateMethodFromPhpFile(string Code, string File = "<source>", bool DumpTree = false, bool DoDebug = false)
		{
			return InternalCreateMethodFromCode(Code, File, DumpTree, DoDebug);
		}

		private IPhp54Function InternalCreateMethodFromCode(string Code, string File = "<source>", bool DumpTree = false, bool DoDebug = false)
		{
			var Tree = Parser.Parse(Code, File);

			//Console.WriteLine(Tree);
			if (Tree.HasErrors())
			{
				var Lines = Code.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
				var Errors = new List<string>();
				foreach (var Message in Tree.ParserMessages)
				{
					Errors.Add(String.Format("Error: {0} at {1}", Message.Message, Message.Location));
					var Line = Lines[Message.Location.Line];
					Errors.Add(String.Format("{0}: {1}", Message.Location.Line + 1, Line.Substr(0, Message.Location.Column) + "^^" + Line.Substr(Message.Location.Column)));
				}
				var ErrorsString = String.Join("\r\n", Errors);
				if (InteractiveErrors)
				{
					Console.Error.WriteLine(ErrorsString);
					Console.ReadKey();
					Environment.Exit(-1);
				}
				else
				{
					throw (new Exception(ErrorsString));
				}
			}

			if (DumpTree)
			{
				Console.WriteLine(Tree.ToXml());
			}
			//Console.WriteLine("'{0}'", Tree.Root.Term.AstConfig.NodeType);
			//Console.WriteLine("'{0}'", Tree.Root.AstNode);
			return (Tree.Root.AstNode as Node).CreateMethod(Tree.Root, FunctionScope, DoDebug);
		}

		public List<string> IncludedPaths = new List<string>();

		static public void Include(Php54Scope Scope, string Path, bool IsRequire, bool IsOnce)
		{
			var Runtime = Scope.Php54Runtime;
			var FullPath = new FileInfo(Path).FullName;
			if (IsOnce)
			{
				if (Runtime.IncludedPaths.Contains(FullPath)) return;
			}

			var Method = Scope.Php54Runtime.CreateMethodFromPhpFile(File.ReadAllText(FullPath), FullPath);
			Method.Execute(Scope);
	
			//Scope.Php54Runtime.TextWriter.Write(Variable);
			//Console.Out.Write(Variable);
			//throw(new NotImplementedException("Can't find path '" + Path + "' Require:" + IsRequire + ", Once:" + IsOnce + ""));
		}

		static public void Echo(Php54Scope Scope, Php54Var Variable)
		{
			//Scope.Php54Runtime.TextWriter.Write(Variable);
			Console.Out.Write(Variable);
		}

		static public void Eval(Php54Scope Scope, Php54Var Variable)
		{
			IPhp54Function EvalFunction;
			try
			{
				 EvalFunction = Scope.Php54Runtime.CreateMethodFromPhpCode(Variable.StringValue, "eval()");
			}
			catch (Exception Exception)
			{
				Console.WriteLine(Exception);
				return;
			}

			EvalFunction.Execute(Scope);
		}
	}

}
