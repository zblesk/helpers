using AutoMapper;
using Flurl;
using Flurl.Http;

namespace zblesk.Joplin;

public class JoplinApi
{
    readonly int _port;
    readonly string _token;
    readonly string _url;
    readonly int _defaultPageSize = 100;

    public JoplinDataset<Note> Notes;
    public JoplinDataset<Notebook> Notebooks;

    public JoplinApi(string token, int port = 41184)
    {
        _port = port;
        _token = token;
        _url = $"http://localhost:{port}";

        Notes = new JoplinDataset<Note>(new JoplinQueryProvider(this));
        Notebooks = new JoplinDataset<Notebook>(new JoplinQueryProvider(this));
    }

    public async Task<bool> IsReady()
    {
        try
        {
            await MakeUrl("ping").GetAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<Notebook>> GetAllNotebooks()
    {
        var page = 0;
        var cont = false;
        var results = new List<Notebook>();
        do
        {
            var response = await MakePagedUrl("folders", ++page)
                .GetJsonAsync<dynamic>();
            cont = (bool)response.has_more;
            foreach (var ntb in response.items)
            {
                //results.Add(new Notebook
                //{
                //    NotebookId = (string)ntb.id,
                //    ParentId = (string)ntb.parent_id,
                //    Title = (string)ntb.title,
                //});
            }
        } while (cont);
        return results;
    }

    public object Search<T>(ParseResults searchCriteria)
        where T : JoplinData
    {
        var url = MakeUrl(searchCriteria.endpoint)
            .SetQueryParam("query", searchCriteria.query)
            .SetQueryParam("fields", searchCriteria.fields)
            .SetQueryParam("type", searchCriteria.searchType);
        Console.WriteLine(url);
        var q = url.GetJsonAsync<dynamic>();
        q.Wait();

        var r = new List<T>();

        var config = new MapperConfiguration(cfg => cfg.CreateMap<T, dynamic>());
        var map = config.CreateMapper();

        foreach (var element in q.Result.items)
        {
            T n = map.Map<T>(element);
            r.Add(n);
        }

        if (searchCriteria.kind == ResultKind.Single)
            return r.FirstOrDefault();
        if (searchCriteria.kind == ResultKind.Bool)
            return r.FirstOrDefault() != null;
        return r;
    }


    private string MakeUrl(string path) => _url.AppendPathSegment(path).SetQueryParam("token", _token);
    private string MakeUrl(params string[] path) => _url.AppendPathSegments(path).SetQueryParam("token", _token);
    private string AddPaging(string url, int page) => url.SetQueryParams(new { limit = _defaultPageSize, page = page });
    private string MakePagedUrl(string path, int page) => AddPaging(MakeUrl(path), page);

    public string BuildQuery(QueryParameters parameters)
        => parameters.BuildQuery(_token, _url);

    public string BuildQuery(QueryParameters parameters, int page)
        => parameters.BuildQuery(_token, _url, page);

    /// --- finalne

    public Task<T> Add<T>(T item)
        where T : JoplinData
        => MakeUrl(item.EntityApiPath)
            .PostJsonAsync(item)
            .ReceiveJson<T>();

    public Task<T> Update<T>(T item)
        where T : JoplinData
        => MakeUrl(item.EntityApiPath)
            .AppendPathSegment(item.id)
            .PutJsonAsync(item)
            .ReceiveJson<T>();

    public Task Delete<T>(T item)
        where T : JoplinData
        => MakeUrl(item.EntityApiPath)
            .AppendPathSegment(item.id)
            .DeleteAsync();

    public Task Delete<T>(string id)
        where T : JoplinData, new()
        => MakeUrl(new T().EntityApiPath)
            .AppendPathSegment(id)
            .DeleteAsync();

}
