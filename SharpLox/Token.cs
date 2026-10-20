namespace SharpLox
{
    internal enum TokenType
    {
        // Single-character tokens
        LeftParen, RightParen, LeftBrace, RightBrace,
        Comma, Dot, Semicolon,
        Plus, Minus, Star, Slash,

        // One or two character tokens
        Equal, EqualEqual,
        Bang, BangEqual,
        Greater, GreaterEqual,
        Less, LessEqual,

        // Literals
        Identifier, String, Number,

        // Keywords
        Class, Fun, Var, Return, Super, This,
        If, Else, For, While,
        And, Or, True, False,
        Print,
        Nil,

        EOF
    }

    internal class Token
    {
        TokenType type;
        string lexeme;
        int line;
        object? literal;

        public Token(TokenType type, string lexeme, int line, object? literal)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.line = line;
            this.literal = literal;
        }

        public override string ToString()
        {
            return $"{type} {lexeme} {literal}";
        }
    }
}
