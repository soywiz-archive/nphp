using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPhp.Runtime
{
	public class Php54FunctionScope
	{
		public Dictionary<string, Action<Php54Scope>> Functions { get; protected set; }

		public Php54FunctionScope()
		{
			Functions = new Dictionary<string, Action<Php54Scope>>();
		}
	}
}
