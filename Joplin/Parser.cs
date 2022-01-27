using System.Collections.ObjectModel;
using System.Linq.Expressions;
using zblesk.Joplin.Poco;

namespace zblesk.Joplin;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
internal class Parser
{
    readonly List<string> _fieldList = new();
    readonly Expression _expression;
    string? _queryText, _fields;

    static readonly ReadOnlyDictionary<Type, string> DefaultFieldset =
        new(new Dictionary<Type, string>
            {
                { typeof(Note), "id,parent_id,title,body,created_time,updated_time,is_conflict,latitude,longitude,altitude,author,source_url,is_todo,todo_due,todo_completed,source,source_application,application_data,order,user_created_time,user_updated_time,encryption_applied,markup_language,is_shared,share_id,conflict_original_id,master_key_id"}
            });

    public Parser(Expression expression)
    {
        _expression = expression;
    }

    public (string query, string fields) GetQuery<T>()
    {
        if (_queryText == null)
        {
            _queryText = Parse(_expression);
            _fields = string.Join(
                ',',
                _fieldList.Count == 0
                    ? DefaultFieldset[typeof(T)]
                    : string.Join(',', _fieldList));
        }
        return (_queryText, _fields);
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

    string Parse(UnaryExpression ex) => Parse(ex.Operand);

    string Parse(LambdaExpression ex) => Parse(ex.Body);

    string Parse(MethodCallExpression ex)
    {
        if (ex.Method.Name == "Where")
            return Parse(ex.Arguments[1]);
        if (ex.Method.Name == "Select")
        {
            var operand = ((UnaryExpression)ex.Arguments[1]).Operand;
            var bindings = ((dynamic)operand).Body.Bindings;
            foreach (var member in bindings)
            {
                var fieldName = ((member as MemberAssignment).Expression as MemberExpression).Member.Name;
                _fieldList.Add(fieldName);
            }
            return Parse(ex.Arguments[0]);
        }
        throw new InvalidOperationException($"Unknown method: {ex.Method.Name}");
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
        return $"Unknown type {ex.Type}";
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
