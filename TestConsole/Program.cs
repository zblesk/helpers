
using System.Linq.Expressions;
using zblesk.Joplin;

var devmode_token = "388f2954431e126b19fc9748a5dc09a45a0d07cf50cb8098f4890fc966c5725617b33f84d8deba9247c958b2afa783a2bb2af33a66d2eb127608740e437f9b92";
var token = devmode_token;
var port = 27583;

var api = new JoplinApi(token, port);

var t = new QueryVisitor();

Expression e = () => (from n in api.Notes
                      where "2022" == n.title
                      select new { n.id, n.title, n.is_todo, n.body })
                     .Skip(20)
                     .Take(10);

Console.ReadKey();
