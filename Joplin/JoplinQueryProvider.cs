using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper;
using Flurl;
using Flurl.Http;

namespace zblesk.Joplin;

public class JoplinQueryProvider : IQueryProvider
{
    private readonly JoplinApi _joplinApi;
    private readonly QueryVisitor _visitor = new();
    private static readonly IMapper _mapper = new MapperConfiguration(_ => { }).CreateMapper();

    public JoplinQueryProvider(JoplinApi joplinApi)
    {
        _joplinApi = joplinApi;
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
        var promise = url.GetJsonAsync<dynamic>();
        promise.Wait();
        var apiResult = promise.Result;
        var destinationType = param.ReturnType ?? param.queriedTypes.First();

        switch (param.RequestedResultKind)
        {
            case ResultKind.List:
                var listType = typeof(List<>).MakeGenericType(new[] { destinationType });
                var list = Activator.CreateInstance(listType);
                var addMethod = listType!.GetMethod("Add");
                var add = (object obj) => addMethod!.Invoke(list, new object[] { obj });
                var page = 1;
                var @continue = false;
                // If the user requested list, but API call was for a single object. Wrap in list.
                if (param.ApiResponseKind == ResultKind.Single)
                {
                    var resultObj = _mapper.Map(apiResult, typeof(ExpandoObject), destinationType);
                    add(resultObj);
                }
                else
                    do
                    {
                        foreach (var obj in apiResult.items)
                        {
                            var resultObj = _mapper.Map(obj, typeof(ExpandoObject), destinationType);
                            add(resultObj);
                        }
                        @continue = apiResult.has_more == true && !param.ExplicitPagingInvoked;
                        if (@continue)
                        {
                            page++;
                            promise = _joplinApi.BuildQuery(param, page).GetJsonAsync<dynamic>();
                            promise.Wait();
                            apiResult = promise.Result;
                        }
                    }
                    while (@continue);
                return list!;
            case ResultKind.Single:
                return _mapper.Map(apiResult, typeof(ExpandoObject), destinationType);
            case ResultKind.Bool:
                break;
            default:
                throw new NotImplementedException($"Unknown result type: {param.RequestedResultKind}");
        }
        throw new NotImplementedException($"Unknown result type: {param.RequestedResultKind}");
    }
}
