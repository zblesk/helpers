﻿using System.Linq.Expressions;
using System.Reflection;

namespace zblesk.Joplin;

public class JoplinQueryProvider : IQueryProvider
{
    private readonly JoplinApi _joplinApi;
    private readonly Type _type;

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

    public string GetQueryText(Expression expression)
        => new Parser(expression).GetQuery(_type).ToString();

    public object Execute(Expression expression)
    {
        var criteria = new Parser(expression).GetQuery(_type);
        var searchMethod = typeof(JoplinApi).GetMethod("Search").MakeGenericMethod(_type);
        var result = searchMethod.Invoke(_joplinApi, new object[] { criteria });
        return result;
    }
}
