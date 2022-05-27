using System.Collections;
using System.Linq.Expressions;

namespace zblesk.Joplin;

public class JoplinDataset<T> : IQueryable<T>, IQueryable, IOrderedQueryable<T>, IOrderedQueryable
{
    readonly JoplinQueryProvider _provider;
    readonly Expression _expression;

    public JoplinDataset(JoplinQueryProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _expression = Expression.Constant(this);
    }

    public JoplinDataset(JoplinQueryProvider provider, Expression expression)
    {
        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }
        if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
        {
            throw new ArgumentOutOfRangeException(nameof(expression));
        }
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _expression = expression;
    }

    Expression IQueryable.Expression => _expression;

    Type IQueryable.ElementType => typeof(T);

    IQueryProvider IQueryable.Provider => _provider;

    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_provider.Execute(_expression)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_provider.Execute(_expression)).GetEnumerator();
}
