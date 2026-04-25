using System.Linq.Expressions;

namespace SPS.Shared.Persistence.Specifications
{
    public abstract class Specification<T>
    {
        public abstract Expression<Func<T, bool>> ToExpression();
        public bool IsSatisfiedBy(T entity) => ToExpression().Compile()(entity);

        public static Specification<T> operator &(Specification<T> left, Specification<T> right) => new AndSpecification<T>(left, right);
        public static Specification<T> operator |(Specification<T> left, Specification<T> right) => new OrSpecification<T>(left, right);
        public static Specification<T> operator !(Specification<T> spec) => new NotSpecification<T>(spec);
    }

    internal class AndSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;

        public AndSpecification(Specification<T> left, Specification<T> right) => (_left, _right) = (left, right);
        public override Expression<Func<T, bool>> ToExpression()
        {
            var leftExpr = _left.ToExpression();
            var rightExpr = _right.ToExpression();
            var param = Expression.Parameter(typeof(T));
            var body = Expression.AndAlso(
                Expression.Invoke(leftExpr, param),
                Expression.Invoke(rightExpr, param));
            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }

    internal class OrSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;

        public OrSpecification(Specification<T> left, Specification<T> right) => (_left, _right) = (left, right);
        public override Expression<Func<T, bool>> ToExpression()
        {
            var leftExpr = _left.ToExpression();
            var rightExpr = _right.ToExpression();
            var param = Expression.Parameter(typeof(T));
            var body = Expression.OrElse(
                Expression.Invoke(leftExpr, param),
                Expression.Invoke(rightExpr, param));
            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }

    internal class NotSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _spec;

        public NotSpecification(Specification<T> spec) => _spec = spec;
        public override Expression<Func<T, bool>> ToExpression()
        {
            var expr = _spec.ToExpression();
            var param = expr.Parameters[0];
            var body = Expression.Not(expr.Body);
            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }

    public class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _target;
        private readonly ParameterExpression _replacement;

        public ParameterReplacer(ParameterExpression target, ParameterExpression replacement)
        {
            _target = target;
            _replacement = replacement;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _target ? _replacement : base.VisitParameter(node);
        }
    }
}