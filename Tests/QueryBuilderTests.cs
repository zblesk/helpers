using System.Linq;
using System.Linq.Expressions;
using Xunit;
using zblesk.Joplin;

namespace Tests;

public class QueryBuilderTests
{
    JoplinApi api = new JoplinApi("MY_TOKEN");

    [Fact]
    public void SearchUrl()
    {
        Expression e = () => (from n in api.Notes
                              where "2022" == n.title
                              select new { n.id, n.title, n.is_todo, n.body })
                             .Skip(20)
                             .Take(10);
        var v = new QueryVisitor().ExtractParams(e);
        var url = v.BuildQuery("MY_TOKEN").ToString();
        Assert.Equal("http://localhost:41184/search?query=%20title%3A%222022%22&token=MY_TOKEN&fields=id%2Ctitle%2Cis_todo%2Cbody", url);
    }


    [Fact]
    public void SingleNoteFetch()
    {
        Expression e = () => api.Notes.Where(n => n.id == "8f2b0daf302542579fee7c7c55ab8781");
        var v = new QueryVisitor().ExtractParams(e);
        var url = v.BuildQuery("MY_TOKEN").ToString();
        Assert.Equal("http://localhost:41184/notes/8f2b0daf302542579fee7c7c55ab8781?token=MY_TOKEN&fields=id%2Cparent_id%2Ctitle%2Cbody%2Ccreated_time%2Cupdated_time%2Cis_conflict%2Clatitude%2Clongitude%2Caltitude%2Cauthor%2Csource_url%2Cis_todo%2Ctodo_due%2Ctodo_completed%2Csource%2Csource_application%2Capplication_data%2Corder%2Cuser_created_time%2Cuser_updated_time%2Cencryption_applied%2Cencryption_cipher_text%2Cmarkup_language%2Cis_shared%2Cshare_id%2Cconflict_original_id%2Cmaster_key_id",
            url);
    }

    [Fact]
    public void SingleNoteSelect()
    {
        var w = new QueryVisitor();

        Expression e = () => api.Notes.Where(n => n.id == "a");
        var v = w.ExtractParams(e);
        var url = v.BuildQuery("MY_TOKEN").ToString();
        Assert.Equal("http://localhost:41184/notes/a?token=MY_TOKEN&fields=id%2Cparent_id%2Ctitle%2Cbody%2Ccreated_time%2Cupdated_time%2Cis_conflict%2Clatitude%2Clongitude%2Caltitude%2Cauthor%2Csource_url%2Cis_todo%2Ctodo_due%2Ctodo_completed%2Csource%2Csource_application%2Capplication_data%2Corder%2Cuser_created_time%2Cuser_updated_time%2Cencryption_applied%2Cencryption_cipher_text%2Cmarkup_language%2Cis_shared%2Cshare_id%2Cconflict_original_id%2Cmaster_key_id",
            url);


        e = () => api.Notebooks.Where(n => n.id == "a");
        v = w.ExtractParams(e);
        url = v.BuildQuery("MY_TOKEN").ToString();
        Assert.Equal("http://localhost:41184/folders/a?token=MY_TOKEN&fields=id%2Ctitle%2Ccreated_time%2Cupdated_time%2Cuser_created_time%2Cuser_updated_time%2Cencryption_cipher_text%2Cencryption_applied%2Cparent_id%2Cis_shared%2Cshare_id%2Cmaster_key_id%2Cicon",
            url);

    }


    [Fact]
    public void SingleNoteFirstOrDefault()
    {
        Expression e = () => api.Notebooks.FirstOrDefault(n => n.id == "8f2b0daf302542579fee7c7c55ab8781");
        var v = new QueryVisitor().ExtractParams(e);
        var url = QueryBuilder.BuildQuery(v, "MY_TOKEN").ToString();
        Assert.Equal("http://localhost:41184/folders/8f2b0daf302542579fee7c7c55ab8781?token=MY_TOKEN&fields=id%2Ctitle%2Ccreated_time%2Cupdated_time%2Cuser_created_time%2Cuser_updated_time%2Cencryption_cipher_text%2Cencryption_applied%2Cparent_id%2Cis_shared%2Cshare_id%2Cmaster_key_id%2Cicon",
            url);
    }

    [Fact]
    public void DefaultFieldsets()
    {

    }
}
