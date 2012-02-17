using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NPhp.Tests
{
	public class TestUtils
	{
		static public String CaptureOutput(Action Action)
		{
			var OldOut = Console.Out;
			var OldError = Console.Error;
			var OutWriter = new StringWriter();
			Console.SetOut(OutWriter);
			Console.SetError(OutWriter);
			try
			{
				Action();
			}
			finally
			{
				Console.SetOut(OldOut);
				Console.SetError(OldError);
			}
			return OutWriter.ToString();
		}
	}
}
