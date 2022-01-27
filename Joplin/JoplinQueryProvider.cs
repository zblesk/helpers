using System.Linq.Expressions;
using System.Reflection;
using zblesk.Joplin.Poco;

namespace zblesk.Joplin;

public class JoplinQueryProvider : IQueryProvider
{
    private readonly JoplinApi joplinApi;

    public JoplinQueryProvider(JoplinApi joplinApi)
    {
        this.joplinApi = joplinApi;
    }

    IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression) 
        => new Query<S>(this, expression);

    IQueryable IQueryProvider.CreateQuery(Expression expression)
    {
        var elementType = TypeSystem.GetElementType(expression.Type);
        try
        {
            return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(elementType), new object[] { this, expression });
        }
        catch (TargetInvocationException tie)
        {
            throw tie.InnerException ?? tie;
        }
    }

    S IQueryProvider.Execute<S>(Expression expression) 
        => (S)Execute(expression);

    object IQueryProvider.Execute(Expression expression) 
        => Execute(expression);

    public string GetQueryText(Expression expression) 
        => new Parser(expression).GetQuery<Note>().ToString();

    public object Execute(Expression expression)
    {
        var (query, fields) = new Parser(expression).GetQuery<Note>();
        var r = joplinApi.Search(query, fields);
        r.Wait();
        return r.Result;
    }
}
