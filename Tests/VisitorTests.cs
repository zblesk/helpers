﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using zblesk.Joplin;

namespace Tests;

public class VisitorTests
{
    JoplinApi api = new JoplinApi("");

    void DictionariesEqual<T, S>(Dictionary<T, S> one, Dictionary<T, S> two)
    {
        if (one == null || two == null)
        {
            Assert.Null(one);
            Assert.Null(two);
            return;
        }
        Assert.Equal(one.Count, two.Count);
        foreach (var kv in one)
        {
            Assert.True(two.Keys.Contains(kv.Key), $"Value missing {kv.Key}");
            Assert.Equal(kv.Value, two[kv.Key]);
        }
    }

    void SetsEqual<T>(HashSet<T> one, HashSet<T> two)
    {
        Assert.True(one.Intersect(two).Count() == one.Count);
    }

    [Fact]
    void SimpleWhere()
    {
        Expression e = () => api.Notebooks.Where(n => n.title == "Odklada*");

        var v = new QueryVisitor().ExtractParams(e);

        Assert.Empty(v.selecting);
        Assert.Contains(typeof(Notebook), v.queriedTypes);
        Assert.Single(v.filterValues);
        Assert.Contains("title", v.filterValues.Keys);
        Assert.Contains("Odklada*", v.filterValues.Values);
        Assert.Equal(1, v.page);
        Assert.Equal(100, v.limit);
        Assert.Equal(ResultKind.List, v.RequestedResultKind);
    }

    [Fact]
    void FirstWithWhere()
    {
        Expression e = () => api.Notes.Where(n => n.title == "my title").First();

        var v = new QueryVisitor().ExtractParams(e);

        Assert.Empty(v.selecting);
        Assert.Contains(typeof(Note), v.queriedTypes);
        Assert.Single(v.filterValues);
        Assert.Contains("title", v.filterValues.Keys);
        Assert.Contains("my title", v.filterValues.Values);
        Assert.Equal(1, v.page);
        Assert.Equal(1, v.limit);
        Assert.Equal(ResultKind.Single, v.RequestedResultKind);
    }

    [Fact]
    void FirstNoWhere()
    {
        Expression e = () => api.Notes.First(n => n.title == "my title");

        var v = new QueryVisitor().ExtractParams(e);

        Assert.Empty(v.selecting);
        Assert.Contains(typeof(Note), v.queriedTypes);
        Assert.Single(v.filterValues);
        Assert.Contains("title", v.filterValues.Keys);
        Assert.Contains("my title", v.filterValues.Values);
        Assert.Equal(1, v.page);
        Assert.Equal(1, v.limit);
        Assert.Equal(ResultKind.Single, v.RequestedResultKind);
    }

    [Fact]
    void FirstOrDefault()
    {
        Expression e = () => api.Notes.FirstOrDefault(n => n.title == "oh title");

        var v = new QueryVisitor().ExtractParams(e);

        Assert.Empty(v.selecting);
        Assert.Contains(typeof(Note), v.queriedTypes);
        Assert.Single(v.filterValues);
        Assert.Contains("title", v.filterValues.Keys);
        Assert.Contains("oh title", v.filterValues.Values);
        Assert.Equal(1, v.page);
        Assert.Equal(1, v.limit);
        Assert.Equal(ResultKind.Single, v.RequestedResultKind);
    }

    [Fact]
    void LinqLeftRightEquals()
    {
        Expression e = () => from n in api.Notes
                             where "left" == n.title && n.body == "right"
                             select new Note { id = n.id, title = n.title };

        var v = new QueryVisitor().ExtractParams(e);

        SetsEqual(v.selecting, new HashSet<string> { "id", "title" });
        Assert.Contains(typeof(Note), v.queriedTypes);
        DictionariesEqual(v.filterValues, new Dictionary<string, string> { { "title", "left" }, { "body", "right" } });
        Assert.Equal(1, v.page);
        Assert.Equal(100, v.limit);
        Assert.Equal(ResultKind.List, v.RequestedResultKind);
    }

