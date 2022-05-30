using Flurl;

namespace zblesk.Joplin;

public static class QueryBuilder
{
    static readonly Dictionary<Type, string[]> DefaultFieldset = new();
    static readonly Dictionary<Type, string> DefaultApiPaths = new();
    static readonly Dictionary<Type, string> FolderNames = new();

    static QueryBuilder()
    {
        foreach (JoplinData dataType in new JoplinData[] { new Note(), new Notebook() })
        {
            DefaultFieldset[dataType.GetType()] = dataType.DefaultFetchFields.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            DefaultApiPaths[dataType.GetType()] = dataType.EntityApiPath;
            FolderNames[dataType.GetType()] = dataType.SearchType;
        }
    }

    public static Flurl.Url BuildQuery(this QueryParameters pars, string token, string baseUrl = "http://localhost:41184")
    {
        if (pars.queriedTypes.Count != 1)
            throw new Exception($"Exactly one Joplin data type must be queried; found {pars.queriedTypes.Count}");
        var type = pars.queriedTypes.First();

        if (pars.selecting.Count == 0) // select all fields if none specified
            pars.selecting = new HashSet<string>(DefaultFieldset[type]);
        pars.selecting.Add("id"); // make sure ID is always present

        Url url;

        foreach (var fieldReplacement in new Dictionary<string, string> { { "body_html", "body" } }) // todo: datetime exts?
            if (pars.selecting.Contains(fieldReplacement.Key))
            {
                pars.selecting.Remove(fieldReplacement.Key);
                pars.selecting.Add(fieldReplacement.Value);
            }

        if ((
            // If only a single one is requested and ID is present
            pars.RequestedResultKind == ResultKind.Single
             // or if the only filtered field is the ID
             || pars.filterValues.Count == 1)
            && pars.filterValues.Keys.Contains("id"))
        {
            // fetch single by id
            url = baseUrl.AppendPathSegments(DefaultApiPaths[type], pars.filterValues["id"]);
            pars.ApiResponseKind = ResultKind.Single;
        }
        else
        {
            // use search endpoint
            url = baseUrl.AppendPathSegment("search")
                .SetQueryParam("query", pars.filterValues.Aggregate("", (acc, kv) => $"{acc} {kv.Key}:\"{kv.Value}\""));
        }

        // Joplin's naming differs here: it's `source_url` on a note, but `sourceurl` as a search param.
        // _queryText = _queryText.Replace("source_url", "sourceurl");

        url.SetQueryParam("token", token)
            .SetQueryParam("fields", string.Join(',', pars.selecting));
        return url;
    }
}
