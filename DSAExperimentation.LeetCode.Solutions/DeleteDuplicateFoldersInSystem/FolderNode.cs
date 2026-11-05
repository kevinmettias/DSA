using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.Solutions.DeleteDuplicateFoldersInSystem;

// One node per folder name in LC 1948's path tree. Children is keyed by folder name (a
// string), so it composes this repo's own HashMap directly rather than reaching for the
// one trie (Graph/Engines/Dags/Trees/Tries), whose keys are strings walked one character
// per edge: here an edge is a whole folder name, which no child store of that trie takes.
//
// It lives beside the solution rather than under DataStructures/ because Signature and
// Deleted are LC 1948's own marking semantics, not a reusable shape (ARCHITECTURE
// §17.3): a name-keyed tree node carrying "my canonical serialization" and "the file
// system will delete me" answers this problem and nothing else.
internal sealed class FolderNode
{
    public HashMap<string, FolderNode> Children { get; } = new();

    // Left empty for a leaf (zero children). The signature pass never inserts a leaf's
    // signature into the duplicate-count map, so an empty Signature here is never
    // mistaken for two unrelated leaves being duplicates of one another - which is
    // exactly what the problem's "same NON-EMPTY set of identical subfolders" wording
    // requires.
    public string Signature { get; set; } = string.Empty;

    public bool Deleted { get; set; }
}