    [Fact]
    void SkipAndTake()
    {
        Expression e = () => (from n in api.Notes
                              where "left" == n.title && n.body == "right"
                              select new { id = n.id, title = n.title })
                             .Skip(20)
                             .Take(10);

        var v = new QueryVisitor().ExtractParams(e);

        SetsEqual(v.selecting, new HashSet<string> { "id", "title" });
        Assert.Contains(typeof(Note), v.queriedTypes);
        DictionariesEqual(v.filterValues, new Dictionary<string, string> { { "title", "left" }, { "body", "right" } });
        Assert.Equal(1, v.page);
        Assert.Equal(100, v.limit);
        Assert.Equal(20, v.skip);
        Assert.Equal(10, v.take);
        Assert.Equal(ResultKind.List, v.RequestedResultKind);
    }

    [Fact]
    void UnsupportedOperations()
    {
        Expression e = () => api.Notebooks.Single();

        var v = new QueryVisitor();
        Assert.Throws<InvalidOperationException>(() => v.ExtractParams(e));

        e = () => api.Notes.First(n => n.id == "a" || n.id == "b");
        Assert.Throws<InvalidOperationException>(() => v.ExtractParams(e));
    }


    [Fact]
    void SimpleFirst()
    {
        Expression e = () => api.Notes.First(n => n.title == "Odklada*");

        var v = new QueryVisitor().ExtractParams(e);

        Assert.Empty(v.selecting);
        Assert.Contains(typeof(Note), v.queriedTypes);
        Assert.Single(v.filterValues);
        Assert.Contains("title", v.filterValues.Keys);
        Assert.Contains("Odklada*", v.filterValues.Values);
        Assert.Equal(1, v.page);
        Assert.Equal(1, v.limit);
        Assert.Equal(ResultKind.Single, v.RequestedResultKind);
    }

    [Fact]
    void FirstOrDefaultById()
    {
        Expression e = () => api.Notes.FirstOrDefault(n => n.id == "1");

        var v = new QueryVisitor().ExtractParams(e);

        Assert.Empty(v.selecting);
        Assert.Contains(typeof(Note), v.queriedTypes);
        Assert.Single(v.filterValues);
        Assert.Contains("id", v.filterValues.Keys);
        Assert.Contains("1", v.filterValues.Values);
        Assert.Equal(1, v.page);
        Assert.Equal(1, v.limit);
        Assert.Equal(ResultKind.Single, v.RequestedResultKind);
    }

    [Fact]
    void WhereSelectMethodSyntax()
    {
        Expression e = () => api.Notes.Where(n => n.id == "8f2b0daf302542579fee7c7c55ab8781").Select(n => new { n.created_time });

        var v = new QueryVisitor().ExtractParams(e);

        Assert.NotEmpty(v.selecting);
        Assert.Contains("created_time", v.selecting);
        Assert.Contains(typeof(Note), v.queriedTypes);
        Assert.Single(v.filterValues);
        Assert.Contains("id", v.filterValues.Keys);
        Assert.Contains("8f2b0daf302542579fee7c7c55ab8781", v.filterValues.Values);
        Assert.Equal(1, v.page);
        Assert.Equal(100, v.limit);
        Assert.Equal(ResultKind.List, v.RequestedResultKind);
    }

    [Fact]
    void WhereSelectLinqSyntax()
    {
        Expression e = () => from n in api.Notes
                             where n.id == "8f2b0daf302542579fee7c7c55ab8781"
                             select new { n.id, n.created_time };

        var v = new QueryVisitor().ExtractParams(e);

        Assert.NotEmpty(v.selecting);
        Assert.Contains("created_time", v.selecting);
        Assert.Contains(typeof(Note), v.queriedTypes);
        Assert.Single(v.filterValues);
        Assert.Contains("id", v.filterValues.Keys);
        Assert.Contains("8f2b0daf302542579fee7c7c55ab8781", v.filterValues.Values);
        Assert.Equal(1, v.page);
        Assert.Equal(100, v.limit);
        Assert.Equal(ResultKind.List, v.RequestedResultKind);
    }
}
