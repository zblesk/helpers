
using AutoMapper;
using Flurl;
using Flurl.Http;
using zblesk.Joplin.Poco;

namespace zblesk.Joplin;

public class JoplinApi
{
    readonly int _port;
    readonly string _token;
    readonly string _url;
    readonly int _defaultPageSize = 100;

    public Query<Note> Notes;
    public Query<Notebook> Notebooks;

    public JoplinApi(string token, int port = 41184)
    {
        _port = port;
        _token = token;
        _url = $"http://localhost:{port}";

        Notes = new Query<Note>(new JoplinQueryProvider(this, typeof(Note)));
        Notebooks = new Query<Notebook>(new JoplinQueryProvider(this, typeof(Notebook)));
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
                .GetJsonAsync();
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

    public async Task<string> CreateMarkdownNote(string notebookId, string title, string body, string url = null, NoteType type = NoteType.Text) 
        => await MakeUrl("notes")
            .PostJsonAsync(new
            {
                title = title,
                body = body,
                parent_id = notebookId,
                source_url = url,
                is_todo = (int)type,
            })
            .ReceiveString();

    public async Task<string> CreateHtmlNote(string notebookId, string title, string body, string url = null, NoteType type = NoteType.Text) 
        => await MakeUrl("notes")
            .PostJsonAsync(new
            {
                title = title,
                body_html = body,
                parent_id = notebookId,
                source_url = url,
                is_todo = (int)type,
            })
            .ReceiveString();

    public Task<dynamic> GetNote(string noteId)
        => MakeUrl("notes", noteId)
        .SetQueryParam("fields", "*")
        .GetJsonAsync();

    public List<T> Search<T>(string query, string fields, string endpoint)
        where T : JoplinData
    {
        var url = MakeUrl(endpoint)
            .SetQueryParam("query", query)
            .SetQueryParam("fields", fields);
        var q = url.GetJsonAsync();
        q.Wait();

        var r = new List<T>();

        var config = new MapperConfiguration(cfg => cfg.CreateMap<T, dynamic>());
        var map = config.CreateMapper();

        foreach (var element in q.Result.items)
        {
            T n = map.Map<T>(element);
            r.Add(n);
        }
        return r;
    }

    private string MakeUrl(string path) => _url.AppendPathSegment(path).SetQueryParam("token", _token);
    private string MakeUrl(params string[] path) => _url.AppendPathSegments(path).SetQueryParam("token", _token);
    private string AddPaging(string url, int page) => url.SetQueryParams(new { limit = _defaultPageSize, page = page });
    private string MakePagedUrl(string path, int page) => AddPaging(MakeUrl(path), page);
}
