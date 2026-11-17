namespace SharpLox
{
    internal class Interpreter : ExprVisitor<object?>
    {
        public object? Interpret(Expr expression)
        {
            return expression.Accept(this);
        }

        object? ExprVisitor<object?>.VisitBinary(Binary expr)
        {
            object left = Evaluate(expr.left);
            object right = Evaluate(expr.right);

            switch (expr.opr.type)
            {
                case TokenType.Plus:
                    if (left is double && right is double)
                        return (double)left + (double)right;
                    if (left is string && right is string)
                        return $"{left}{right}";
                    break;
                case TokenType.Minus:
                    return (double)left - (double)right;
                case TokenType.Slash:
                    return (double)left / (double)right;
                case TokenType.Star:
                    return (double)left * (double)right;

                case TokenType.Greater:
                    return (double)left > (double)right;
                case TokenType.GreaterEqual:
                    return (double)left >= (double)right;
                case TokenType.Less:
                    return (double)left < (double)right;
                case TokenType.LessEqual:
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
                    if(right is double)
                        return -(double)right;
                    else
                        // Error!
                    break;
            }

            return null;
        }

        object? Evaluate(Expr e)
        {
            return e.Accept(this);
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
    }
}
