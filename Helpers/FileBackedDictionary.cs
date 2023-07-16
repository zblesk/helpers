using System;
using System.Collections;
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
public sealed class FileBackedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _dict;
    private readonly string _fileName;
    private readonly object _lock = new object();

    public FileBackedDictionary(string fileName)
    {
        if (!File.Exists(fileName))
        {
            using var f = File.Create(fileName);
            _dict = new Dictionary<TKey, TValue>();
        }
        else
        {
            _dict = JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(File.ReadAllText(fileName));
        }
        _fileName = fileName;
    }

    /// <summary>
    /// Removes an item from the dictionary
    /// </summary>
    public bool Remove(TKey key)
    {
        bool result;
        lock (_lock)
        {
            result = _dict.Remove(key);
            File.WriteAllText(_fileName, JsonConvert.SerializeObject(_dict, Formatting.Indented));
        }
        return result;
    }

    public bool Contains(TKey item) => _dict.ContainsKey(item);

    public int Count() => _dict.Count;

    public TValue this[TKey key]
    {
        get => _dict[key];
        set
        {
            lock (_lock)
            {
                _dict[key] = value;
                File.WriteAllText(_fileName, JsonConvert.SerializeObject(_dict, Formatting.Indented));
            }
        }
    }

    public IEnumerable<TKey> Keys => _dict.Keys;

    public IEnumerable<TValue> Values => _dict.Values;

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => _dict.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();
}
