
using System.Linq.Expressions;
using zblesk.Joplin;

var devmode_token = "388f2954431e126b19fc9748a5dc09a45a0d07cf50cb8098f4890fc966c5725617b33f84d8deba9247c958b2afa783a2bb2af33a66d2eb127608740e437f9b92";
var token = devmode_token;
var port = 27583;

var api = new JoplinApi(token, port);

//var note = api.Notes.FirstOrDefault(n => n.id == "a43192e320cc47b8b20364dc6d8ec605");
var notes = (from note in api.Notes
             where note.title == "This is my"
             select note
            ).ToList();

Console.ReadKey();
