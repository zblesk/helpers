using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace zblesk.Helpers;

/// <summary>
/// A hash class that can persist its contents between program runs in a JSON file. Mostly intended for debugging/one-off tasks.
/// </summary>
public sealed class FileBackedHashSet<T>
{
    private readonly HashSet<T> _set;
    private readonly string _fileName;
    private readonly object _lock = new object();

    public FileBackedHashSet(string fileName)
    {
        if (!File.Exists(fileName))
        {
            using var f = File.Create(fileName);
            _set = new HashSet<T>();
        }
        else
        {
            _set = JsonConvert.DeserializeObject<HashSet<T>>(File.ReadAllText(fileName));
        }
        _fileName = fileName;
    }


    /// <summary>
    /// Adds an item to the set
    /// </summary>
    /// <param name="item">The item to add</param>
    /// <returns>True if added, false if already present</returns>
    public bool Add(T item)
    {
        bool result;
        lock (_lock)
        {
            result = _set.Add(item);
            File.WriteAllText(_fileName, JsonConvert.SerializeObject(_set, Formatting.Indented));
        }
        return result;
    }


    /// <summary>
    /// Removes an item from the set
    /// </summary>
    /// <param name="item">The item to remove</param>
    /// <returns>True if removed, false if wasn't present in set</returns>
    public bool Remove(T item)
    {
        bool result;
        lock (_lock)
        {
            result = _set.Remove(item);
            File.WriteAllText(_fileName, JsonConvert.SerializeObject(_set, Formatting.Indented));
        }
        return result;
    }

    public bool Contains(T item) => _set.Contains(item);

    public int Count() => _set.Count;
}
