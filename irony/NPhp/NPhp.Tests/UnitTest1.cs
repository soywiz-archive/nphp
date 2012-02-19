using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPhp.Runtime;

namespace NPhp.Tests
{
	[TestClass]
	public partial class UnitTest1
	{
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

		[TestMethod]
		public void SimpleFor()
		{
			Assert.AreEqual("0123456789", RunAndCaptureOutput(@"
				for ($n = 0; $n < 10; $n = $n + 1) echo $n;
			"));
		}

		[TestMethod]
		public void ForWithPostIncrement()
		{
			Assert.AreEqual("0123456789", RunAndCaptureOutput(@"
				for ($n = 0; $n < 10; $n++) echo $n;
			"));
		}

		[TestMethod]
		public void PartialFor()
		{
			Assert.AreEqual("0123456789", RunAndCaptureOutput(@"
				$n = 0;
				for (; $n < 10;) {
					echo $n++;
				}
			"));
		}

		[TestMethod]
		public void AddStrings()
		{
			Assert.AreEqual("10", RunAndCaptureOutput(@"
				echo '1test2' + 4 + '2demo' + 3;
			"));
		}

		[TestMethod]
		public void PostIncrementAndDecrement()
		{
			Assert.AreEqual("0130-1-3", RunAndCaptureOutput(@"
				$n = 0;
				echo $n++;
				echo $n++;
				echo ++$n;

				$n = 0;
				echo $n--;
				echo $n--;
				echo --$n;
			"));
		}

		[TestMethod]
		public void EvalTest()
		{
			Assert.AreEqual("1", RunAndCaptureOutput(@"
				eval('echo 1;');
			"));
		}

		[TestMethod]
		public void FunctionTest()
		{
			Assert.AreEqual("3", RunAndCaptureOutput(@"
				$a = -1;
				$b = -2;
				function add($a, $b) {
					echo $a + $b;
				}
				add(1, 2);
			"));
		}

		[TestMethod]
		public void RefTest()
		{
			Assert.AreEqual("2", RunAndCaptureOutput(@"
				$a = 1;
				$b = &$a;
				$b = 2;
				echo $a;
			"));
		}
	}

	public partial class UnitTest1
	{
		static Php54Runtime Runtime;
		static Php54FunctionScope FunctionScope;

		[ClassInitialize]
		static public void PrepareRuntime(TestContext Context)
		{
			FunctionScope = new Php54FunctionScope();
			Runtime = new Php54Runtime(FunctionScope);
		}

		[TestInitialize]
		public void PrepareRuntime()
		{
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

	}

}
