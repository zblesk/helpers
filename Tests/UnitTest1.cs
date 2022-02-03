using Microsoft.VisualStudio.TestTools.UnitTesting;
using zblesk.Joplin;
using System.Linq;
using System.Threading.Tasks;

namespace Tests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        // dev 
        var token = "d96248fb2b68b5cc6fd58114846d9809a725d88899295219d454f0d738ff81e173c9df563382ec73cb1456df63dabd3fc657c1deedbcc80673e24f1e1a871d96";
        var port = 27583;

        //token = "fa0b50d2a5e917d628674d68d2e9d9fc9a4c0d86bdc7d65d193fcfd4e3716ed07b5c19a42063087c7bc9821c96f92637d9e2dd70811dcb47933c5b98b91ff16c";
        //port = 41184;

        var api = new JoplinApi(token, port);

        test(api).Wait();
    }

    private async Task test(JoplinApi api)
    {
        var w = from n in api.Notes
                where n.source_url == "https://zble.sk" & !(n.latitude == "55")
                select new Note { title = n.title, is_conflict = n.is_conflict, latitude = n.latitude, author = n.author, body = n.body };

        var ee = w.ToList();


        return;
        var we = (from n in api.Notebooks
                  select new { n.title })
                .ToList();

        Assert.IsNotNull(api);
        Assert.IsTrue(await api.IsReady());
    }
}
