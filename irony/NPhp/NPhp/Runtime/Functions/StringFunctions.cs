using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPhp.Runtime.Functions
{
	/// <summary>
	/// 
	/// </summary>
	/// <see cref="http://es2.php.net/manual/en/ref.strings.php"/>
	[Php54NativeLibrary]
	public class StringFunctions
	{
		/// <summary>
		/// Returns the portion of string specified by the start and length parameters.
		/// </summary>
		/// <param name="base_string">The input string. Must be one character or longer.</param>
		/// <param name="start">
		/// If start is non-negative, the returned string will start at the start'th position in string, counting from zero. For instance, in the string 'abcdef', the character at position 0 is 'a', the character at position 2 is 'c', and so forth.
		/// If start is negative, the returned string will start at the start'th character from the end of string.
		/// If string is less than or equal to start characters long, FALSE will be returned.
		/// </param>
		/// <param name="length">
		/// If length is given and is positive, the string returned will contain at most length characters beginning from start (depending on the length of string).
		/// If length is given and is negative, then that many characters will be omitted from the end of string (after the start position has been calculated when a start is negative). If start denotes the position of this truncation or beyond, false will be returned.
		/// If length is given and is 0, FALSE or NULL an empty string will be returned.
		/// If length is omitted, the substring starting from start until the end of the string will be returned.
		/// </param>
		/// <returns>
		/// Returns the extracted part of string; or FALSE on failure, or an empty string.
		/// </returns>
		static public string substr(string base_string, int start, int length = int.MaxValue)
		{
			if (start < 0) start = base_string.Length + start;
			if (length > base_string.Length - start) length = base_string.Length - start;
			return base_string.Substring(start, length);
		}

		/// <summary>
		/// Returns the length of the given string.
		/// </summary>
		/// <param name="base_string">The string being measured for length.</param>
		/// <returns>The length of the string on success, and 0 if the string is empty.</returns>
		static public int strlen(string base_string)
		{
			return base_string.Length;
		}

		static private int _hexdec(char hexstr)
		{
			if (hexstr >= '0' && hexstr <= '9') return hexstr - '0' + 0x0;
			if (hexstr >= 'a' && hexstr <= 'f') return hexstr - 'a' + 0xA;
			if (hexstr >= 'A' && hexstr <= 'F') return hexstr - 'f' + 0xF;
			throw(new InvalidCastException("Invalid hex digit '" + hexstr + "'"));
		}

		//static private readonly string DecHexLookupString = "0123456789ABCDEF";
		static private readonly string DecHexLookupString = "0123456789abcdef";

		static private char _dechex(int value)
		{
			return DecHexLookupString[value];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hexstr"></param>
		/// <returns></returns>
		static public int hexdec(string hexstr)
		{
			int value = 0;
			for (int n = 0; n < hexstr.Length; n++)
			{
				value <<= 4;
				value |= _hexdec(hexstr[n]);
			}
			return value;
		}

		static public string dechex(int _value)
		{
			uint value = (uint)_value;
			string hexstr = "";
			while (value > 0)
			{
				hexstr = _dechex((int)(value & 0xF)) + hexstr;
				value >>= 4;
			}
			return hexstr;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="String"></param>
		/// <returns></returns>
		static public int ord(string String)
		{
			return (int)String[0];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Ascii"></param>
		/// <returns></returns>
		static public string chr(int Ascii)
		{
			return char.ConvertFromUtf32(Ascii);
		}
	}
}
