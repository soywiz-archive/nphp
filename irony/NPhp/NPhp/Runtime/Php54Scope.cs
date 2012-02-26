using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPhp.Runtime.Functions;

namespace NPhp.Runtime
{
	public class Php54Scope
	{
		public Php54Scope StaticScope;
		public Php54Runtime Runtime;
		protected Dictionary<string, Php54Var> Variables = new Dictionary<string, Php54Var>();
		public Php54Var[] Arguments;
		public Php54Var ReturnValue = Php54Var.FromNull();

		static readonly public Php54Scope Methods = new Php54Scope(null);

		public Php54Scope(Php54Runtime Php54Runtime)
		{
			this.Runtime = Php54Runtime;
		}

		public Php54Scope NewScope()
		{
			return new Php54Scope(Runtime);
		}

		public void SetArgumentCount(int Count)
		{
			Arguments = new Php54Var[Count];
		}

		public void SetArgument(int Index, Php54Var Value)
		{
			Arguments[Index] = Value;
		}

		public void LoadArgument(string Name, int Index)
		{
			Variables[Name] = GetArgument(Index);
		}

		public Php54Var GetArgument(int Index)
		{
			return (Index < Arguments.Length) ? Arguments[Index] : Php54Var.FromNull();
		}

		public void SetReturnValue(Php54Var Value)
		{
			ReturnValue = Value;
		}

		public void SetReturnValueObject(object Value)
		{
			ReturnValue = Php54Var.FromObject(Value);
		}

		public Php54Var GetReturnValue()
		{
			return ReturnValue;
		}

		public void CallFunctionByName(string Name)
		{
			IPhp54Function Method;

			if (!Runtime.FunctionScope.Functions.TryGetValue(Name, out Method))
			{
				throw(new KeyNotFoundException("Can't find function '" + Name + "'"));
			}
			Method.Execute(this);
		}

		public Php54Var GetVariable(string Name)
		{
			if (!Variables.ContainsKey(Name)) Variables[Name] = Php54Var.FromNull();
			return Variables[Name];
		}

		public Php54Var GetConstant(string Name)
		{
			switch (Name)
			{
				case "PHP_MAJOR_VERSION": return Php54Var.FromInt(CoreFunctions.PHP_MAJOR_VERSION);
				case "PHP_MINOR_VERSION": return Php54Var.FromInt(CoreFunctions.PHP_MINOR_VERSION);
				case "PHP_RELEASE_VERSION": return Php54Var.FromInt(CoreFunctions.PHP_RELEASE_VERSION);
				case "PHP_EXTRA_VERSION": return Php54Var.FromString(CoreFunctions.PHP_EXTRA_VERSION);
				case "PHP_VERSION": return Php54Var.FromString(CoreFunctions.PHP_VERSION);
				case "PHP_VERSION_ID": return Php54Var.FromInt(CoreFunctions.PHP_VERSION_ID);
				case "PHP_ZTS": return Php54Var.FromInt(1); // Is Thread Safe
				case "PHP_DEBUG": return Php54Var.FromInt(0); // No debug
				case "PHP_MAXPATHLEN": return Php54Var.FromInt(260);
				case "PHP_OS": return Php54Var.FromString("WINNT");
				case "PHP_SAPI": return Php54Var.FromString("cli"); // The Server API for this build of PHP. Available since PHP 4.2.0. See also php_sapi_name().
				case "PHP_EOL": return Php54Var.FromString("\r\n");
				case "PHP_INT_MAX": return Php54Var.FromInt(int.MaxValue);
				case "PHP_INT_SIZE": return Php54Var.FromInt(sizeof(int));
				case "DEFAULT_INCLUDE_PATH": return Php54Var.FromString(".");
			}
			return Runtime.ConstantScope.GetVariable(Name);
		}
	}
}
