namespace zblesk.Joplin;

public enum ResultKind { Single, List, Any }

public record struct ParseResults(string query, string fields, string endpoint, ResultKind kind, int pageSize, int? page, string searchType);
