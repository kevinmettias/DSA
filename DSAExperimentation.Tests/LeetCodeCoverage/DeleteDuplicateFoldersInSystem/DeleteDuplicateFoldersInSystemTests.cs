using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.Tests.LeetCodeCoverage.DeleteDuplicateFoldersInSystem.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DeleteDuplicateFoldersInSystem;

// LeetCode 1948. Delete Duplicate Folders in System: build a folder-name-keyed tree
// (Fixtures/FolderNode.cs, composing this repo's own HashMap the way TrieNode
// composes it for chars), then one post-order pass canonically serializes every
// non-leaf subtree - a leaf is never inserted into the signature-count map, so two
// unrelated originally-empty folders are never mistaken for duplicates of each
// other, which is exactly what keeps a lone leaf like "/d/a" below from being
// deleted just because other, unrelated leaves elsewhere are also empty - sorting
// each node's "name(childSignature)" pieces via this repo's own MergeSort
// (RemoveSubFoldersFromTheFilesystemTests precedent) so insertion order never
// affects the signature, while a HashMap<string,int> counts repeats. Any non-leaf
// whose signature repeats is marked deleted; a final pre-order walk emits the path
// of every surviving folder and never descends into a deleted one, which is what
// makes deletion cascade to every descendant for free.
public sealed partial class DeleteDuplicateFoldersInSystemTests
{
    [Fact]
    public void DeleteDuplicateFolders_LeetCodeExampleOne_DeletesBothIdenticalTopLevelFolders()
    {
        string[][] paths = [["a"], ["c"], ["d"], ["a", "b"], ["c", "b"], ["d", "a"]];

        var result = DeleteDuplicateFolders(paths);

        AssertSamePaths([["d"], ["d", "a"]], result);
    }

    [Fact]
    public void DeleteDuplicateFolders_DuplicateSubtreeAtTwoLevels_DeletesAllCopiesKeepsUniqueSibling()
    {
        // "a" and "b" are exact duplicates of each other two levels deep (each has
        // children x -> {y} and z), so BOTH are deleted entirely, not just one -
        // this problem deletes every copy, unlike "keep one" duplicate-subtree
        // problems. "c" has a differently-shaped single child and survives.
        string[][] paths =
        [
            ["a"], ["a", "x"], ["a", "x", "y"], ["a", "z"],
            ["b"], ["b", "x"], ["b", "x", "y"], ["b", "z"],
            ["c"], ["c", "x"],
        ];

        var result = DeleteDuplicateFolders(paths);

        AssertSamePaths([["c"], ["c", "x"]], result);
    }

    [Fact]
    public void DeleteDuplicateFolders_MultipleUnrelatedEmptyFolders_NeverTreatsLeavesAsDuplicates()
    {
        string[][] paths = [["a"], ["b"], ["c"]];

        var result = DeleteDuplicateFolders(paths);

        AssertSamePaths([["a"], ["b"], ["c"]], result);
    }

    private static void AssertSamePaths(string[][] expected, List<string[]> actual)
    {
        var expectedSet = expected.Select(p => string.Join('/', p)).ToHashSet();
        var actualSet = actual.Select(p => string.Join('/', p)).ToHashSet();
        Assert.Equal(expectedSet, actualSet);
    }

    private static List<string[]> DeleteDuplicateFolders(string[][] paths)
    {
        var root = BuildTree(paths);

        var signatureCounts = new HashMap<string, int>();
        Serialize(root, signatureCounts);
        MarkDuplicates(root, signatureCounts);

        var result = new List<string[]>();
        Collect(root, new List<string>(), result);
        return result;
    }

    private static FolderNode BuildTree(string[][] paths)
    {
        var root = new FolderNode();

        foreach (var path in paths)
        {
            var current = root;

            foreach (var name in path)
            {
                if (!current.Children.TryGetValue(name, out var next))
                {
                    next = new FolderNode();
                    current.Children.Set(name, next);
                }

                current = next;
            }
        }

        return root;
    }

    // Leaves return without ever touching signatureCounts, so a leaf's empty
    // Signature can never collide with another leaf's in the duplicate-count map.
    private static void Serialize(FolderNode node, HashMap<string, int> signatureCounts)
    {
        if (node.Children.Count == 0)
        {
            return;
        }

        var pieces = new string[node.Children.Count];
        var i = 0;

        foreach (var name in node.Children.Keys)
        {
            node.Children.TryGetValue(name, out var child);
            Serialize(child, signatureCounts);
            pieces[i++] = name + "(" + child.Signature + ")";
        }

        var sequence = new ArrayIndexedSequence<string>(pieces);
        MergeSort.Sort<string, ArrayIndexedSequence<string>>(sequence, StringComparer.Ordinal);

        node.Signature = string.Concat(pieces);
        var newCount = signatureCounts.TryGetValue(node.Signature, out var existing) ? existing + 1 : 1;
        signatureCounts.Set(node.Signature, newCount);
    }

    private static void MarkDuplicates(FolderNode node, HashMap<string, int> signatureCounts)
    {
        foreach (var name in node.Children.Keys)
        {
            node.Children.TryGetValue(name, out var child);

            if (child.Children.Count > 0 && signatureCounts.TryGetValue(child.Signature, out var count) && count > 1)
            {
                child.Deleted = true;
            }

            MarkDuplicates(child, signatureCounts);
        }
    }

    private static void Collect(FolderNode node, List<string> pathSoFar, List<string[]> result)
    {
        foreach (var name in node.Children.Keys)
        {
            node.Children.TryGetValue(name, out var child);

            if (child.Deleted)
            {
                continue;
            }

            pathSoFar.Add(name);
            result.Add(pathSoFar.ToArray());
            Collect(child, pathSoFar, result);
            pathSoFar.RemoveAt(pathSoFar.Count - 1);
        }
    }
}
