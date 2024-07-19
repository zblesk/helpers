
using System.Linq.Expressions;
using System.Text.Json.Nodes;
using zblesk.Helpers;
using zblesk.Joplin;

var devmode_token = "d96248fb2b68b5cc6fd58114846d9809a725d88899295219d454f0d738ff81e173c9df563382ec73cb1456df63dabd3fc657c1deedbcc80673e24f1e1a871d96";
var token = devmode_token;
var port = 27583;

var api = new JoplinApi(token, port);
// Re dyn anon mapping: https://stackoverflow.com/questions/9639451/how-to-map-an-anonymous-object-to-a-class-by-automapper



//var notes = (from note in api.Notes
//             where
//                note.parent_id == "0b433eae117d48928a5cdacea2fcb197"
//             select new Note { title = note.title, source_url = note.source_url }
//            )
//            .Take(11)
//            .ToList();

var w = new MatrixChatroomWatcher("https://matrix.zble.sk", "!vy:zble.sk", "@", "syt_Ym90ZLVE_2HgF7x");
w.NewMessageReceived += (JsonNode message) =>
    {
        Console.WriteLine((string)message.ToString());
    };
await w.Start();

Console.ReadKey();
