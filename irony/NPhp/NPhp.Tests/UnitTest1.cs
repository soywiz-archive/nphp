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
		[TestMethod]
		public void TestMethod1()
		{
			var Out = TestUtils.CaptureOutput(() =>
			{
				var Runtime = new Php54Runtime();
				var Method = Runtime.CreateMethodFromCode(@"
					echo(1+(2+3)*2)/11;
					echo 5;
					echo 2;
				");
				Method();
			});

			Assert.AreEqual("152", Out);
		}
	}
}
