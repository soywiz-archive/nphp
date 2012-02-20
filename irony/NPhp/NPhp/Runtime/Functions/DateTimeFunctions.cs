using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPhp.Runtime.Functions
{
	/// <summary>
	/// 
	/// </summary>
	/// <see cref="http://es.php.net/manual/en/ref.datetime.php"/>
	[Php54NativeLibrary]
	public class DateTimeFunctions
	{
		static public double microtime(bool get_as_float = false)
		{
			if (get_as_float)
			{
				return (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
			}
			else
			{
				throw(new NotImplementedException());
			}
		}
	}
}
