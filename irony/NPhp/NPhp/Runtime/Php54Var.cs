using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace NPhp.Runtime
{
	public sealed partial class Php54Var
	{
		static public readonly Php54Var Methods = new Php54Var(null, TypeEnum.Null);

		/// <summary>
		/// This type. Can be a reference.
		/// </summary>
		private TypeEnum ThisType;

		/// <summary>
		/// 
		/// </summary>
		private dynamic ThisDynamicValue;

		/// <summary>
		/// Dynamic value of a reference.
		/// </summary>
		public dynamic ReferencedDynamicValue
		{
			private set
			{
				if (IsReference)
				{
					//Debug.Assert(this._DynamicValue != this);
					((Php54Var)this.ThisDynamicValue).ReferencedDynamicValue = value;
				}
				else
				{
					this.ThisDynamicValue = value;
				}
			}
			get
			{
				if (IsReference)
				{
					//Debug.Assert(this._DynamicValue != this);
					return ((Php54Var)this.ThisDynamicValue).ReferencedDynamicValue;
				}
				else
				{
					return this.ThisDynamicValue;
				}
			}
		}

		/// <summary>
		/// Gets the type of the resolved references. Can't be a reference.
		/// </summary>
		public TypeEnum ReferencedType
		{
			get
			{
				if (IsReference)
				{
					return ReferencedValue.ReferencedType;
				}
				else
				{
					return ThisType;
				}
			}
		}

		public bool IsReference
		{
			get
			{
				return ThisType == TypeEnum.Reference;
			}
		}

		public enum TypeEnum
		{
			Reference,
			Null,
			Array,
			Bool,
			Int,
			Double,
			String,
		}

		static private TypeEnum TypeToTypeEnum(Type Type)
		{
			if (Type == typeof(int)) return TypeEnum.Int;
			if (Type == typeof(bool)) return TypeEnum.Bool;
			if (Type.IsArray) return TypeEnum.Array;
			if (Type == typeof(String)) return TypeEnum.String;
			if (Type == typeof(double)) return TypeEnum.Double;
			if (Type == typeof(DBNull)) return TypeEnum.Null;
			throw(new NotImplementedException());
		}

		static private TypeEnum CombineTypes(TypeEnum Type1, TypeEnum Type2)
		{
			return (TypeEnum)Math.Max((int)Type1, (int)Type2);
		}

		public double GetDoubleOrDefault(double DefaultValue)
		{
			if (ReferencedDynamicValue == null) return DefaultValue;
			return DoubleValue;
		}

		public string GetStringOrDefault(string DefaultValue)
		{
			if (ReferencedDynamicValue == null) return DefaultValue;
			return StringValue;
		}

		public int GetIntegerOrDefault(int DefaultValue)
		{
			if (ReferencedDynamicValue == null) return DefaultValue;
			return IntegerValue;
		}

		public bool GetBooleanOrDefault(bool DefaultValue)
		{
			if (ReferencedDynamicValue == null) return DefaultValue;
			return BoolValue;
		}

		private Php54Var()
		{
		}

		private Php54Var(dynamic Value, TypeEnum Type)
		{
			this.ThisDynamicValue = Value;
			this.ThisType = Type;
			//this.Type = (Value != null) ? Value.GetType() : null;
		}

		static public Php54Var CreateRef(Php54Var ReferencedValue)
		{
			return new Php54Var(ReferencedValue, TypeEnum.Reference);
		}

		static public Php54Var FromInt(int Value)
		{
			return new Php54Var(Value, TypeEnum.Int);
		}

		static public Php54Var FromArray(Php54Array Value)
		{
			return new Php54Var(Value, TypeEnum.Array);
		}

		static public Php54Var FromNewArray()
		{
			return new Php54Var(new Php54Array(), TypeEnum.Array);
		}

		static public Php54Var FromString(string Value)
		{
			return new Php54Var(Value, TypeEnum.String);
		}

		static public Php54Var FromTrue()
		{
			return FromBool(true);
		}

		static public Php54Var FromFalse()
		{
			return FromBool(false);
		}

		static public Php54Var FromBool(bool Value)
		{
			return new Php54Var(Value, TypeEnum.Bool);
		}

		static public Php54Var FromObject(object Object)
		{
			return new Php54Var(Object, TypeToTypeEnum(Object.GetType()));
		}

		static public Php54Var FromNull()
		{
			return new Php54Var(null, TypeEnum.Null);
		}
		
		public bool ToBool()
		{
			return BoolValue;
		}

		public int ToInt()
		{
			return IntegerValue;
		}

		public override string ToString()
		{
			return StringValue;
		}

		public void AddElement(Php54Var Entry)
		{
			if (this.ReferencedType != TypeEnum.Array) throw(new NotImplementedException());
			this.ArrayValue.AddElement(Entry);
		}

		public void AddKeyValuePair(Php54Var Key, Php54Var Value)
		{
			if (this.ReferencedType != TypeEnum.Array) throw (new NotImplementedException());
			this.ArrayValue.AddPair(Key, Value);
		}

		public IEnumerator<KeyValuePair<Php54Var, Php54Var>> GetArrayIterator()
		{
			if (this.ReferencedType != TypeEnum.Array)
			{
				return (new KeyValuePair<Php54Var, Php54Var>[] { }).Select(Item => Item).GetEnumerator();
				//throw (new NotImplementedException());
			}
			return (this.ReferencedDynamicValue as Php54Array).GetEnumerator().Select(Item => Item).GetEnumerator();
		}

		public Php54Var Access(Php54Var Item)
		{
			switch (this.ReferencedType)
			{
				case TypeEnum.Array:
					return this.ArrayValue.GetElementByKey(Item);
				case TypeEnum.String:
					return Php54Var.FromString("" + this.StringValue[Item.IntegerValue]);
				case TypeEnum.Null:
					return this;
			}

			throw (new NotImplementedException());
		}

		static public bool IteratorMoveNext(IEnumerator<KeyValuePair<Php54Var, Php54Var>> Iterator)
		{
			return Iterator.MoveNext();
		}

		static public Php54Var IteratorGetCurrentValue(IEnumerator<KeyValuePair<Php54Var, Php54Var>> Iterator)
		{
			return Iterator.Current.Value;
		}

		static public Php54Var IteratorGetCurrentKey(IEnumerator<KeyValuePair<Php54Var, Php54Var>> Iterator)
		{
			return Iterator.Current.Key;
		}

		static public bool operator ==(Php54Var Left, Php54Var Right)
		{
			return (Left.ReferencedType == Right.ReferencedType) && (Left.ReferencedDynamicValue == Right.ReferencedDynamicValue);
		}

		static public bool operator !=(Php54Var Left, Php54Var Right)
		{
			return !(Left == Right);
		}

		public override bool Equals(object that)
		{
			if (that.GetType() != this.GetType()) return false;
			return this == (Php54Var)that;
		}

		public override int GetHashCode()
		{
			return ReferencedDynamicValue.GetHashCode();
		}

		public Php54Var Clone()
		{
			if (this.ReferencedType == TypeEnum.Array) return Php54Var.FromArray(this.ArrayValue.Clone());
			return new Php54Var(this.ThisDynamicValue, this.ThisType);
		}
	}
}
