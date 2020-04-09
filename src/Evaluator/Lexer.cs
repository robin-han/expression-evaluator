using System;
using System.Collections.Generic;
using System.Text;

namespace Code.Expressions.CSharp
{
    public class Lexer
    {
        /// <summary>
        /// End of input character, used as index outof the expression range.
        /// </summary>
        private const char EOI = char.MinValue;

        #region Fields
        private readonly string text;
        private int pos;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the Lexer class.
        /// </summary>
        /// <param name="text"></param>
        public Lexer(string text)
        {
            this.text = text;
            this.pos = -1;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the expression text.
        /// </summary>
        public string Text
        {
            get { return this.text; }
        }

        /// <summary>
        /// Get current position.
        /// </summary>
        public int Position
        {
            get { return this.pos; }
        }

        /// <summary>
        /// Gets current token.
        /// </summary>
        public Token Token
        {
            get;
            private set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Indicates whether position is at end of the expression.
        /// </summary>
        /// <returns></returns>
        public bool IsEOF()
        {
            return pos >= text.Length;
        }

        /// <summary>
        /// Move to next token.
        /// </summary>
        /// <returns>true if can move next, otherwise false</returns>
        public bool MoveNext()
        {
            this.Token = NextToken();
            return (this.Token != Token.EOF);
        }

        /// <summary>
        /// Peek char by delta.
        /// </summary>
        /// <param name="delta">The delta, default is 1.</param>
        /// <returns>The character.</returns>
        public char PeekChar(int delta)
        {
            return CharAt(pos + delta);
        }

        /// <summary>
        /// Peek next toke by delta.
        /// </summary>
        /// <param name="delta">The delta, default is 1.</param>
        /// <returns>The token</returns>
        public Token PeekToken(uint delta = 1)
        {
            int oldPos = pos;
            Token token = Token;
            for (int i = 0; (i < delta && token != Token.EOF); i++)
            {
                token = NextToken();
            }
            Reset(oldPos);
            return token;
        }

        /// <summary>
        /// Gets next token.
        /// </summary>
        /// <returns></returns>
        private Token NextToken()
        {
            while (!IsEOF())
            {
                char ch = NextChar();

                if (char.IsWhiteSpace(ch))
                {
                    continue;
                }

                if (IsFirstIdentifier(ch))
                {
                    return ScanIdentifier();
                }

                switch (ch)
                {
                    case '"':
                        return ScanString();

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        return ScanNumber();

                    case '.':
                        return Token.Dot;

                    case '(':
                        return Token.OpenParen;

                    case ')':
                        return Token.CloseParen;

                    case '[':
                        return Token.OpenBracket;

                    case ']':
                        return Token.CloseBracket;

                    case '+':
                        return Token.Plus;

                    case '-':
                        return Token.Minus;

                    case '*':
                        return Token.Asterisk;

                    case '/':
                        return Token.Slash;

                    case '!':
                        if (CharAt(pos + 1) == '=')
                        {
                            AdvanceChar();
                            return Token.ExclamationEqual;
                        }
                        else
                        {
                            return Token.Exclamation;
                        }

                    case '~':
                        return Token.Tilde;

                    case '=':
                        if (CharAt(pos + 1) == '=')
                        {
                            AdvanceChar();
                            return Token.EqualEqual;
                        }
                        else
                        {
                            return Token.Equal;
                        }

                    case '>':
                        if (CharAt(pos + 1) == '=')
                        {
                            AdvanceChar();
                            return Token.GreaterThanEqual;
                        }
                        else if (CharAt(pos + 1) == '>')
                        {
                            AdvanceChar();
                            return Token.GreaterThanGreaterThan;
                        }
                        else
                        {
                            return Token.GreaterThan;
                        }

                    case '<':
                        if (CharAt(pos + 1) == '=')
                        {
                            AdvanceChar();
                            return Token.LessThanEqual;
                        }
                        else if (CharAt(pos + 1) == '<')
                        {
                            AdvanceChar();
                            return Token.LessThanLessThan;
                        }
                        else
                        {
                            return Token.LessThan;
                        }

                    case '&':
                        if (CharAt(pos + 1) == '&')
                        {
                            AdvanceChar();
                            return Token.AmpersandAmpersand;
                        }
                        else
                        {
                            return Token.Ampersand;
                        }

                    case '^':
                        return Token.Caret;

                    case '|':
                        if (CharAt(pos + 1) == '|')
                        {
                            AdvanceChar();
                            return Token.BarBar;
                        }
                        else
                        {
                            return Token.Bar;
                        }

                    case '?':
                        return Token.Question;

                    case ':':
                        return Token.Colon;

                    case ',':
                        return Token.Comma;

                    default:
                        if (ch != EOI)
                        {
                            return new Token(TokenType.Unknown, ch.ToString());
                        }
                        break;
                }
            }
            return Token.EOF;
        }

        /// <summary>
        /// Reset the position.
        /// </summary>
        /// <param name="newPos">The new position.</param>
        private void Reset(int newPos)
        {
            pos = newPos;
        }

        /// <summary>
        /// Advance the current position by one.
        /// </summary>
        private void AdvanceChar()
        {
            pos++;
        }

        /// <summary>
        /// Grab the next character and advance the position.
        /// </summary>The next char.
        /// <returns></returns>
        private char NextChar()
        {
            if (pos >= text.Length)
            {
                return EOI;
            }
            return CharAt(++pos);
        }

        /// <summary>
        /// Scans identifier.
        /// </summary>
        /// <returns></returns>
        private Token ScanIdentifier()
        {
            int startPos = pos;

            char ch = CharAt(pos);
            while (IsIdentifier(ch))
            {
                ch = CharAt(++pos);
            }

            return new Token(TokenType.Identifier, text.Substring(startPos, pos-- - startPos)); ;
        }

        /// <summary>
        /// Scans string literal
        /// </summary>
        /// <returns></returns>
        private Token ScanString()
        {
            string str = "";

            bool inQuote = true;
            while (inQuote && !IsEOF())
            {
                char ch = CharAt(++pos);
                if (ch == '"' && CharAt(pos + 1) == '"')
                {
                    NextChar();
                }
                else
                {
                    inQuote = (ch != '"');
                }

                if (inQuote)
                {
                    str += ch;
                }
            }

            return new Token(TokenType.StringLiteral, str);
        }

        /// <summary>
        /// Scan a number
        /// </summary>
        /// <returns></returns>
        private Token ScanNumber()
        {
            int startPos = pos;

            char ch = CharAt(pos);
            if (ch == '+' || ch == '-')
            {
                ch = CharAt(++pos);
            }

            while ('0' <= ch && ch <= '9')
            {
                ch = CharAt(++pos);
            }

            if (ch == '.')
            {
                ch = CharAt(++pos);
            }

            if (ch == 'e' || ch == 'E')
            {
                ch = CharAt(++pos);
                if (ch == '+' || ch == '-')
                {
                    ch = CharAt(++pos);
                }
            }

            while ('0' <= ch && ch <= '9')
            {
                ch = CharAt(++pos);
            }

            return new Token(TokenType.NumberLiteral, text.Substring(startPos, pos-- - startPos));
        }

        /// <summary>
        /// Gets a character at given index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The character</returns>
        private char CharAt(int index)
        {
            if (index < 0 || index >= text.Length)
            {
                return EOI;
            }
            return text[index];
        }

        /// <summary>
        /// Check whether character is identifier's first character.
        /// </summary>
        /// <param name="ch">The character.</param>
        /// <returns></returns>
        private bool IsFirstIdentifier(char ch)
        {
            return ch == '_' || ('a' <= ch && ch <= 'z') || ('A' <= ch && ch <= 'Z');
        }

        /// <summary>
        /// Check whether character is idendifier character.
        /// </summary>
        /// <param name="ch">The character.</param>
        /// <returns>true if it is idendifier character otherwise false.</returns>
        private bool IsIdentifier(char ch)
        {
            return ch == '_' || ('a' <= ch && ch <= 'z') || ('A' <= ch && ch <= 'Z') || ('0' <= ch && ch <= '9');
        }
        #endregion
    }
}
