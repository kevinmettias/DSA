namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// The alphabet LowercaseTrieNode is bounded to: the 26 lowercase English letters.
//
// Owned here rather than in LowercaseTrieNode because every user of the trie names
// it - the trie's own index guard, the LeetCode solutions that loop over the
// alphabet to look for an edit, and the topology tests that assert the children
// array's length - and the node is only one of those readers.
internal static class LowercaseAlphabet
{
    public const int Size = 26;
}
