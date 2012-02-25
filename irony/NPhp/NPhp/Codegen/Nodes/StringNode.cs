using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Parsing;
using System.Diagnostics;
using NPhp.Runtime;
using NPhp.Common;
using NPhp.LanguageGrammar;

namespace NPhp.Codegen.Nodes
{
	public class StringNode : Node
	{
		bool Interpolate;
		String UnquotedString;

		public override void Init(AstContext context, ParseTreeNode parseNode)
		{
			var StringWithQuotes = parseNode.FindTokenAndGetText();
			UnquotedString = Php54Utils.FullStringUnquote(StringWithQuotes);
			Interpolate = (StringWithQuotes[0] == '"');
		}

		public override void PreGenerate(NodeGenerateContext Context)
		{
		}

		public override void Generate(NodeGenerateContext Context)
		{
			// @TODO: Temporal solution. It should have its own grammar!
			if (Interpolate)
			{
				int LastPosition = 0;
				int CurrentPosition = 0;
				int InterpolatedStackElements = 0;
				//var Parts = new List<string>();

				Action<int> EmitString = (EndPosition) =>
				{
					if (LastPosition != EndPosition)
					{
						var Part = UnquotedString.Substr(LastPosition, EndPosition - LastPosition);
						Context.MethodGenerator.Push(Part);
						Context.MethodGenerator.Call((Func<string, Php54Var>)Php54Var.FromString);
						LastPosition = EndPosition;
						InterpolatedStackElements++;
					}
				};

				Action<string> EmitVariable = (VariableName) =>
				{
					//Console.WriteLine(VariableName);
					GetVariableNode.GetVariable(Context, VariableName);
					InterpolatedStackElements++;
				};

				for (CurrentPosition = 0; CurrentPosition < UnquotedString.Length; CurrentPosition++)
				{
					var Char = UnquotedString[CurrentPosition];
					if (Char == '{')
					{
						if (UnquotedString[CurrentPosition + 1] == '$')
						{
							EmitString(CurrentPosition);
							CurrentPosition = CurrentPosition + 2;
							int m = CurrentPosition;
							bool Found = false;
							for (; CurrentPosition < UnquotedString.Length; CurrentPosition++)
							{
								if (UnquotedString[CurrentPosition] == '}')
								{
									Found = true;
									break;
								}
							}
							if (!Found) throw (new InvalidOperationException());
							var Variable = UnquotedString.Substr(m, CurrentPosition - m);
							EmitVariable(Variable);
							LastPosition = CurrentPosition + 1;
						}
					}
					else if (Char == '$')
					{
						EmitString(CurrentPosition);
						CurrentPosition = CurrentPosition + 1;
						int m = CurrentPosition;
						for (; CurrentPosition < UnquotedString.Length; CurrentPosition++)
						{
							if (!PhpVariableTerminal.IsValidCharacter(UnquotedString[CurrentPosition])) break;
						}
						var Variable = UnquotedString.Substr(m, CurrentPosition - m);
						EmitVariable(Variable);
						CurrentPosition--;

						LastPosition = CurrentPosition + 1;
					}
					else
					{

					}
				}

				EmitString(CurrentPosition);

				InterpolatedStackElements--;
				for (int n = 0; n < InterpolatedStackElements; n++)
				{
					Context.MethodGenerator.Call((Func<Php54Var, Php54Var, Php54Var>)Php54Var.Concat);
				}
			}
			else
			{
				Context.MethodGenerator.Push(UnquotedString);
				Context.MethodGenerator.Call((Func<string, Php54Var>)Php54Var.FromString);
			}
			//Console.WriteLine("Value: '{0}'", Value);
		}
	}
}
