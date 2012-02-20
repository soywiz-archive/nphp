using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace NPhp.Codegen
{
	public partial class SafeILGenerator
	{
		private ILGenerator ILGenerator;
		TypeStackClass TypeStack = new TypeStackClass();
		List<Label> Labels = new List<Label>();
		bool OverflowCheck = false;
		bool DoEmit = true;
		bool TrackStack = true;

		public enum UnaryOperatorEnum
		{
			Negate,
			Not
		}

		public enum PointerAttributesSet
		{
			Unaligned = 1,
			Volatile = 2,
		}

		public enum BinaryOperatorEnum
		{
			AdditionSigned,
			AdditionUnsigned,
			And,
			DivideSigned,
			DivideUnsigned,
			MultiplySigned,
			MultiplyUnsigned,
			Or,
			RemainingSigned,
			RemainingUnsigned,
			ShiftLeft,
			ShiftRightSigned,
			ShiftRightUnsigned,
			SubstractionSigned,
			SubstractionUnsigned,
		}

		public enum BinaryComparison2Enum
		{
			Equals,
			GreaterThanSigned,
			GreaterThanUnsigned,
			LessThanSigned,
			LessThanUnsigned,
		}

		public enum BinaryComparisonEnum
		{
			Equals,
			NotEquals,
			GreaterOrEqualSigned,
			GreaterOrEqualUnsigned,
			GreaterThanSigned,
			GreaterThanUnsigned,
			LessOrEqualSigned,
			LessOrEqualUnsigned,
			LessThanSigned,
			LessThanUnsigned,
		}

		public enum UnaryComparisonEnum
		{
			False,
			True,
		}

		public class TypeStackClass
		{
			//public List<Type> List;
			private LinkedList<Type> Stack = new LinkedList<Type>();

			public int Count
			{
				get
				{
					return Stack.Count;
				}
			}

			public void Pop(int Count)
			{
				while (Count-- > 0) Pop();
			}

			public Type Pop()
			{
				try
				{
					return Stack.First.Value;
				}
				finally
				{
					Stack.RemoveFirst();
				}
			}

			public void Push(Type Type)
			{
				Stack.AddFirst(Type);
			}

			public TypeStackClass Clone()
			{
				var NewTypeStack = new TypeStackClass();
				NewTypeStack.Stack = new LinkedList<Type>(Stack);
				return NewTypeStack;
			}

			public Type GetLastest()
			{
				return Stack.First.Value;
			}

			public Type[] GetLastestList(int Count)
			{
				return Stack.Take(Count).Reverse().ToArray();
			}
		}

		public TypeStackClass CaptureStackInformation(Action Action)
		{
			var OldTypeStack = TypeStack;
			var NewTypeStack = TypeStack.Clone();
			var OldDoEmit = DoEmit;
			TypeStack = NewTypeStack;
			DoEmit = false;
			try
			{
				Action();
				return NewTypeStack;
			}
			finally
			{
				DoEmit = OldDoEmit;
				TypeStack = OldTypeStack;
			}
		}

		public class Label
		{
			private SafeILGenerator SafeILGenerator;
			internal System.Reflection.Emit.Label ReflectionLabel { get; private set; }
			private bool Marked;

			internal Label(SafeILGenerator SafeILGenerator)
			{
				this.SafeILGenerator = SafeILGenerator;
				this.ReflectionLabel = SafeILGenerator.ILGenerator.DefineLabel();
			}

			public void Mark()
			{
				SafeILGenerator.ILGenerator.MarkLabel(ReflectionLabel);
				Marked = true;
			}
		}

		public Label CreateLabel()
		{
			var Label = new Label(this);
			Labels.Add(Label);
			return Label;
		}

		public SafeILGenerator(ILGenerator ILGenerator)
		{
			this.ILGenerator = ILGenerator;
		}

		public void DoOverflowCheck(Action Action)
		{
			var OldOverflowCheck = OverflowCheck;
			OverflowCheck = true;
			try
			{
				Action();
			}
			finally
			{
				OverflowCheck = OldOverflowCheck;
			}
		}
	}
}
