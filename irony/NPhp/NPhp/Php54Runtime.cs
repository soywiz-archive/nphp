using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Irony.Ast;

namespace NPhp
{
	/*
	public class Php54VarInt : Php54Var
	{
		private int Value;

		public Php54VarInt(int Value) { this.Value = Value; }
		override public dynamic DynamicValue { get { return Value; } }
		override public string StringValue { get { return Value.ToString(); } }
		override public bool BoolValue { get { return (Value != 0); } }
	}

	public class Php54VarString : Php54Var
	{
		private string Value;

		public Php54VarString(string Value) { this.Value = Value; }
		override public dynamic DynamicValue { get { return Value; } }
		override public string StringValue { get { return Value; } }
		override public bool BoolValue { get { return (Value != ""); } }
	}

	public class Php54VarBool : Php54Var
	{
		private bool Value;

		public Php54VarBool(bool Value) { this.Value = Value; }
		override public dynamic DynamicValue { get { return Value; } }
		override public string StringValue { get { return Value ? "1" : ""; } }
		override public bool BoolValue { get { return (Value); } }
	}

	public class Php54VarNull : Php54Var
	{
		private object Value;

		public Php54VarNull() { this.Value = null; }
		override public dynamic DynamicValue { get { return Value; } }
		override public string StringValue { get { return ""; } }
		override public bool BoolValue { get { return false; } }
	}
	*/

	public sealed class Php54Var
	{
		//private Type Type;
		private dynamic DynamicValue;
		static private readonly Type BoolType = typeof(bool);
		private Type Type
		{
			get
			{
				if (DynamicValue == null) return null;
				return DynamicValue.GetType();
			}
		}

		/*
		public dynamic DynamicValue
		{
			get
			{
				return Value;
			}
		}
		*/
		public string StringValue
		{
			get
			{
				if (Type == null) return "";
				if (Type == BoolType) return (DynamicValue) ? "1" : "";
				return DynamicValue.ToString();
			}
		}
		public bool BoolValue
		{
			get
			{
				if (Type == BoolType) return DynamicValue;
				return (DynamicValue != 0);
			}
		}
		public double DoubleValue
		{
			get
			{
				if (Type == typeof(double)) return DynamicValue;
				var Str = StringValue;
				int value = 0;
				for (int n = 0; n < Str.Length; n++)
				{
					if (Str[n] >= '0' && Str[n] <= '9')
					{
						value *= 10;
						value += Str[n] - '0';
					}
					else
					{
						break;
					}
				}
				//return double.Parse(StringValue);
				return value;
			}
		}

		public Php54Var(dynamic Value)
		{
			this.DynamicValue = Value;
			//this.Type = (Value != null) ? Value.GetType() : null;
		}

		static public Php54Var FromInt(int Value)
		{
			return new Php54Var(Value);
		}

		static public Php54Var FromString(string Value)
		{
			return new Php54Var(Value);
		}

		static public Php54Var FromTrue()
		{
			return new Php54Var(true);
		}

		static public Php54Var FromFalse()
		{
			return new Php54Var(false);
		}

		static public Php54Var FromNull()
		{
			return new Php54Var(null);
		}

		static public Php54Var Add(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.DoubleValue + Right.DoubleValue);
		}

		static public Php54Var Concat(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.StringValue + Right.StringValue);
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

		static public Php54Var UnaryPostInc(Php54Var Left, int Count)
		{
			var Old = Left.DynamicValue;
			Left.DynamicValue = Old + Count;
			return new Php54Var(Old);
		}

		static public Php54Var UnaryPreInc(Php54Var Left, int Count)
		{
			Left.DynamicValue = Left.DynamicValue + Count;
			return new Php54Var(Left.DynamicValue);
		}

		static public Php54Var UnarySub(Php54Var Right)
		{
			return new Php54Var(-Right.DynamicValue);
		}

		public static Php54Var CompareEquals(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.DynamicValue == Right.DynamicValue);
		}

		public static Php54Var CompareGreaterThan(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.DynamicValue > Right.DynamicValue);
		}

		public static Php54Var CompareLessThan(Php54Var Left, Php54Var Right)
		{
			return new Php54Var(Left.DynamicValue < Right.DynamicValue);
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
			//Left.Type = Right.Type;
		}

		static public bool ToBool(Php54Var Variable)
		{
			return Variable.BoolValue;
		}

		public override string ToString()
		{
			return StringValue;
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
			Console.Out.Write(Variable);
		}

		static public void Eval(Php54Scope Scope, Php54Var Variable)
		{
			var Action = Scope.Php54Runtime.CreateMethodFromCode(Variable.StringValue, "eval()");
			Action(Scope);
		}
	}
}
