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

		static private string RunAndCaptureOutput(string Code)
		{
			var Out = TestUtils.CaptureOutput(() =>
			{
				var Method = Runtime.CreateMethodFromCode(Code);
				Method();
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
	}
}
