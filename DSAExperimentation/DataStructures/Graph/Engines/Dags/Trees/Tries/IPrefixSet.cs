namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The set a trie is to a caller that keeps keys and no values, as LC 208 keeps its words. The one trie
// is one through PrefixSet, its view of a map to bool, and a hand-written trie that keeps only an
// end-of-word mark may implement it too, so one contract suite holds them all to the same answers and
// a strategy benchmark compares them (the owner's ruling of 2026-11-02, ARCHITECTURE §17.5). It is not
// IPrefixMap with the values dropped by convention: a trie that keeps no values should not have to
// invent one to meet a map's contract.
/// <summary>String keys kept without values, with prefix queries over the keys.</summary>
public interface IPrefixSet
{
    /// <summary>Gets how many distinct keys are stored.</summary>
    int Count { get; }

    /// <summary>Adds a key; adding a key already stored changes nothing.</summary>
    /// <param name="key">The key to add; the empty key is a key like any other.</param>
    void Add(string key);

    /// <summary>Reports whether a key was added.</summary>
    /// <param name="key">The key to look for.</param>
    /// <returns>True when the key itself was added; a key that is only a prefix of added keys is not.</returns>
    bool HasKey(string key);

    /// <summary>Reports whether any stored key starts with a prefix.</summary>
    /// <param name="prefix">The prefix to look for; every key starts with the empty prefix.</param>
    /// <returns>True when some stored key starts with the prefix, so the empty prefix asks whether any key is stored.</returns>
    bool HasPrefix(string prefix);
}
