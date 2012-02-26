using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NPhp.Runtime.Functions
{
	[Php54NativeLibrary]
	public class FilesystemFunctions
	{
		static public bool is_file(string FilePath)
		{
			return (new FileInfo(FilePath)).Exists;
		}

		static public bool is_dir(string FilePath)
		{
			return (new DirectoryInfo(FilePath)).Exists;
		}

		static public string file_get_contents(string FilePath)
		{
			return File.ReadAllText(FilePath, Encoding.Default);
		}
	}
}
