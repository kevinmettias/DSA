using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmarks-project twin of DSAExperimentation.Tests's own
// LeetCodeCoverage/DeleteDuplicateFoldersInSystem/Fixtures/FolderNode.cs
// (VisitStateNode's existing precedent for a fixture duplicated per project, since
// Benchmarks only references the production DSAExperimentation project, not
// Tests). Children is keyed by folder name, composing this repo's own HashMap
// directly rather than the char-keyed DataStructures.Trie.Trie&lt;TValue&gt;.
internal sealed class FolderNode
{
    public HashMap<string, FolderNode> Children { get; } = new();

    public string Signature { get; set; } = string.Empty;

    public bool Deleted { get; set; }
}
