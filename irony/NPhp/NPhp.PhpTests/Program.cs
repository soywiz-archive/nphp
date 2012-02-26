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
		static Php54Runtime Runtime = new Php54Runtime();

		static public TValue GetOrDefault<TKey, TValue>(Dictionary<TKey, TValue> Dictionary, TKey Key, TValue DefaultValue)
		{
			if (Dictionary.ContainsKey(Key)) return Dictionary[Key];
			return DefaultValue;
		}

		static public void RunTest(string[] ContentLines, string FileName)
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

			var TestName = GetOrDefault(Sections, "TEST", "").Trim();
			var TestFile = GetOrDefault(Sections, "FILE", "").Trim();
			var TestIni = GetOrDefault(Sections, "INI", "").Trim();
			var TestSkipIf = GetOrDefault(Sections, "SKIPIF", "").Trim();
			var TestExpect = GetOrDefault(Sections, "EXPECT", "").Trim();
			var TestExpectf = GetOrDefault(Sections, "EXPECTF", "").Trim();
			if (TestExpectf != "") TestExpect = TestExpectf;
			//TestExpect = "aaa";

			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write("{0}:{1}...", FileName, TestName);
			Console.ForegroundColor = ConsoleColor.Red;

			try
			{
				Runtime.Reset();
				var Method = Runtime.CreateMethodFromPhpFile(TestFile);
				var Scope = Runtime.GlobalScope;
				var TestOutput = CaptureOutput(() =>
				{
					Method.Execute(Scope);
					Runtime.Shutdown();
				}).Trim();

				
				if (TestOutput != TestExpect)
				{
					var Result = Diff.DiffTextProcessed(TestOutput, TestExpect);
					if (!Result.AreEquals)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine("Error");
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
				Console.WriteLine(Exception.StackTrace.Substr(0, 300));
				//Console.WriteLine(Exception);
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
			Runtime.FunctionScope.LoadAllNativeFunctions();

			foreach (var PhptFile in new DirectoryInfo("../../tests").EnumerateFiles("*.phpt", SearchOption.AllDirectories))
			//foreach (var PhptFile in Directory.EnumerateFiles("../../tests/func", "*.phpt", SearchOption.AllDirectories))
			{
				Directory.SetCurrentDirectory(PhptFile.DirectoryName);
				RunTest(File.ReadAllLines(PhptFile.FullName), PhptFile.Directory.Name + "/" + PhptFile.Name);
			}
			
			Console.ReadKey();
		}
	}
}
