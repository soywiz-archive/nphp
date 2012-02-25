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
			foreach (var Line in ContentLines)
			{
				if (Line.Substr(0, 2) == "--")
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

			Console.Write("{0}...", TestName);

			try
			{
				var Method = Runtime.CreateMethodFromPhpFile(TestFile);
				var Scope = new Php54Scope(Runtime);
				var TestOutput = CaptureOutput(() =>
				{
					Method(Scope);
				});

				Console.WriteLine("{0}", (TestOutput == TestExpect) ? "Ok" : "Error");
				if (TestOutput != TestExpect)
				{
					var Result = Diff.DiffTextProcessed(TestOutput, TestExpect);
					foreach (var Item in Result.Items)
					{
						Item.Print();
					}
				}
			}
			catch (Exception Exception)
			{
				Console.WriteLine(Exception);
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
