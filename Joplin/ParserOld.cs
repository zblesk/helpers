using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace zblesk.Joplin;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
internal class Parser
{
    readonly List<string> _fieldList = new();
    readonly Expression _expression;
    string? _queryText, _fields;
    int _pageSize = 100;
    int? _page = null;
    ResultKind _resultKind = ResultKind.List;

    static readonly ReadOnlyDictionary<Type, string> DefaultFieldset =
        new(new Dictionary<Type, string>
            {
                { typeof(Note), new Note().DefaultFetchFields },
                { typeof(Notebook), new Notebook().DefaultFetchFields },
            });

    static readonly ReadOnlyDictionary<Type, string> DefaultApiPaths =
        new(new Dictionary<Type, string>
            {
                { typeof(Note), new Note().EntityApiPath },
                { typeof(Notebook), new Notebook().EntityApiPath },
            });

    static readonly ReadOnlyDictionary<Type, string> FolderNames =
        new(new Dictionary<Type, string>
            {
                { typeof(Note), new Note().SearchType },
                { typeof(Notebook), new Notebook().SearchType },
            });

    public Parser(Expression expression)
    {
        _expression = expression;
    }

    public ParseResults GetQuery(Type type)
    {
        var endpoint = "search";
        if (_queryText == null)
        {
            _queryText = Parse(_expression);

            // If there is a query, search. If not, go to the standard endpoint to get all objects.zs
            if (_queryText.Length == 0)
                endpoint = DefaultApiPaths[type];

            if (_fieldList.Count > 0
                && !_fieldList.Contains("id"))
                _fieldList.Add("id");
            _fields = _fieldList.Count == 0
                    ? DefaultFieldset[type]
                    : string.Join(',', _fieldList);

            // Joplin's naming differs here: it's `source_url` on a note, but `sourceurl` as a search param.
            _queryText = _queryText.Replace("source_url", "sourceurl");
        }
        return new ParseResults(_queryText, _fields ?? "", endpoint, _resultKind, _pageSize, _page, FolderNames[type]);
    }

    string Parse(Expression ex)
    {
        if (ex is BinaryExpression)
            return Parse(((BinaryExpression)ex));
        if (ex is MemberExpression)
            return Parse(((MemberExpression)ex));
        if (ex is ConstantExpression)
            return Parse(((ConstantExpression)ex));
        if (ex is UnaryExpression)
            return Parse(((UnaryExpression)ex));
        if (ex is LambdaExpression)
            return Parse(((LambdaExpression)ex));
        if (ex is MethodCallExpression)
            return Parse(((MethodCallExpression)ex));
        return "?? " + ex.GetType();
    }

    string Parse(BinaryExpression ex)
        => $"{Parse(ex.Left)}{Operation(ex.NodeType)}{Parse(ex.Right)}";

    string Parse(MemberExpression ex) => $"{ex.Member.Name}";

    string Parse(UnaryExpression ex)
        => ex.NodeType switch
        {
            ExpressionType.Not => "-" + Parse(ex.Operand),
            _ => Parse(ex.Operand)
        };

    string Parse(LambdaExpression ex) => Parse(ex.Body);

    string Parse(MethodCallExpression ex)
    {
        if (ex.Method.Name == "Where")
            return Parse(ex.Arguments[1]);
        if (ex.Method.Name == "Select")
        {
            var operand = ((UnaryExpression)ex.Arguments[1]).Operand;
            if ((operand as dynamic).Body is NewExpression)
            {
                var newExpr = (operand as dynamic).Body as NewExpression;
                _fieldList.AddRange(newExpr.Arguments.Select(arg => (arg as MemberExpression).Member.Name));
                // won't help, since I still can't return an anon type :( 
                return Parse(ex.Arguments[0]);
            }
            else if ((operand as dynamic).Body is MemberInitExpression)
            {
                var initExpr = ((dynamic)operand).Body as MemberInitExpression;
                _fieldList.AddRange(initExpr.Bindings.Select(b => b.Member.Name));
                return Parse(ex.Arguments[0]);
            }
            return Parse(ex.Arguments[0]);
        }
        if (new[] { "First", "FirstOrDefault" }.Contains(ex.Method.Name))
        {
            _page = 1;
            _pageSize = 1;
            _resultKind = ResultKind.Single;
            if (ex.Arguments.Count == 2)
                return Parse(ex.Arguments[1]);
            if (ex.Arguments.Count == 1
                && ex.Arguments[0] is MethodCallExpression methodExpr)
                return Parse(methodExpr.Arguments[0]);
            throw new InvalidOperationException("Unknown operation");
        }
        if (ex.Method.Name == "Any")
        {
            _page = 1;
            _pageSize = 1;
            _resultKind = ResultKind.Bool;
        }
        return Parse(ex.Arguments[0]);
    }

    string Parse(ConstantExpression ex)
    {
        if (ex.Type == typeof(string))
            return ex.Value.ToString();
        if (ex.Type == typeof(int)
            || ex.Type == typeof(long))
            return ex.Value.ToString();
        if (ex.Type == typeof(bool))
            return ((bool)ex.Value) ? "1" : "0";
        return "";
    }

    string Operation(ExpressionType op)
        => op.ToString() switch
        {
            "Equal" => ":",
            "And" => " ",
            "AndAlso" => " ",
            _ => throw new InvalidOperationException($"Do not use the operator '{op}'")
        };
}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
