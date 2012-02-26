using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPhp
{
	static public class StringExtensions
	{
		static public string Substr(this string String, int Position, int Length = int.MaxValue)
		{
			if (Position < 0) Position = String.Length + Position;
			if (Position < 0) Position = 0;
			if (Length < 0) Length = String.Length + Length - Position;
			Length = Math.Min(Length, String.Length - Position);
			if (Position >= String.Length || Length <= 0) return "";
			return String.Substring(Position, Length);
		}
	}
}
