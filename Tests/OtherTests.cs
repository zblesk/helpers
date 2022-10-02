using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Xunit.Sdk;
using zblesk.Joplin;

namespace Tests;

public class OtherTests
{
    [Fact]
    void MakeNoteMarkdownLink()
    {
        var note = new Note();
        Assert.Throws<InvalidOperationException>(() => note.GetMarkdownLink());
        note.id = "8f2b0daf302542579fee7c7c55ab8781";
        Assert.Equal("[](:/8f2b0daf302542579fee7c7c55ab8781)", note.GetMarkdownLink());
        Assert.Equal("[My Title](:/8f2b0daf302542579fee7c7c55ab8781)", note.GetMarkdownLink("My Title"));
        note.title = "ultratitle";
        Assert.Equal("[My Title 2](:/8f2b0daf302542579fee7c7c55ab8781)", note.GetMarkdownLink("My Title 2"));
        Assert.Equal("[ultratitle](:/8f2b0daf302542579fee7c7c55ab8781)", note.GetMarkdownLink());
    }
}
