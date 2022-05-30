namespace zblesk.Joplin;

public class QueryParameters
{
    public HashSet<string> selecting = new();
    public HashSet<Type> queriedTypes = new();
    public Dictionary<string, string> filterValues = new();
    public int page = 1;
    public int limit = 100;
    public int? take = null;
    public int? skip = null;
    public ResultKind RequestedResultKind = ResultKind.List;
    public ResultKind ApiResponseKind = ResultKind.List;
    public Type? ReturnType = null;
}
