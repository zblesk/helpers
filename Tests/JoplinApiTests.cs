using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using zblesk.Joplin;


namespace Tests;

/// <summary>
/// Requires a running dev instance of Joplin with the prepared test content.
/// </summary>
public class JoplinApiTests
{
    JoplinApi api = new JoplinApi(
        "d96248fb2b68b5cc6fd58114846d9809a725d88899295219d454f0d738ff81e173c9df563382ec73cb1456df63dabd3fc657c1deedbcc80673e24f1e1a871d96",
        27583);

    /// <summary>
    /// When returning a list, but calling the 'retrieve-by-id' API.
    /// </summary>
    [Fact]
    public void FilterListReducedToSingle()
    {
        var noteId = "a11a325cc04a453c92ff56da279e0e05";
        var results = api.Notes.Where(n => n.id == noteId).ToList();
        Assert.NotNull(results);
        Assert.Single(results);
        var note = results[0];
        Assert.Equal(noteId, note.id);
        Assert.Equal("This is my test note", note.title);
        // Careful: Joplin returns \n, not \r\n.
        Assert.Equal("With some body and **bold text**, some 文字 and 🈁️. \n\n==That's Smart™.==", note.body);
        Assert.NotNull(note.created_time);
        Assert.NotNull(note.source);
        Assert.Equal("08a1e7a29b214950b40a5de60f347c91", note.parent_id);
    }

    /// <summary>
    /// When returning a list, but calling the 'retrieve-by-id' API.
    /// </summary>
    [Fact]
    public void FilterSingle()
    {
        var note = api.Notes.FirstOrDefault(n => n.id == "a43192e320cc47b8b20364dc6d8ec605");
        Assert.NotNull(note);
        Assert.Equal("a43192e320cc47b8b20364dc6d8ec605", note!.id);
        Assert.Equal("This is my test note", note.title);
        // Careful: Joplin returns \n, not \r\n.
        Assert.Equal("With some body and **bold text**, some 文字 and 🈁️. \n\n==That's Smart™.==", note.body);
        Assert.NotNull(note.created_time);
        Assert.NotNull(note.source);
        Assert.Equal("08a1e7a29b214950b40a5de60f347c91", note.parent_id);
    }

    [Fact]
    public void FetchMultiple()
    {
        var notes = (from note in api.Notes
                     where note.title == "This is my"
                     select note
                    ).ToList();
        Assert.NotNull(notes);
        // At _least_ 3; can be more, if more are present in the running Joplin instance.
        Assert.True(notes.Count >= 3);
        foreach (var note in notes)
        {
            Assert.Contains("This is my", note.title);
            Assert.NotNull(note.body);
            Assert.NotEmpty(note.body);
            Assert.NotNull(note.created_time);
            Assert.NotNull(note.source);
            Assert.NotEmpty(note.source);
        }
        foreach (var (noteId, parentId) in new[] {
            ("a43192e320cc47b8b20364dc6d8ec605", "08a1e7a29b214950b40a5de60f347c91"),
            ("5f65a1261fb846c287188f0d2d863343", "08a1e7a29b214950b40a5de60f347c91"),
            ("b5996d13c44b4f89a19bb16855ade76a", "60d0adf25d1a4648a386c7656acb5579")
        })
        {
            Assert.Contains(notes, n => n.id == noteId);
            var note = notes.First(n => n.id == noteId);
            Assert.Equal(parentId, note.parent_id);
        }
    }

    [Fact]
    public void FetchMultipleSelectedExplicitType()
    {
        var notes = (from note in api.Notes
                     where note.title == "This is my"
                     select new Note { title = note.title, source_url = note.source_url }
                    ).ToList();
        Assert.NotNull(notes);
        // At _least_ 3; can be more, if more are present in the running Joplin instance.
        Assert.True(notes.Count >= 3);
        foreach (var note in notes)
        {
            Assert.Contains("This is my", note.title);
            Assert.Null(note.body);
            Assert.Null(note.created_time);
            Assert.Null(note.source);
            Assert.Null(note.body_html);
        }
        foreach (var (noteId, parentId) in new[] {
            ("a43192e320cc47b8b20364dc6d8ec605", "08a1e7a29b214950b40a5de60f347c91"),
            ("5f65a1261fb846c287188f0d2d863343", "08a1e7a29b214950b40a5de60f347c91"),
            ("b5996d13c44b4f89a19bb16855ade76a", "60d0adf25d1a4648a386c7656acb5579")
        })
        {
            Assert.Contains(notes, n => n.id == noteId);
            var note = notes.First(n => n.id == noteId);
            Assert.Null(note.parent_id);
            Assert.DoesNotContain(notes, n => n.id == parentId);
        }
        Assert.Equal("https://zblesk.net/", notes.First(n => n.id == "5f65a1261fb846c287188f0d2d863343").source_url);
        Assert.Equal("", notes.First(n => n.id == "a43192e320cc47b8b20364dc6d8ec605").source_url);
        Assert.Equal("", notes.First(n => n.id == "b5996d13c44b4f89a19bb16855ade76a").source_url);
    }
}
