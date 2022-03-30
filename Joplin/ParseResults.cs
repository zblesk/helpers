namespace zblesk.Joplin;


public record struct ParseResults(string query, string fields, string endpoint, ResultKind kind, int pageSize, int? page, string searchType);
