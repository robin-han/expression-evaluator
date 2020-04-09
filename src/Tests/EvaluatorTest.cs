using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Code.Expressions.CSharp.Tests
{
    [TestClass]
    public class EvaluatorTest
    {
        [TestMethod]
        public void MixedOperator()
        {
            Evaluator evaluator = new Evaluator();
            // Binary: +,-,*,/,%
            Assert.AreEqual(7d, evaluator.Evaluate("1 + 2*3"));
            Assert.AreEqual(2d, evaluator.Evaluate("1 + 2 + 3 - 4"));
            Assert.AreEqual(9d, evaluator.Evaluate("(1 + 2)*3"));
            Assert.AreEqual(2d, evaluator.Evaluate("6 / (4 - 1)"));
            Assert.AreEqual(3d, evaluator.Evaluate("(1+(3-1)*4)/3"));
            Assert.AreEqual(3.5, evaluator.Evaluate(".5+2-.5+1.5"));
            Assert.AreEqual(1d, evaluator.Evaluate("-1+2"));
            Assert.AreEqual(0d, evaluator.Evaluate("-1+2-1"));
            Assert.AreEqual(3d, evaluator.Evaluate("3%7"));
            Assert.AreEqual(3.5d, evaluator.Evaluate("3.5%7"));
            Assert.AreEqual("123abc", evaluator.Evaluate("123 + \"abc\""));

            // Unary: +,-,!,~
            Assert.AreEqual(10d, evaluator.Evaluate("+10"));
            Assert.AreEqual(-10d, evaluator.Evaluate("-10"));
            Assert.AreEqual(-101L, evaluator.Evaluate("~100"));
            Assert.AreEqual(2L, evaluator.Evaluate("~-3"));
            Assert.AreEqual(false, evaluator.Evaluate("!true"));
            Assert.AreEqual(true, evaluator.Evaluate("!false"));
            Assert.AreEqual(true, evaluator.Evaluate("!null"));
            Assert.AreEqual(false, evaluator.Evaluate("!10"));
            Assert.AreEqual(false, evaluator.Evaluate("!-10"));
            Assert.AreEqual(true, evaluator.Evaluate("!0"));
            Assert.AreEqual(true, evaluator.Evaluate("!\"\""));
            Assert.AreEqual(false, evaluator.Evaluate("!\"abc\""));
        }

        [TestMethod]
        public void MemberAccess()
        {
            var valueObj = new Dictionary<string, object>();
            valueObj["a"] = 100;
            valueObj["b"] = new { name = "XiaoMing", age = 100 };
            valueObj["c"] = new List<int>() { 1, 2, 3, 4, 5 };
            valueObj["d"] = new { items = new List<int> { 1, 2, 3, 4, 5 } };

            Evaluator evaluator = new Evaluator(new EvaluateContext(valueObj));
            Assert.AreEqual(100, evaluator.Evaluate("a"));
            Assert.AreEqual(200d, evaluator.Evaluate("a + 100"));
            Assert.AreEqual("XiaoMing", evaluator.Evaluate("b.name"));
            Assert.AreEqual("XiaoMing", evaluator.Evaluate("b.name.ToString()"));
            Assert.AreEqual(101d, evaluator.Evaluate("c[0] + 100"));
            Assert.AreEqual(3, evaluator.Evaluate("c[2]"));
            Assert.AreEqual(5, evaluator.Evaluate("c[4]"));
            Assert.AreEqual(true, evaluator.Evaluate("c[3] >= 4"));
            Assert.AreEqual(true, evaluator.Evaluate("c[3] + c[4] + 100 <= 109"));
            Assert.AreEqual(3, evaluator.Evaluate("d.items[2]"));
            Assert.AreEqual(5, evaluator.Evaluate("d.items[4]"));
            Assert.AreEqual(true, evaluator.Evaluate("d.items[3] == 4"));
            Assert.AreEqual(true, evaluator.Evaluate("d.items[3] + d.items[4] + 100 <= 109"));

            var valueObj2 = new
            {
                Country = "China",
                Company = "Alibaba"
            };
            evaluator = new Evaluator(new EvaluateContext(valueObj2));
            Assert.AreEqual("China", evaluator.Evaluate("Country"));
            Assert.AreEqual("Alibaba", evaluator.Evaluate("Company"));
            Assert.AreEqual("China.Alibaba", evaluator.Evaluate("Country + \".\" + Company"));
        }

        [TestMethod]
        public void ComparsionOperator()
        {
            Evaluator evaluator = new Evaluator();

            // ==, !=
            Assert.AreEqual(true, evaluator.Evaluate("100 == 100"));
            Assert.AreEqual(false, evaluator.Evaluate("100 != 100"));
            Assert.AreEqual(false, evaluator.Evaluate("90 == 100"));
            Assert.AreEqual(true, evaluator.Evaluate("90 != 100"));
            Assert.AreEqual(true, evaluator.Evaluate("true == true"));
            Assert.AreEqual(true, evaluator.Evaluate("false == false"));
            Assert.AreEqual(false, evaluator.Evaluate("false != false"));

            // <,<=
            Assert.AreEqual(false, evaluator.Evaluate("100 < 100"));
            Assert.AreEqual(true, evaluator.Evaluate("90 < 100"));
            Assert.AreEqual(true, evaluator.Evaluate("100 <= 100"));
            Assert.AreEqual(false, evaluator.Evaluate("101 <= 100"));

            // >,>=
            Assert.AreEqual(false, evaluator.Evaluate("100 > 100"));
            Assert.AreEqual(true, evaluator.Evaluate("101 > 100"));
            Assert.AreEqual(true, evaluator.Evaluate("100 >= 100"));
            Assert.AreEqual(false, evaluator.Evaluate("90 >= 100"));

            // <<, >>
            Assert.AreEqual(400L, evaluator.Evaluate("100 << 2"));
            Assert.AreEqual(400L, evaluator.Evaluate("100.3 << 2"));
            Assert.AreEqual(400L, evaluator.Evaluate("100.8 << 2"));
            Assert.AreEqual(25L, evaluator.Evaluate("100 >> 2"));
            Assert.AreEqual(25L, evaluator.Evaluate("100.3 >> 2"));
            Assert.AreEqual(25L, evaluator.Evaluate("100.8 >> 2"));

            // |, ^, &
            Assert.AreEqual(11L, evaluator.Evaluate("10 | 3"));
            Assert.AreEqual(9L, evaluator.Evaluate("10 ^ 3"));
            Assert.AreEqual(2L, evaluator.Evaluate("10 & 3"));

            // true, false, :?
            Assert.AreEqual(100d, evaluator.Evaluate("2? 100: 10"));
            Assert.AreEqual(10d, evaluator.Evaluate("0 ? 100: 10"));
            Assert.AreEqual(100d, evaluator.Evaluate("true ? 100: 10"));
            Assert.AreEqual(10d, evaluator.Evaluate("false ? 100: 10"));
            Assert.AreEqual(100d, evaluator.Evaluate("\"abc\" ? 100: 10"));
            Assert.AreEqual(10d, evaluator.Evaluate("\"\" ? 100: 10"));
            Assert.AreEqual(10d, evaluator.Evaluate("null ? 100: 10"));
            Assert.AreEqual(false, evaluator.Evaluate(" 1 > 2 ? true: false"));
            Assert.AreEqual(true, evaluator.Evaluate(" 1 <= 2 ? true: false"));
        }
    }
}
