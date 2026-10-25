namespace SharpLox 
{
    internal abstract class Expr
    {
        public abstract R Accept<R>(ExprVisitor<R> visitor);
    }


    internal interface ExprVisitor<R>
    {
        public R VisitBinary(Binary expr);
        public R VisitGrouping(Grouping expr);
        public R VisitLiteral(Literal expr);
        public R VisitUnary(Unary expr);
    }


    internal class Binary : Expr
    {
        public Expr left;
        public Token opr;
        public Expr right;

        public Binary(Expr left, Token opr, Expr right)
        {
            this.left = left;
            this.opr = opr;
            this.right = right;
        }

        public override R Accept<R>(ExprVisitor<R> visitor)
        {
            return visitor.VisitBinary(this);
        }
    }

    internal class Grouping : Expr
    {
        public Expr expression;

        public Grouping(Expr expression)
        {
            this.expression = expression;
        }

        public override R Accept<R>(ExprVisitor<R> visitor)
        {
            return visitor.VisitGrouping(this);
        }
    }

    internal class Literal : Expr
    {
        public object value;

        public Literal(object value)
        {
            this.value = value;
        }

        public override R Accept<R>(ExprVisitor<R> visitor)
        {
            return visitor.VisitLiteral(this);
        }
    }

    internal class Unary : Expr
    {
        public Token opr;
        public Expr right;

        public Unary(Token opr, Expr right)
        {
            this.opr = opr;
            this.right = right;
        }

        public override R Accept<R>(ExprVisitor<R> visitor)
        {
            return visitor.VisitUnary(this);
        }
    }
}
