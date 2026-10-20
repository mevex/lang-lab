using System.Globalization;

namespace SharpLox
{
    internal class Scanner
    {
        string source;

        int start = 0;
        int current = 0;
        int line = 1;

        List<Token> tokens = new List<Token>();
        static Dictionary<string, TokenType> keywords = new() {
            {"class", TokenType.Class},
            {"fun", TokenType.Fun},
            {"var", TokenType.Var},
            {"return", TokenType.Return},
            {"super", TokenType.Super},
            {"this", TokenType.This},
            {"if", TokenType.If},
            {"else", TokenType.Else},
            {"for", TokenType.For},
            {"while", TokenType.While},
            {"and", TokenType.And},
            {"or", TokenType.Or},
            {"true", TokenType.True},
            {"false", TokenType.True},
            {"print", TokenType.Print},
            {"nil", TokenType.Nil},
            };

        public Scanner(string source)
        {
            this.source = source;
        }

        public List<Token> ScanTokens()
        {
            if (source == null)
            {
                tokens.Add(new Token(TokenType.EOF, "", line, null));
                return tokens;
            }

            while (!IsAtEnd())
            {
                start = current;
                ScanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", line, null));
            return tokens;
        }

        void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                // Single character tokens
                case '(':
                    AddToken(TokenType.LeftParen);
                    break;
                case ')':
                    AddToken(TokenType.RightParen);
                    break;
                case '{':
                    AddToken(TokenType.LeftBrace);
                    break;
                case '}':
                    AddToken(TokenType.RightBrace);
                    break;
                case ',':
                    AddToken(TokenType.Comma);
                    break;
                case '.':
                    AddToken(TokenType.Dot);
                    break;
                case ';':
                    AddToken(TokenType.Semicolon);
                    break;
                case '+':
                    AddToken(TokenType.Plus);
                    break;
                case '-':
                    AddToken(TokenType.Minus);
                    break;
                case '*':
                    AddToken(TokenType.Star);
                    break;

                // distinguish between one or two character token
                case '=':
                    AddToken(MatchNext('=') ? TokenType.EqualEqual : TokenType.Equal);
                    break;
                case '!':
                    AddToken(MatchNext('=') ? TokenType.BangEqual : TokenType.Bang);
                    break;
                case '>':
                    AddToken(MatchNext('=') ? TokenType.GreaterEqual : TokenType.Greater);
                    break;
                case '<':
                    AddToken(MatchNext('=') ? TokenType.LessEqual : TokenType.Less);
                    break;

                // Division or comments
                case '/':
                    if (MatchNext('/'))
                        ConsumeLineComment();
                    else if (MatchNext('*'))
                        ConsumeBlockComment();
                    else
                        AddToken(TokenType.Slash);
                    break;

                case '"':
                    ScanString();
                    break;

                // Ignore white spaces
                case ' ':
                case '\r':
                case '\t':
                    break;

                // New line
                case '\n':
                    line++;
                    break;

                default:
                    if (IsDigit(c))
                        ScanNumber();
                    else if (IsAlpha(c))
                        ScanIdentifier();
                    else
                        Lox.Error(line, $"Unexpected character: {c}");
                    break;
            }
        }

        void ScanString()
        {
            // NOTE: We don't support escape sequences like "\n"
            while(Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n')
                    line++;
                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(line, "Unterminated string");
                return;
            }

            // The closing quote
            Advance();

            AddStringToken();
        }

        void ScanNumber()
        {
            while (IsDigit(Peek()) && !IsAtEnd())
                Advance();

            if (Peek() == '.')
            {
                if (IsDigit(PeekNext()))
                {
                    // Consumes the dot
                    Advance();

                    while (IsDigit(Peek()) && !IsAtEnd())
                        Advance();
                }
                else
                {
                    Lox.Error(line, "Trailing '.' after a number literal");
                }
            }

            ReadOnlySpan<char> value = source.AsSpan(start, current - start);
            AddToken(TokenType.Number, Double.Parse(value, CultureInfo.InvariantCulture));
        }

        void ScanIdentifier()
        {
            while (IsAlphaNumeric(Peek()))
                Advance();

            string lexeme = source.Substring(start, current - start);
            if (keywords.TryGetValue(lexeme, out TokenType type))
                AddKeywordToken(type, lexeme);
            else
                AddToken(TokenType.Identifier);
        }

        void ConsumeLineComment()
        {
            while (Peek() != '\n' && !IsAtEnd())
                Advance();
        }

        void ConsumeBlockComment()
        {
            while (!(Peek() == '*' && PeekNext() == '/') && !IsAtEnd())
            {
                char c = Advance();

                if (c == '\n')
                    line++;

                if (c == '/' && MatchNext('*'))
                    ConsumeBlockComment();
            }

            // Consumes the star
            Advance();
            // Consumes the slash
            Advance();
        }

        void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        void AddStringToken()
        {
            // Remove the surrounding quotes
            string literal = source.Substring(start + 1, current - start - 2);
            tokens.Add(new Token(TokenType.String, $"\"{literal}\"", line, literal));
        }

        void AddKeywordToken(TokenType type, string lexeme)
        {
            tokens.Add(new Token(type, lexeme, line, null));
        }

        void AddToken(TokenType type, Object? literal)
        {
            string lexeme = source.Substring(start, current - start);
            tokens.Add(new Token(type, lexeme, line, literal));
        }

        char Advance()
        {
            return source[current++];
        }

        char Peek()
        {
            return IsAtEnd() ? '\0' : source[current];
        }

        char PeekNext()
        {
            if (current + 1 >= source.Length)
                return '\0';
            else
                return source[current + 1];
        }

        bool IsAtEnd()
        {
            return current >= source.Length;
        }

        bool MatchNext(char expected)
        {
            if (source[current] != expected || IsAtEnd())
            {
                return false;
            }
            else
            {
                current++;
                return true;
            }
        }

        bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        bool IsAlpha(char c)
        {
            return c >= 'a' && c <= 'z' ||
                   c >= 'A' && c <= 'Z' ||
                   c == '_';
        }

        bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }
    }
}
