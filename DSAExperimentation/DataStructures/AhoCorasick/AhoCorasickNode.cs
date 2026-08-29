using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.DataStructures.AhoCorasick;

// Replicates TrieNode<TValue>'s "HashMap<char,Self> children plus a match marker" PATTERN, not its
// type (ARCHITECTURE.md §5 step 5) - TrieNode<TValue> is sealed (can't be subclassed), and wrapping
// it by composition would still need a side-dictionary to get from a plain TrieNode reached via
// Children back to a richer wrapper carrying Fail/OutputLink, relocating the exact per-character
// hot-path indirection those fields exist to avoid. TrieNode<TValue>.Value is also single-slot
// last-write-wins, wrong for duplicate-pattern handling (PatternIndices needs every match, not just
// the most recent).
internal sealed class AhoCorasickNode
{
    public HashMap<char, AhoCorasickNode> Children { get; }

    public List<int> PatternIndices { get; } = new();

    // Null only for the root - the scan/build fallback loop's stop condition is reference-equality
    // to the automaton's own root, not a null check, so an accidentally-skipped depth-1 base case
    // (see AhoCorasick.cs) fails loudly with a NullReferenceException instead of looping forever.
    public AhoCorasickNode? Fail { get; set; }

    // The nearest proper failure-ancestor with a non-empty PatternIndices - a pointer computed once
    // per node at construction (never a copied list), letting FindAll's scan report every match at
    // a position in O(matches found there) instead of walking the full Fail chain per character.
    public AhoCorasickNode? OutputLink { get; set; }

    public AhoCorasickNode(IEqualityComparer<char> comparer) => Children = new HashMap<char, AhoCorasickNode>(comparer);
}
