using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using Flurl.Http;

namespace zblesk.Joplin;

public class JoplinQueryProvider : IQueryProvider
{
    private readonly JoplinApi _joplinApi;
    private readonly Type _type;
    private QueryVisitor _visitor = new QueryVisitor();

    public JoplinQueryProvider(JoplinApi joplinApi, Type type)
    {
        _joplinApi = joplinApi;
        _type = type;
    }

    IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
        => new JoplinDataset<S>(this, expression);

    IQueryable IQueryProvider.CreateQuery(Expression expression)
    {
        var elementType = TypeSystem.GetElementType(expression.Type);
        try
        {
            return (IQueryable)Activator.CreateInstance(typeof(JoplinDataset<>).MakeGenericType(elementType), new object[] { this, expression });
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

    public object Execute(Expression expression)
    {
        var param = _visitor.ExtractParams(expression);
        var url = _joplinApi.BuildQuery(param);
        var q = url.GetJsonAsync();
        q.Wait();

        switch (param.Result)
        {
            case ResultKind.List:
                break;
            case ResultKind.Single:
                break;
            case ResultKind.Bool:
                break;
            default:
                throw new NotImplementedException($"Unknown result type: {param.Result}");
        }

        //var r = new List<>();

        //var config = new MapperConfiguration(cfg => cfg.CreateMap<T, dynamic>());
        //var map = config.CreateMapper();

        //foreach (var element in q.Result.items)
        //{
        //    T n = map.Map<T>(element);
        //    r.Add(n);
        //}

        //if (searchCriteria.kind == ResultKind.Single)
        //    return r.FirstOrDefault();
        //if (searchCriteria.kind == ResultKind.Bool)
        //    return r.FirstOrDefault() != null;
        //return r;
        return null;
    }
}
