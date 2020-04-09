using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Code.Expressions.CSharp.Tests
{
    [TestClass]
    public class LexerTest
    {
        [TestMethod]
        public void ParseString_01()
        {
            Lexer lexer = new Lexer("\"abc\"");
            Assert.IsTrue(lexer.MoveNext());
            Token token = lexer.Token;
            Assert.AreEqual(TokenType.StringLiteral, token.Type);
            Assert.AreEqual("abc", token.Text);
        }

        [TestMethod]
        public void ParseString_02()
        {
            Lexer lexer = new Lexer("\"a\"\"bc\"");
            Assert.IsTrue(lexer.MoveNext());
            Token token = lexer.Token;
            Assert.AreEqual(TokenType.StringLiteral, token.Type);
            Assert.AreEqual("a\"bc", token.Text);
        }
    }
}
