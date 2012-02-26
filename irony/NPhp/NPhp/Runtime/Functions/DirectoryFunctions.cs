using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NPhp.Runtime.Functions
{
	[Php54NativeLibrary]
	public class DirectoryFunctions
	{
		static public Stream fopen(string Path, string Mode)
		{
			var XFileShare = FileShare.ReadWrite;
			var XFileAccess = FileAccess.Read;
			var XFileMode = FileMode.Open;

			foreach (var Char in Mode)
			{
				switch (Char)
				{
					case 'w':
						XFileShare = FileShare.None;
						XFileAccess = FileAccess.Write;
						XFileMode = FileMode.OpenOrCreate;
						break;
					case 'r':
						XFileShare = FileShare.None;
						XFileAccess = FileAccess.Read;
						XFileMode = FileMode.Open;
						break;
					case 'b':
						// Binary ignore.
						break;
					default: throw(new NotImplementedException("Unknown mode '" + Char + "'"));
				}
			}

			//Console.WriteLine(XFileMode);

			return File.Open(Path, XFileMode, XFileAccess, XFileShare);
		}

		static public bool feof(Stream Stream)
		{
			return Stream.Position >= Stream.Length;
		}

		static public void fwrite(Stream Stream, string Text)
		{
			var Bytes = Encoding.Default.GetBytes(Text);
			Stream.Write(Bytes, 0, Bytes.Length);
		}

		static public string fread(Stream Stream, int Count)
		{
			var Buffer = new byte[Count];
			int Readed = Stream.Read(Buffer, 0, Count);
			return Encoding.Default.GetString(Buffer, 0, Readed);
		}

		static public void fclose(Stream Stream)
		{
			Stream.Close();
		}

		static public Php54Var scandir(string Path)
		{
			var Return = Php54Var.FromNewArray();
			{
				Return.AddElement(".");
				Return.AddElement(Php54Var.FromString(".."));

				foreach (var Item in new DirectoryInfo(Path).EnumerateFileSystemInfos())
				{
					Return.AddElement(Item.Name);
				}
			}
			return Return;
		}
	}
}
