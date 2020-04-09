using System;

namespace Code.Expressions.CSharp
{
    #region Class Token
    /// <summary>
    /// Defines Token class
    /// </summary>
    public class Token
    {
        #region Tokens
        public static readonly Token EOF = new Token(TokenType.EOF, "");

        public static readonly Token Plus = new Token(TokenType.Plus, "+");
        public static readonly Token Minus = new Token(TokenType.Minus, "-");
        public static readonly Token Asterisk = new Token(TokenType.Asterisk, "*");
        public static readonly Token Slash = new Token(TokenType.Slash, "/");
        public static readonly Token Percent = new Token(TokenType.Percent, "%");
        public static readonly Token PlusPlus = new Token(TokenType.Plus, "++");
        public static readonly Token MinusMinus = new Token(TokenType.Minus, "--");

        public static readonly Token Equal = new Token(TokenType.Equal, "=");

        public static readonly Token LessThan = new Token(TokenType.LessThan, "<");
        public static readonly Token GreaterThan = new Token(TokenType.GreaterThan, ">");
        public static readonly Token LessThanLessThan = new Token(TokenType.LessThanLessThan, "<<");
        public static readonly Token GreaterThanGreaterThan = new Token(TokenType.GreaterThanGreaterThan, ">>");
        public static readonly Token LessThanEqual = new Token(TokenType.LessThanEqual, "<=");
        public static readonly Token GreaterThanEqual = new Token(TokenType.GreaterThanEqual, ">=");
        public static readonly Token EqualEqual = new Token(TokenType.EqualsEqual, "==");
        public static readonly Token ExclamationEqual = new Token(TokenType.ExclamationEqual, "!=");
        public static readonly Token AmpersandAmpersand = new Token(TokenType.AmpersandAmpersand, "&&");
        public static readonly Token BarBar = new Token(TokenType.BarBar, "||");

        public static readonly Token Question = new Token(TokenType.Question, "?");
        public static readonly Token Colon = new Token(TokenType.Colon, ":");

        public static readonly Token Ampersand = new Token(TokenType.Ampersand, "&");
        public static readonly Token Bar = new Token(TokenType.Bar, "|");
        public static readonly Token Caret = new Token(TokenType.Caret, "^");

        public static readonly Token Exclamation = new Token(TokenType.Exclamation, "!");
        public static readonly Token Tilde = new Token(TokenType.Tilde, "~");

        public static readonly Token Comma = new Token(TokenType.Comma, ",");
        public static readonly Token Dot = new Token(TokenType.Dot, ".");
        public static readonly Token OpenParen = new Token(TokenType.OpenParen, "(");
        public static readonly Token CloseParen = new Token(TokenType.CloseParen, ")");
        public static readonly Token OpenBracket = new Token(TokenType.OpenBracket, "[");
        public static readonly Token CloseBracket = new Token(TokenType.Dot, "]");
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the Token class.
        /// </summary>
        /// <param name="type">The token type.</param>
        /// <param name="text">The token text.</param>
        public Token(TokenType type, string text)
        {
            this.Type = type;
            this.Text = text;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the token type.
        /// </summary>
        public TokenType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the token text.
        /// </summary>
        public string Text
        {
            get;
            private set;
        }
        #endregion
    }
    #endregion

    #region Enum TokenType
    /// <summary>
    /// Defines TokeType enumation.
    /// </summary>
    public enum TokenType
    {
        Unknown,
        EOF,

        NumberLiteral,
        StringLiteral,

        OpenParen, // (
        CloseParen,
        OpenBracket, // [
        CloseBracket,

        Comma,
        Dot,
        Identifier,

        Equal,

        LessThan,
        GreaterThan,
        LessThanLessThan,
        GreaterThanGreaterThan,
        LessThanEqual,
        GreaterThanEqual,
        EqualsEqual,
        ExclamationEqual,

        Plus,
        Minus,
        Asterisk,
        Slash,
        Percent,
        MinusMinus,
        PlusPlus,

        Ampersand,
        Bar,
        Caret,
        Exclamation,
        Tilde,
        AmpersandAmpersand,
        BarBar,
        Question,
        Colon
    }
    #endregion
}
