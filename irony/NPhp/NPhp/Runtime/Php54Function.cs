using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPhp.Runtime
{
	public interface IPhp54Function
	{
		void Execute(Php54Scope Scope);
	}

	public class Php54Function : IPhp54Function
	{
		//protected Php54Runtime Runtime;
		protected Php54Scope StaticScope;
		protected Action<Php54Scope> Code;

		public Php54Function(/*Php54Runtime Runtime,*/ Action<Php54Scope> Code)
		{
			//this.Runtime = Runtime;
			this.Code = Code;
		}

		public void Execute(Php54Scope Scope)
		{
			if (StaticScope == null)
			{
				this.StaticScope = new Php54Scope(Scope.Runtime);
			}

			var OldStaticScope = Scope.StaticScope;
			try
			{
				Scope.StaticScope = this.StaticScope;
				Code(Scope);
			}
			finally
			{
				Scope.StaticScope = OldStaticScope;
			}
		}
	}

}
