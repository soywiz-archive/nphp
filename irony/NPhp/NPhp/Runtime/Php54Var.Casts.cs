using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NPhp.Runtime
{
	public sealed partial class Php54Var
	{
		public Php54Var ReferencedValue
		{
			get
			{
				switch (ThisType)
				{
					case TypeEnum.Reference: return (Php54Var)ThisDynamicValue;
					default: throw (new InvalidOperationException());
				}
				
			}
		}

		public string StringValue
		{
			get
			{
				switch (ReferencedType)
				{
					case TypeEnum.Null: return "";
					case TypeEnum.Bool: return (ReferencedDynamicValue) ? "1" : "";
					case TypeEnum.Array: return "Array";
					default: return ReferencedDynamicValue.ToString();
				}
			}
		}

		public object ResourceValue
		{
			get
			{
				switch (ReferencedType)
				{
					case TypeEnum.Resource: return (object)ReferencedDynamicValue;
					default: throw(new NotImplementedException());
				}
			}
		}

		public bool BooleanValue
		{
			get
			{
				switch (ReferencedType)
				{
					case TypeEnum.Null: return false;
					case TypeEnum.Bool: return ReferencedDynamicValue;
					default: return (ReferencedDynamicValue != 0);
				}
			}
		}

		public double DoubleValue
		{
			get
			{
				switch (ReferencedType)
				{
					case TypeEnum.Null: return 0;
					case TypeEnum.Int: return (double)ReferencedDynamicValue;
					case TypeEnum.Double: return ReferencedDynamicValue;
					default:
						{
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
			}
		}

		public int IntegerValue
		{
			get
			{
				switch (ReferencedType)
				{
					case TypeEnum.Null: return 0;
					case TypeEnum.Int: return ReferencedDynamicValue;
					case TypeEnum.Double: return (int)(double)ReferencedDynamicValue;
					default: return (int)DoubleValue;
				}
			}
		}

		public dynamic NumericValue
		{
			get
			{
				switch (ReferencedType)
				{
					case TypeEnum.Null: return 0;
					case TypeEnum.Int: case TypeEnum.Double: return ReferencedDynamicValue;
					default: return DoubleValue;
				}
			}
		}

		public Php54Array ArrayValue
		{
			get
			{
				switch (ReferencedType)
				{
					case TypeEnum.Array: return (Php54Array)ReferencedDynamicValue;
					default: throw (new NotImplementedException());
				}
			}
		}

		public bool CastToBool()
		{
			return BooleanValue;
		}

		public int CastToInt()
		{
			return IntegerValue;
		}

		public double CastToFloat()
		{
			return DoubleValue;
		}

		public string CastToString()
		{
			return StringValue;
		}
	}
}
