using DSAExperimentation.DataStructures.Trie;

namespace DSAExperimentation.LeetCode.ImplementTrie;

// LeetCode 208. Implement Trie (Prefix Tree): a design problem - insert/search/
// startsWith against a fixed word set, not a single return value.
//
// This repo's own Trie<TValue> already implements exactly this shape
// (Set/HasKey/HasPrefix, LeetCode's own verbs traded for this repo's naming - the
// same trade LRUCacheSolution makes over ICache<TKey,TValue>), so composing it
// directly is the only strategy. Neither original harness carried a second
// (baseline) arm, so there is none to promote here.
internal static class ImplementTrieSolution
{
    public static Trie<bool> CreateByTriePrimitive() => new();
}
