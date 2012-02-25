﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPhp.Runtime
{
	public class Php54Function
	{
		protected Action<Php54Scope> Code;

		public Php54Function(Action<Php54Scope> Code)
		{
			this.Code = Code;
		}

		public void Execute(Php54Scope Scope)
		{
			Code(Scope);
		}
	}
}
