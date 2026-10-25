namespace SharpLox
{
    internal class AstPrinter : ExprVisitor<string>
    {
        public string VisitBinary(Binary expr)
        {
            return Parenthesize(expr.opr.lexeme, expr.left, expr.right);
        }

        public string VisitGrouping(Grouping expr)
        {
            return Parenthesize("group", expr.expression);
        }

        public string VisitLiteral(Literal expr)
        {
            if (expr.value == null)
                return "nil";
            else
                return expr.value.ToString();
        }

        public string VisitUnary(Unary expr)
        {
            return Parenthesize(expr.opr.lexeme, expr.right);
        }

        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        string Parenthesize(string name, params Expr[] exprs)
        {
            string result = $"({name}";

            foreach (Expr e in exprs)
            {
                result += " ";
                result += e.Accept(this);
            }
            result += ")";

            return result;
        }
    }
}
