using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPhp.Common
{
	public class Php54Utils
	{
		static public int StringToInteger(string Text)
		{
			if (Text.Length < 1) throw (new InvalidCastException());
			if (Text[0] == '-') return -StringToInteger(Text.Substring(1));
			if (Text[0] == '0')
			{
				if (Text.Length >= 2)
				{
					switch (Text.Substring(0, 2))
					{
						case "0x":
						case "0X": return Convert.ToInt32(Text.Substring(2), 16);
						case "0b":
						case "0B": return Convert.ToInt32(Text.Substring(2), 2);
					}
					return Convert.ToInt32(Text.Substring(1), 8);
				}
			}
			return int.Parse(Text);
		}

		static public string NormalizeFunctionName(string Name)
		{
			return Name.ToLower();
		}

		static public string StringQuote(string String)
		{
			return "\"" + String + "\"";
		}

		static public bool IsCompletelyNumeric(string String)
		{
			int StringLength = String.Length;
			if (StringLength > 0)
			{
				if (String[0] == '-') return IsCompletelyNumeric(String.Substring(1));
				for (int n = 0; n < StringLength; n++)
				{
					if (String[n] >= '0' && String[n] <= '9')
					{
					}
					else
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
	}
}
