using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPhp.Runtime;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace NPhp.PhpTess
{
	class Program
	{
		static Php54FunctionScope FunctionScope = new Php54FunctionScope();
		static Php54Runtime Runtime = new Php54Runtime(FunctionScope);

		static public void RunTest(string[] ContentLines)
		{
			var SectionName = "";
			var Sections = new Dictionary<string, string>();
			var HeaderRegex = new Regex(@"^--(\w+)--$", RegexOptions.Compiled);
			foreach (var Line in ContentLines)
			{
				if (HeaderRegex.IsMatch(Line))
				{
					Debug.Assert(Line.Substr(-2) == "--");
					SectionName = Line.Substr(2, -2);
				}
				else
				{
					if (!Sections.ContainsKey(SectionName)) Sections[SectionName] = "";
					Sections[SectionName] += Line + "\n";
				}
			}

			var TestName = "";
			var TestFile = "";
			var TestExpect = "";
			Sections.TryGetValue("TEST", out TestName);
			Sections.TryGetValue("FILE", out TestFile);
			Sections.TryGetValue("EXPECT", out TestExpect);
			TestName = TestName.Trim();
			TestFile = TestFile.Trim();
			TestExpect = TestExpect.Trim();
			//TestExpect = "aaa";

			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write("{0}...", TestName);
			Console.ForegroundColor = ConsoleColor.Red;

			try
			{
				var Method = Runtime.CreateMethodFromPhpFile(TestFile);
				var Scope = new Php54Scope(Runtime);
				var TestOutput = CaptureOutput(() =>
				{
					Method(Scope);
				}).Trim();

				
				if (TestOutput != TestExpect)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Error");
					var Result = Diff.DiffTextProcessed(TestOutput, TestExpect);
					foreach (var Item in Result.Items)
					{
						Item.Print();
					}
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Ok");
				}
			}
			catch (Exception Exception)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(Exception.Message);
			}

			//Console.WriteLine(TestFile);
		}

		static string CaptureOutput(Action Action)
		{
			var OldConsoleOut = Console.Out;
			var NewConsoleOut = new StringWriter();
			Console.SetOut(NewConsoleOut);
			try
			{
				Action();
			}
			finally
			{
				Console.SetOut(OldConsoleOut);
			}
			return NewConsoleOut.ToString();
		}

		static void Main(string[] args)
		{
			FunctionScope.LoadAllNativeFunctions();

			foreach (var PhptFile in Directory.EnumerateFiles("../../tests", "*.phpt", SearchOption.AllDirectories))
			//foreach (var PhptFile in Directory.EnumerateFiles("../../tests/func", "*.phpt", SearchOption.AllDirectories))
			{
				RunTest(File.ReadAllLines(PhptFile));
			}
			
			Console.ReadKey();
		}
	}
}
