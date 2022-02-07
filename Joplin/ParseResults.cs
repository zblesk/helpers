namespace zblesk.Joplin;

public enum ResultKind { Single, List }

public record struct ParseResults(string query, string fields, string endpoint, ResultKind kind, int pageSize, int? page);
