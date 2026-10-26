namespace SharpLox
{
    internal class Parser
    {
        private class ParseError : Exception { }

        List<Token> tokens;
        int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public Expr? Parse()
        {
            try
            {
                return Expression();
            }
            catch (ParseError error)
            {
                return null;
            }
        }

        Expr Expression()
        {
            return Equality();
        }

        Expr Equality()
        {
            Expr result = Comparison();

            while(Match(TokenType.BangEqual, TokenType.EqualEqual))
            {
                Token opr = Previous();
                Expr right = Comparison();
                result = new Binary(result, opr, right);
            }

            return result;
        }

        Expr Comparison()
        {
            Expr result = Term();

            while(Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
            {
                Token opr = Previous();
                Expr right = Term();
                result = new Binary(result, opr, right);
            }

            return result;
        }

        Expr Term()
        {
            Expr result = Factor();

            while(Match(TokenType.Minus, TokenType.Plus))
            {
                Token opr = Previous();
                Expr right = Factor();
                result = new Binary(result, opr, right);
            }

            return result;
        }

        Expr Factor()
        {
            Expr result = Unary();

            while(Match(TokenType.Star, TokenType.Slash))
            {
                Token opr = Previous();
                Expr right = Unary();
                result = new Binary(result, opr, right);
            }

            return result;
        }

        Expr Unary()
        {
            if (Match(TokenType.Bang, TokenType.Minus))
            {
                Token opr = Previous();
                Expr right = Unary();
                return new Unary(opr, right);
            }

            return Primary();
        }

        Expr Primary()
        {
            if (Match(TokenType.True))
                return new Literal(true);
            else if (Match(TokenType.False))
                return new Literal(false);
            else if (Match(TokenType.Nil))
                return new Literal(null);

            else if (Match(TokenType.Number, TokenType.String))
            {
                return new Literal(Previous().literal);
            }
            else if (Match(TokenType.LeftParen))
            {
                Expr group = Expression();
                Consume(TokenType.RightParen, "Expected ')' after expression");
                return new Grouping(group);
            }
            else
            {
                throw Error(Peek(), "Expected expression");
            }
        }

        bool Match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        void Consume(TokenType type, string message)
        {
            if(!Match(type))
            {
                throw Error(Previous(), message);
            }
        }

        bool Check(TokenType type)
        {
            return Peek().type == type && !IsAtEnd();
        }

        Token Advance()
        {
            if (!IsAtEnd())
                current++;
            return Previous();
        }

        bool IsAtEnd()
        {
            return Peek().type == TokenType.EOF;
        }

        Token Peek()
        {
            return tokens[current];
        }

        Token Previous()
        {
            return tokens[current - 1];
        }

        ParseError Error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseError();
        }

        void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().type == TokenType.Semicolon)
                    return;

                switch (Peek().type)
                {
                    case TokenType.Class:
                    case TokenType.Fun:
                    case TokenType.Var:
                    case TokenType.For:
                    case TokenType.If:
                    case TokenType.While:
                    case TokenType.Print:
                    case TokenType.Return:
                        return;
                }

                Advance();
            }
        }
    }
}
