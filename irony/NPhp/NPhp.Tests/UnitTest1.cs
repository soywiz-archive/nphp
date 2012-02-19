using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NPhp.Tests
{
	[TestClass]
	public class UnitTest1
	{
		static Php54Runtime Runtime;

		[ClassInitialize]
		static public void PrepareRuntime(TestContext Context)
		{
			Runtime = new Php54Runtime();
		}

		static private string RunAndCaptureOutput(string Code, Dictionary<string, Php54Var> Variables = null)
		{
			var Out = TestUtils.CaptureOutput(() =>
			{
				var Method = Runtime.CreateMethodFromCode(Code);
				var Scope = new Php54Scope(Runtime);
				if (Variables != null)
				{
					foreach (var Pair in Variables)
					{
						Php54Var.Assign(Scope.GetVariable(Pair.Key), Pair.Value);
					}
				}
				Method(Scope);
			});
			return Out;
		}

		[TestMethod]
		public void SimpleEchoExpressionTest()
		{
			Assert.AreEqual("152", RunAndCaptureOutput(@"
				echo(1+(2+3)*2)/11;
				echo 5;
				echo 2;
			"));
		}

		[TestMethod]
		public void SimpleIfTest()
		{
			Assert.AreEqual("71", RunAndCaptureOutput(@"
				echo 7;
				//echo 2;
				if (0) {
					echo 0;
				}
				if (1) {
					echo 1;
				}
			"));
		}

		[TestMethod]
		public void SimpleIfElseTest()
		{
			Assert.AreEqual("2973", RunAndCaptureOutput(@"
				if (0) {
					echo 1;
				} else {
					echo 2;
				}
				if (1) echo 9;
				if (0) echo 8;
				echo 7;
				if (1) {
					echo 3;
				} else {
					echo 4;
				}
			"));
		}

		[TestMethod]
		public void UndefinedVarUse()
		{
			Assert.AreEqual("", RunAndCaptureOutput(@"
				echo $a;
			"));
		}

		[TestMethod]
		public void SimpleVarUse()
		{
			Assert.AreEqual("3", RunAndCaptureOutput(@"
				$a = 3;
				echo $a;
			"));
		}

		[TestMethod]
		public void UnaryOperation()
		{
			Assert.AreEqual("-3", RunAndCaptureOutput(@"
				$a = -(1 + 2);
				echo $a;
			"));
		}

		[TestMethod]
		public void Concat()
		{
			Assert.AreEqual("test-3::1:", RunAndCaptureOutput(@"
				$a = -(1 + 2);
				$b = false;
				$c = true;
				$d = null;
				echo 'test'.$a.':'.$b.':'.$c.':'.$d;
			"));
		}

		[TestMethod]
		public void Var1Use()
		{
			Assert.AreEqual("4yes", RunAndCaptureOutput(@"
				$a = (1 + 2);
				if ($a == 3) {
					echo $a + 1;
				} else {
					echo 'no';
				}
				if (($a != 2) && ($b != 4)) {
					echo 'yes';
				} else {
					echo 'no';
				}
			"));
		}
		
		[TestMethod]
		public void SimpleWhile()
		{
			Assert.AreEqual("987654321", RunAndCaptureOutput(@"
				$n = 9;
				while ($n > 0) {
					echo $n;
					$n = $n - 1;
				}
			"));
		}
	}
}
