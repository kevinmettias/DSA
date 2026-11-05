namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The map a trie is to its callers: values stored under string keys, and whether any stored key starts
// with a prefix. Every configuration of Trie<TStore, TValue> implements it, and so may a hand-written
// trie that specializes the whole map, so one contract suite holds them all to the same answers and a
// strategy benchmark can compare them (the owner's ruling of 2026-11-02, ARCHITECTURE §17.5).
/// <summary>Values stored under string keys, with prefix queries over the keys.</summary>
/// <typeparam name="TValue">The value stored under each key.</typeparam>
public interface IPrefixMap<TValue>
{
    /// <summary>Gets how many distinct keys are stored.</summary>
    int Count { get; }

    /// <summary>Stores a value under a key, replacing any value already stored there.</summary>
    /// <param name="key">The key to store under; the empty key is a key like any other.</param>
    /// <param name="value">The value to store.</param>
    void Set(string key, TValue value);

    /// <summary>Reports whether a value is stored under a key.</summary>
    /// <param name="key">The key to look for.</param>
    /// <returns>True when the key itself was stored; a key that is only a prefix of stored keys is not.</returns>
    bool HasKey(string key);

    /// <summary>Reads the value stored under a key.</summary>
    /// <param name="key">The key to look up.</param>
    /// <param name="value">The value stored under the key, when there is one.</param>
    /// <returns>True when a value is stored under the key.</returns>
    bool TryGetValue(string key, out TValue value);

    /// <summary>Reports whether any stored key starts with a prefix.</summary>
    /// <param name="prefix">The prefix to look for; every key starts with the empty prefix.</param>
    /// <returns>True when some stored key starts with the prefix, so the empty prefix asks whether any key is stored.</returns>
    bool HasPrefix(string prefix);
}
