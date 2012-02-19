using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;

namespace NPhp
{
	public class Php54Var
	{
		public dynamic DynamicValue;

		public bool BoolValue
		{
			get
			{
				Type Type = DynamicValue.GetType();
				if (Type == typeof(bool)) return DynamicValue;
				return (DynamicValue != 0);
			}
		}

		public Php54Var(dynamic Value)
		{
			this.DynamicValue = Value;
		}

		static public Php54Var FromInt(int Value)
		{
			return new Php54Var(Value);
		}

		static public Php54Var FromString(string Value)
		{
			return new Php54Var(Value);
		}

		static public Php54Var Add(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.DynamicValue + Right.DynamicValue);
		}

		static public Php54Var Sub(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.DynamicValue - Right.DynamicValue);
		}

		static public Php54Var Mul(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.DynamicValue * Right.DynamicValue);
		}

		static public Php54Var Div(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.DynamicValue / Right.DynamicValue);
		}

		static public Php54Var UnaryAdd(Php54Var Right)
		{
			return new Php54Var(+Right.DynamicValue);
		}

		static public Php54Var UnarySub(Php54Var Right)
		{
			return new Php54Var(-Right.DynamicValue);
		}

		public static Php54Var CompareEquals(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.DynamicValue == Right.DynamicValue);
		}

		public static Php54Var CompareNotEquals(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.DynamicValue != Right.DynamicValue);
		}

		public static Php54Var LogicalAnd(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.BoolValue && Right.BoolValue);
		}

		static public void Assign(Php54Var Left, Php54Var Right)
		{
			Left.DynamicValue = Right.DynamicValue;
		}

		static public bool ToBool(Php54Var Variable)
		{
			return Variable.BoolValue;
		}

		public override string ToString()
		{
			if (DynamicValue == null) {
				return "";
			}
			Type Type = DynamicValue.GetType();
			if (Type == typeof(bool))
			{
				if (DynamicValue == false)
				{
					return "";
				}
				if (DynamicValue == true)
				{
					return "1";
				}
			}
			return DynamicValue.ToString();
		}
	}

	public class Php54Scope
	{
		public Php54Runtime Php54Runtime;
		protected Dictionary<string, Php54Var> Variables = new Dictionary<string, Php54Var>();

		static public Php54Scope NullInstance = null;

		public Php54Scope(Php54Runtime Php54Runtime)
		{
			this.Php54Runtime = Php54Runtime;
		}

		public Php54Var GetVariable(string Name)
		{
			if (!Variables.ContainsKey(Name)) Variables[Name] = new Php54Var(null);
			return Variables[Name];
		}
	}

	public class Php54Runtime
	{
		Php54Grammar Grammar;
		LanguageData LanguageData;
		Parser Parser;

		public Php54Runtime()
		{
			Grammar = new Php54Grammar();
			LanguageData = new LanguageData(Grammar);
			Parser = new Parser(LanguageData);
			Parser.Context.TracingEnabled = true;
		}

		public Action<Php54Scope> CreateMethodFromCode(string Code, string File = "<source>", bool DumpTree = false)
		{
			var Tree = Parser.Parse(Code, File);

			//Console.WriteLine(Tree);
			if (Tree.HasErrors())
			{
				var Errors = "";
				foreach (var Message in Tree.ParserMessages)
				{
					Errors += String.Format("Error: {0} at {1}", Message.Message, Message.Location);
				}
				Console.Error.WriteLine(Errors);
				throw (new Exception(Errors));
			}

			if (DumpTree)
			{
				Console.WriteLine(Tree.ToXml());
			}
			//Console.WriteLine("'{0}'", Tree.Root.Term.AstConfig.NodeType);
			//Console.WriteLine("'{0}'", Tree.Root.AstNode);
			var Action = (Tree.Root.AstNode as Node).CreateMethod();
			return Action;
		}

		static public void Echo(Php54Var Variable)
		{
			Console.Write(Variable);
		}
	}
}
