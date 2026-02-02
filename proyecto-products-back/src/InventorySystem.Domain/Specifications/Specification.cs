using System.Linq.Expressions;

namespace InventorySystem.Domain.Specifications;

public abstract class Specification<T> : ISpecification<T>
{
    public abstract Expression<Func<T, bool>> Criteria { get; }

    public bool IsSatisfiedBy(T entity)
    {
        var predicate = Criteria.Compile();
        return predicate(entity);
    }

    public Specification<T> And(Specification<T> specification) => new AndSpecification<T>(this, specification);
    public Specification<T> Or(Specification<T> specification) => new OrSpecification<T>(this, specification);
    public Specification<T> Not() => new NotSpecification<T>(this);
}

public class AndSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    public AndSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override Expression<Func<T, bool>> Criteria
    {
        get
        {
            var leftExpr = _left.Criteria;
            var rightExpr = _right.Criteria;
            var param = Expression.Parameter(typeof(T));
            var combined = Expression.AndAlso(
                Expression.Invoke(leftExpr, param),
                Expression.Invoke(rightExpr, param));
            return Expression.Lambda<Func<T, bool>>(combined, param);
        }
    }
}

public class OrSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    public OrSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left;
        _right = right;
    }

    public override Expression<Func<T, bool>> Criteria
    {
        get
        {
            var leftExpr = _left.Criteria;
            var rightExpr = _right.Criteria;
            var param = Expression.Parameter(typeof(T));
            var combined = Expression.OrElse(
                Expression.Invoke(leftExpr, param),
                Expression.Invoke(rightExpr, param));
            return Expression.Lambda<Func<T, bool>>(combined, param);
        }
    }
}

public class NotSpecification<T> : Specification<T>
{
    private readonly Specification<T> _specification;

    public NotSpecification(Specification<T> specification)
    {
        _specification = specification;
    }

    public override Expression<Func<T, bool>> Criteria
    {
        get
        {
            var expr = _specification.Criteria;
            var param = Expression.Parameter(typeof(T));
            var notExpr = Expression.Not(Expression.Invoke(expr, param));
            return Expression.Lambda<Func<T, bool>>(notExpr, param);
        }
    }
}
