using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NPhp.Runtime.Functions
{
	/// <summary>
	/// 
	/// </summary>
	/// <seealso cref="http://es2.php.net/manual/en/ref.outcontrol.php"/>
	[Php54NativeLibrary]
	public class OutputFunctions
	{
		static Stack<TextWriter> BufferList = new Stack<TextWriter>();
		static StringWriter Current;

		static public void __ob_reset()
		{
			while (BufferList.Count > 0) ob_end_clean();
			Current = new StringWriter();
		}

		static public void __ob_shutdown()
		{
			while (BufferList.Count > 0) ob_end_flush();
		}

		static public int ob_get_level()
		{
			return BufferList.Count;
		}

		static public void ob_start()
		{
			BufferList.Push(Console.Out);
			Console.SetOut(Current = new StringWriter());
		}

		static public string ob_get_contents()
		{
			if (Current == null) return "";
			return Current.ToString();
		}

		static public void ob_clean()
		{
			Current = new StringWriter();
		}

		/*
		static public void ob_flush()
		{
			var Contents = ob_get_contents();
			ob_clean();
		}
		*/

		static public void ob_end_clean()
		{
			if (BufferList.Count > 0)
			{
				Console.SetOut(BufferList.Pop());
			}
		}

		static public void ob_end_flush()
		{
			var Contents = ob_get_contents();
			ob_end_clean();
			Console.Write(Contents);
		}

		static public string ob_get_clean()
		{
			try
			{
				return ob_get_contents();
			}
			finally
			{
				ob_end_clean();
			}
		}
	}
}
