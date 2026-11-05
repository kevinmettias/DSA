using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.LeetCode.Solutions.ImplementTrie;

// LeetCode's three calls on the trie, which IPrefixSet - the layer LC 208 asks for - answers under its own
// names (ARCHITECTURE 17.13): insert is Add, search is HasKey and startsWith is HasPrefix.
internal static class ImplementTrieCalls
{
    public static void Insert(IPrefixSet words, string word) => words.Add(word);

    public static bool Search(IPrefixSet words, string word) => words.HasKey(word);

    public static bool StartsWith(IPrefixSet words, string prefix) => words.HasPrefix(prefix);
}
