using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DeleteDuplicateFoldersInSystem.Fixtures;

// One node per folder name in the path tree; Children is keyed by folder name (a
// string), so it composes this repo's own HashMap directly rather than reaching for
// DataStructures.Trie.Trie<TValue>, whose Children are necessarily char-keyed - see
// Trie.cs's own doc comment on why that representation isn't swapped out for an
// arbitrary TKey.
internal sealed class FolderNode
{
    public HashMap<string, FolderNode> Children { get; } = new();

    // Left empty for a leaf (zero children). DeleteDuplicateFoldersInSystemTests's
    // Serialize never inserts a leaf's signature into the duplicate-count map, so an
    // empty Signature here is never mistaken for two unrelated leaves being
    // duplicates of one another.
    public string Signature { get; set; } = string.Empty;

    public bool Deleted { get; set; }
}
