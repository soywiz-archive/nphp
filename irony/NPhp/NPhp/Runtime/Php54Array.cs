using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPhp.Common;

namespace NPhp.Runtime
{
	public sealed class Php54Array
	{
		private int LastNumericIndex = -1;
		private List<object> Keys = new List<object>();
		private List<Php54Var> Values = new List<Php54Var>();
		private Dictionary<object, Php54Var> Dictionary = new Dictionary<object, Php54Var>();
		public bool PureArray { get; private set; }

		public Php54Array()
		{
			PureArray = true;
		}

		public IEnumerable<KeyValuePair<object, Php54Var>> GetEnumerator()
		{
			var KeysEnumerator = Keys.GetEnumerator();
			var ValuesEnumerator = Values.GetEnumerator();
			while (KeysEnumerator.MoveNext() && ValuesEnumerator.MoveNext())
			{
				yield return new KeyValuePair<object, Php54Var>(KeysEnumerator.Current, ValuesEnumerator.Current);
			}
		}

		private void UpdateLastNumericIndex(Php54Var Key)
		{
			if ((Key.Type != Php54Var.TypeEnum.Int) && (Key.Type != Php54Var.TypeEnum.String)) throw (new InvalidOperationException("Keys must be integers or strings only"));

			if ((Key.Type == Php54Var.TypeEnum.Int) || (Php54Utils.IsCompletelyNumeric(Key.StringValue)))
			{
				var IntKey = Key.IntegerValue;
				if (IntKey > LastNumericIndex)
				{
					LastNumericIndex = IntKey;
					PureArray = false;
				}
				else
				{
					//PureArray = PureArray; // not changed
				}
			}
			else
			{
				PureArray = false;
			}
		}

		public void AddElement(Php54Var Value)
		{
			var Key = ++LastNumericIndex;
			Keys.Add(Key);
			Values.Add(Value);
			Dictionary[Key] = Value;
		}

		public void AddPair(Php54Var Key, Php54Var Value)
		{
			UpdateLastNumericIndex(Key);
			Keys.Add(Key);
			Values.Add(Value);
			Dictionary[Key] = Value;
		}
	}
}
