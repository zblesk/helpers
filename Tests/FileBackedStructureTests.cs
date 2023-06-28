using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Xunit.Sdk;
using zblesk.Helpers;
using zblesk.Joplin;

namespace Tests;

public class FileBackedStructureTests
{ 
    [Fact]
    void FileBackedSet()
    {
        if (File.Exists("inthashtest.json"))
        {
            File.Delete("inthashtest.json");
        }

        var fbs = new FileBackedHashSet<int>("inthashtest.json");
        Assert.Equal(0, fbs.Count());
        fbs.Add(3);
        fbs.Add(1);
        fbs.Add(3);
        fbs.Add(3);
        fbs.Add(7);
        Assert.Equal(3, fbs.Count());
        Assert.True(File.Exists("inthashtest.json"));
        var json = File.ReadAllText("inthashtest.json");
        Assert.Equal("[\r\n  3,\r\n  1,\r\n  7\r\n]", json);
        fbs.Remove(4);
        fbs.Remove(1);
        Assert.Equal(2, fbs.Count());
        json = File.ReadAllText("inthashtest.json");
        Assert.Equal("[\r\n  3,\r\n  7\r\n]", json);

        File.Delete("inthashtest.json");
    }

    [Fact]
    void FileBackedDictionary()
    {
        if (File.Exists("strintdicttest.json"))
        {
            File.Delete("strintdicttest.json");
        }

        var fbd = new FileBackedDictionary<string, int>("strintdicttest.json");
        Assert.Equal(0, fbd.Count());
        fbd["jiggly"] = 6;
        fbd["puff"] = 9;
        fbd["jiggly"] = 42;
        Assert.Equal(2, fbd.Count());
        Assert.True(File.Exists("strintdicttest.json"));
        Assert.Equal(
            "{\r\n  \"jiggly\": 42,\r\n  \"puff\": 9\r\n}",
            File.ReadAllText("strintdicttest.json"));
        fbd.Remove("puff");
        Assert.Equal(1, fbd.Count());
        Assert.Equal(
            "{\r\n  \"jiggly\": 42\r\n}",
            File.ReadAllText("strintdicttest.json"));

        File.Delete("strintdicttest.json");
    }
}
