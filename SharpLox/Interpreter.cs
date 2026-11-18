namespace SharpLox
{
    class RuntimeError : Exception
    {
        public Token token;
        public string message;

        public RuntimeError(Token token, string message)
        {
            this.token = token;
            this.message = message;
        }
    }

    internal class Interpreter : ExprVisitor<object?>
    {
        public void Interpret(Expr? expression)
        {
            try
            {
                Object? value = Evaluate(expression);
                Console.WriteLine(Stringify(value));
            }
            catch (RuntimeError error)
            {
                Lox.Error(error);
            }
        }

        object? ExprVisitor<object?>.VisitBinary(Binary expr)
        {
            object? left = Evaluate(expr.left);
            object? right = Evaluate(expr.right);

            switch (expr.opr.type)
            {
                case TokenType.Plus:
                    if (left is double)
                    {
                        if (right is double)
                            return (double)left + (double)right;
                        else
                            throw new RuntimeError(expr.opr, "Right operand must be a number");
                    }
                    else
                    {
                        if (left is string)
                        {
                            if (right is string)
                                return $"{left}{right}";
                            else
                                throw new RuntimeError(expr.opr, "Right operand must be a string");
                        }
                        else if (right is string)
                            throw new RuntimeError(expr.opr, "Left operand must be a string");
                        else if (right is double)
                            throw new RuntimeError(expr.opr, "Left operand must be a number");
                        else
                            throw new RuntimeError(expr.opr, "Both operands must be either numbers or strings");
                    }
                case TokenType.Minus:
                    CheckNumberOperands(expr.opr, left, right);
                    return (double)left - (double)right;
                case TokenType.Slash:
                    CheckNumberOperands(expr.opr, left, right);
                    return (double)left / (double)right;
                case TokenType.Star:
                    CheckNumberOperands(expr.opr, left, right);
                    return (double)left * (double)right;

                case TokenType.Greater:
                    CheckNumberOperands(expr.opr, left, right);
                    return (double)left > (double)right;
                case TokenType.GreaterEqual:
                    CheckNumberOperands(expr.opr, left, right);
                    return (double)left >= (double)right;
                case TokenType.Less:
                    CheckNumberOperands(expr.opr, left, right);
                    return (double)left < (double)right;
                case TokenType.LessEqual:
                    CheckNumberOperands(expr.opr, left, right);
                    return (double)left <= (double)right;

                case TokenType.EqualEqual:
                    return IsEqual(left, right);
                case TokenType.BangEqual:
                    return !IsEqual(left, right);
            }

            return null;
        }

        object? ExprVisitor<object?>.VisitGrouping(Grouping expr)
        {
            return Evaluate(expr.expression);
        }

        object? ExprVisitor<object?>.VisitLiteral(Literal expr)
        {
            return expr.value;
        }

        object? ExprVisitor<object?>.VisitUnary(Unary expr)
        {
            object? right = Evaluate(expr.right);

            switch (expr.opr.type)
            {
                case TokenType.Bang:
                    return !IsTruthy(right);

                case TokenType.Minus:
                    CheckNumberOperand(expr.opr, right);
                    return -(double)right;
            }

            return null;
        }

        object? Evaluate(Expr? e)
        {
            if (e is not null)
                return e.Accept(this);
            return null;
        }

        bool IsTruthy(object? o)
        {
            if (o is null)
                return false;
            if (o is bool)
                return (bool)o;
            return false;
        }

        bool IsEqual(object? l, object? r)
        {
            if (l is null && r is null)
                return true;
            if (l is null)
                return false;
            return l.Equals(r);
        }

        string Stringify(object? o)
        {
            if (o is null)
                return "nil";

            if (o is double)
                return o.ToString();
            //string text = o.ToString();

            return o.ToString();
        }

        void CheckNumberOperand(Token opr, object? operand)
        {
            if (operand is not double)
                throw new RuntimeError(opr, "Operand must be a number");
        }

        void CheckNumberOperands(Token opr, object? left, object? right)
        {
            if (left is not double)
                throw new RuntimeError(opr, "Left operand must be a number");

            if (right is not double)
                throw new RuntimeError(opr, "Right operand must be a number");
        }
    }
}
