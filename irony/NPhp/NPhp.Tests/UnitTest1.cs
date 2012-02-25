using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPhp.Runtime;

namespace NPhp.Tests
{
	[TestClass]
	public partial class UnitTest1
	{
		[TestMethod]
		public void PhpRaw0()
		{
			Assert.AreEqual("text", RunFileAndCaptureOutput(@"text"));
		}

		[TestMethod]
		public void PhpRaw1()
		{
			Assert.AreEqual("HelloWorld", RunFileAndCaptureOutput(@"Hello<?php ?>World"));
		}

		[TestMethod]
		public void PhpRaw2()
		{
			Assert.AreEqual("Hello", RunFileAndCaptureOutput(@"Hello<?php "));
		}

		[TestMethod]
		public void PhpRaw3a()
		{
			Assert.AreEqual("abcd", RunFileAndCaptureOutput(@"a<?php echo 'b'; ?>c<?php echo 'd';"));
		}

		[TestMethod]
		public void PhpRaw3b()
		{
			Assert.AreEqual("0123", RunFileAndCaptureOutput(@"0<?php echo 1; ?>2<?php echo 3;"));
		}

		[TestMethod]
		public void ForContinue()
		{
			Assert.AreEqual("13579", RunCodeAndCaptureOutput(@"
				for ($n = 1; $n <= 10; $n++) {
					if (!($n % 2)) continue;
					echo $n;
				}
			"));
		}

		[TestMethod]
		public void StringUnquote()
		{
			Assert.AreEqual("\n\r\t\\", RunCodeAndCaptureOutput("echo \"\\n\\r\\t\\\\\";"));
		}

		[TestMethod]
		public void StringInterpolation1()
		{
			Assert.AreEqual("01234", RunCodeAndCaptureOutput(@"
				$a = 1;
				$b = 3;
				echo ""0{$a}2{$b}4"";
			"));
		}

		[TestMethod]
		public void StringInterpolation2()
		{
			Assert.AreEqual("0(12)3", RunCodeAndCaptureOutput(@"
				$a = 1;
				$b = 2;
				echo ""0($a$b)3"";
			"));
		}

		[TestMethod]
		public void StringInterpolation3()
		{
			Assert.AreEqual("12", RunCodeAndCaptureOutput(@"
				$a = 1;
				$b = 2;
				echo ""$a$b"";
			"));
		}

		[TestMethod]
		public void StringInterpolation4()
		{
			Assert.AreEqual("1", RunCodeAndCaptureOutput(@"
				$a = 1;
				$b = 2;
				echo ""$a"";
			"));
		}

		[TestMethod]
		public void ForBreak()
		{
			Assert.AreEqual("98765", RunCodeAndCaptureOutput(@"
				for ($n = 9; $n > 0; $n--) {
					if ($n < 5) break;
					echo $n;
				}
			"));
		}

		[TestMethod]
		public void SimpleEchoExpressionTest()
		{
			Assert.AreEqual("152", RunCodeAndCaptureOutput(@"
				echo(1+(2+3)*2)/11;
				echo 5;
				echo 2;
			"));
		}

		[TestMethod]
		public void SimpleEchoWithoutSemicolon()
		{
			Assert.AreEqual("Hello", RunFileAndCaptureOutput(@"<?php echo 'Hello'?>"));
		}

		[TestMethod]
		public void SimpleForeachTest()
		{
			Assert.AreEqual("123", RunCodeAndCaptureOutput(@"
				foreach ([1,2,3] as $v) echo $v;
			"));
		}

		[TestMethod]
		public void SimpleForeachKeyValueTest()
		{
			Assert.AreEqual("01:12:23:", RunCodeAndCaptureOutput(@"
				foreach ([1,2,3] as $k => $v) echo $k . $v . ':';
			"));
		}

		[TestMethod]
		public void PairForeachKeyValueTest()
		{
			Assert.AreEqual("test1:22:13:34:", RunCodeAndCaptureOutput(@"
				foreach (['test' => 1, 2 => 2, 1 => 3, 3 => 4] as $k => $v) echo $k . $v . ':';
			"));
		}

		[TestMethod]
		public void AssocArrayRepeatKeyTest()
		{
			Assert.AreEqual("[2]{\"1\":2}", RunCodeAndCaptureOutput(@"
				echo JsOn_EncodE(array(0 => 1, 0 => 2));
				echo JSon_ENCODE(array(1 => 1, 1 => 2));
			"));
		}
		
		[TestMethod]
		public void SimpleIfTest()
		{
			Assert.AreEqual("71", RunCodeAndCaptureOutput(@"
				echo 7;
				//echo 2;
				if (0) {
					echo 0;
				}
				if (1) {
					echo 1;
				}
			"));
		}

		[TestMethod]
		public void SimpleIfElseTest()
		{
			Assert.AreEqual("2973", RunCodeAndCaptureOutput(@"
				if (0) {
					echo 1;
				} else {
					echo 2;
				}
				if (1) echo 9;
				if (0) echo 8;
				echo 7;
				if (1) {
					echo 3;
				} else {
					echo 4;
				}
			"));
		}

		[TestMethod]
		public void UndefinedVarUse()
		{
			Assert.AreEqual("", RunCodeAndCaptureOutput(@"
				echo $a;
			"));
		}

		[TestMethod]
		public void SimpleVarUse()
		{
			Assert.AreEqual("3", RunCodeAndCaptureOutput(@"
				$a = 3;
				echo $a;
			"));
		}

		[TestMethod]
		public void UnaryOperation()
		{
			Assert.AreEqual("-3", RunCodeAndCaptureOutput(@"
				$a = -(1 + 2);
				echo $a;
			"));
		}

		[TestMethod]
		public void Concat()
		{
			Assert.AreEqual("test-3::1:", RunCodeAndCaptureOutput(@"
				$a = -(1 + 2);
				$b = false;
				$c = true;
				$d = null;
				echo 'test'.$a.':'.$b.':'.$c.':'.$d;
			"));
		}

		[TestMethod]
		public void Var1Use()
		{
			Assert.AreEqual("4yes", RunCodeAndCaptureOutput(@"
				$a = (1 + 2);
				if ($a == 3) {
					echo $a + 1;
				} else {
					echo 'no';
				}
				if (($a != 2) && ($b != 4)) {
					echo 'yes';
				} else {
					echo 'no';
				}
			"));
		}
		
		[TestMethod]
		public void SimpleWhile()
		{
			Assert.AreEqual("987654321", RunCodeAndCaptureOutput(@"
				$n = 9;
				while ($n > 0) {
					echo $n;
					$n = $n - 1;
				}
			"));
		}

		[TestMethod]
		public void SimpleFor()
		{
			Assert.AreEqual("0123456789", RunCodeAndCaptureOutput(@"
				for ($n = 0; $n < 10; $n = $n + 1) echo $n;
			"));
		}

		[TestMethod]
		public void ForWithPostIncrement()
		{
			Assert.AreEqual("0123456789", RunCodeAndCaptureOutput(@"
				for ($n = 0; $n < 10; $n++) echo $n;
			"));
		}

		[TestMethod]
		public void PartialFor()
		{
			Assert.AreEqual("0123456789", RunCodeAndCaptureOutput(@"
				$n = 0;
				for (; $n < 10;) {
					echo $n++;
				}
			"));
		}

		[TestMethod]
		public void AddStrings()
		{
			Assert.AreEqual("10", RunCodeAndCaptureOutput(@"
				echo '1test2' + 4 + '2demo' + 3;
			"));
		}

		[TestMethod]
		public void PostIncrementAndDecrement()
		{
			Assert.AreEqual("0130-1-3", RunCodeAndCaptureOutput(@"
				$n = 0;
				echo $n++;
				echo $n++;
				echo ++$n;

				$n = 0;
				echo $n--;
				echo $n--;
				echo --$n;
			"));
		}

		[TestMethod]
		public void EvalTest()
		{
			Assert.AreEqual("1", RunCodeAndCaptureOutput(@"
				eval('echo 1;');
			"));
		}

		[TestMethod]
		public void SimpleFunctionTest()
		{
			Assert.AreEqual("1234", RunCodeAndCaptureOutput(@"
				echo 1;
				function test() {
					echo 3;
				}
				echo 2;
				test();
				echo 4;
			"));
		}

		[TestMethod]
		public void FunctionTest()
		{
			Assert.AreEqual("3", RunCodeAndCaptureOutput(@"
				$a = -1;
				$b = -2;
				function add($a, $b) {
					echo $a + $b;
				}
				add(1, 2);
			"));
		}

		[TestMethod]
		public void RefTest()
		{
			Assert.AreEqual("2", RunCodeAndCaptureOutput(@"
				$a = 1;
				$b = &$a;
				$b = 2;
				echo $a;
			"));
		}

		[TestMethod]
		public void FunctionReturnType()
		{
			Assert.AreEqual("1a1Array", RunCodeAndCaptureOutput(@"
				function ret_int() { return 1; }
				function ret_str() { return 'a'; }
				function ret_bit() { return true; }
				function ret_nil() { return null; }
				function ret_arr() { return [1,2,3]; }
				echo ret_int();
				echo ret_str();
				echo ret_bit();
				echo ret_nil();
				echo ret_arr();
			"));
		}

		[TestMethod]
		public void FunctionChaining()
		{
			Assert.AreEqual("abc", RunCodeAndCaptureOutput(@"
				function a() { return 'a'; }
				function b($a) { return $a.'b'; }
				function c($b) { return $b.'c'; }
				echo c(b(a()));
			"));
		}

		[TestMethod]
		public void ParameterOrderIsLeftToRight()
		{
			Assert.AreEqual("ab", RunCodeAndCaptureOutput(@"
				function a() { echo 'a'; }
				function b() { echo 'b'; }
				function c($a, $b) { }
				c(a(), b());
			"));
		}

		[TestMethod]
		public void ParameterOrderIsLeftToRightPlusReturn()
		{
			Assert.AreEqual("abAB", RunCodeAndCaptureOutput(@"
				function a() { echo 'a'; return 'A'; }
				function b() { echo 'b'; return 'B'; }
				function c($a, $b) { return $a . $b; }
				echo c(a(), b());
			"));
		}

		[TestMethod]
		public void StringApiTest()
		{
			Assert.AreEqual("el|ello|5", RunCodeAndCaptureOutput(@"
				echo substr('hello', 1, 2);
				echo '|';
				echo substr('hello', 1);
				echo '|';
				echo strlen('hello');
			"));
		}

		[TestMethod]
		public void HexDecTest()
		{
			Assert.AreEqual("f2f2f2f2", RunCodeAndCaptureOutput(@"
				function F($X, $Y, $Z){
					$X = hexdec($X);
					$Y = hexdec($Y);
					$Z = hexdec($Z);
					$calc = (($X & $Y) | ((~ $X) & $Z)); // X AND Y OR NOT X AND Z
					return  $calc; 
				}
				echo dechex(F('1f1f1f1f', '32323232', 'e4e4e4e4'));
			"));
		}

		[TestMethod]
		public void HexLiteralsTest()
		{
			Assert.AreEqual("f2f2f2f2", RunCodeAndCaptureOutput(@"
				function _F($X, $Y, $Z){
					return  (($X & $Y) | ((~ $X) & $Z)); // X AND Y OR NOT X AND Z
				}
				echo dechex(_F(0x1f1f1f1f, 0x32323232, 0xe4e4e4e4));
			"));
		}

		[TestMethod]
		public void NonDecimalLiteralsTest()
		{
			Assert.AreEqual("63:63:511:", RunCodeAndCaptureOutput(@"
				echo 0x3F . ':';
				echo 0x3f . ':';
				echo 0777 . ':';
				//echo 0b10101 . ':';
			"));
		}

		[TestMethod]
		public void SpecialTokens()
		{
			Assert.AreEqual("f3", RunCodeAndCaptureOutput(@"
				function f() {
					return __FUNCTION__ . __LINE__; 
				}
				echo f();
			"));
		}

		[TestMethod]
		public void Constants()
		{
			Assert.AreEqual("TEST:20", RunCodeAndCaptureOutput(@"
				define('TEST', 10);
				function a() {
					return TEST; 
				}
				echo 'TEST:'.(a()+TEST);
			"));
		}

		[TestMethod]
		public void GettypeTest()
		{
			Assert.AreEqual(":integer:string:double", RunCodeAndCaptureOutput(@"
				echo ':'.gettype(1);
				echo ':'.gettype('test');
				echo ':'.gettype(99999999*39);
			"));
		}

		[TestMethod]
		public void EmptyArrayTest()
		{
			Assert.AreEqual("[][]", RunCodeAndCaptureOutput(@"
				echo json_encode(array());
				echo json_encode([]);
			"));
		}

		[TestMethod]
		public void SimpleArrayTest()
		{
			Assert.AreEqual("[1,\"test\",2]", RunCodeAndCaptureOutput(@"
				echo json_encode(array(1,'test',2));
			"));
		}

		[TestMethod]
		public void SimpleArray2Test()
		{
			Assert.AreEqual("[1,\"test\",2]", RunCodeAndCaptureOutput(@"
				echo json_encode([1,'test',2]);
			"));
		}

		[TestMethod]
		public void ComplexArrayTest()
		{
			Assert.AreEqual("[1,[\"test\",[],2,[false]],3]", RunCodeAndCaptureOutput(@"
				echo json_encode([1,['test',[],2,[false]],3]);
			"));
		}

		[TestMethod]
		public void AssociativeArray()
		{
			Assert.AreEqual("{\"0\":1,\"2\":\"test\",\"3\":2}{\"0\":1,\"2\":\"test\",\"3\":2}{\"0\":1,\"2a\":\"test\",\"1\":2}", RunCodeAndCaptureOutput(@"
				echo json_encode(array(1,2=>'test',2));
				echo json_encode(array(1,'2'=>'test',2));
				echo json_encode(array(1,'2a'=>'test',2));
			"));
		}

		[TestMethod]
		public void OrdChr()
		{
			Assert.AreEqual("64", RunCodeAndCaptureOutput(@"
				echo ord(chr(0x40));
			"));
		}

		[TestMethod]
		public void SimpleArrayAccess()
		{
			Assert.AreEqual("e[3,2,0,4]", RunCodeAndCaptureOutput(@"
				$array = [1, 2, 3, 4];
				$array[0] = 3;
				$array[2] = 0;
				$str = 'hello';
				echo $str[1];
				echo json_encode($array);
			"));
		}

		[TestMethod]
		public void ArrayAccessTest()
		{
			Assert.AreEqual("[1,[2,[3,[0,[5,6]]]]]", RunCodeAndCaptureOutput(@"
				$array = [1, [2, [3, [4, [5, 6]]]]];
				$array[1][1][1][0] = 0;
				echo json_encode($array);
			"));
		}
	}

	public partial class UnitTest1
	{
		static Php54Runtime Runtime;
		static Php54FunctionScope FunctionScope;

		[ClassInitialize]
		static public void PrepareRuntime(TestContext Context)
		{
			FunctionScope = new Php54FunctionScope();
			FunctionScope.LoadAllNativeFunctions();
			Runtime = new Php54Runtime(FunctionScope);
		}

		[TestInitialize]
		public void PrepareRuntime()
		{
		}

		static private string RunFileAndCaptureOutput(string Code, Dictionary<string, Php54Var> Variables = null)
		{
			var Out = TestUtils.CaptureOutput(() =>
			{
				var Method = Runtime.CreateMethodFromPhpFile(Code);
				var Scope = new Php54Scope(Runtime);
				if (Variables != null)
				{
					foreach (var Pair in Variables)
					{
						Php54Var.Assign(Scope.GetVariable(Pair.Key), Pair.Value);
					}
				}
				Method(Scope);
			});
			return Out;
		}

		static private string RunCodeAndCaptureOutput(string Code, Dictionary<string, Php54Var> Variables = null)
		{
			return RunFileAndCaptureOutput("<?php " + Code + " ?>", Variables);
		}

	}

}
