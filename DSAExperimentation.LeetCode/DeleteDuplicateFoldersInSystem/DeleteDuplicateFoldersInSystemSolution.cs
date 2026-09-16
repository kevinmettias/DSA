using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.DeleteDuplicateFoldersInSystem;

// LeetCode 1948. Delete Duplicate Folders in System: mark every non-leaf folder that
// has a structurally identical twin anywhere in the tree, delete all of them at once
// (both copies, not "keep one"), and report the paths that survive.
//
// Both strategies build the same FolderNode tree and answer the same question - which
// non-leaf subtrees repeat - and differ only in how they find the repeats. The
// baseline compares every pair of non-leaf subtrees directly, O(n^2) structural
// walks. The composed strategy makes one post-order pass that canonically serializes
// each non-leaf subtree, sorting each node's "name(childSignature)" pieces with this
// repo's own MergeSort so insertion order can never affect the signature, and counts
// repeats in a HashMap<string,int>.
//
// A leaf is never given a signature and never compared, so two unrelated originally-
// empty folders are never mistaken for duplicates - that is what keeps a lone leaf
// like "/d/a" from being deleted just because other, unrelated leaves are also empty.
// The final pre-order walk emits the path of every surviving folder and never descends
// into a deleted one, which is what makes deletion cascade to every descendant for
// free.
//
// Neither strategy takes a prepared-input overload: the tree is mutable marking state,
// so it has to be rebuilt from the immutable paths on every call, and the paths array
// already IS LeetCode's own input shape.
internal static class DeleteDuplicateFoldersInSystemSolution
{
    private const string SignatureOpen = "(";
    private const string SignatureClose = ")";

    // The definition-literal brute force: collect every non-leaf folder and ask, for
    // each pair, whether the two subtrees are identical. Deliberately BCL throughout -
    // a List and two nested loops - so it stands for "what you would write without
    // this repo" (ARCHITECTURE §17.5); only the folder tree it walks is a repo shape,
    // because that is the input both arms are handed.
    public static List<string[]> DeleteDuplicateFoldersByPairwiseComparison(string[][] paths)
    {
        var root = BuildTree(paths);
        var nonLeafFolders = new List<FolderNode>();
        CollectNonLeafFolders(root, nonLeafFolders);

        for (var i = 0; i < nonLeafFolders.Count; i++)
        {
            for (var j = i + 1; j < nonLeafFolders.Count; j++)
            {
                if (IsIdenticalSubtree(nonLeafFolders[i], nonLeafFolders[j]))
                {
                    nonLeafFolders[i].Deleted = true;
                    nonLeafFolders[j].Deleted = true;
                }
            }
        }

        return CollectSurvivingPaths(root);
    }

    // One post-order serialization pass plus one counting map: every subtree that
    // repeats is found in a single walk instead of by pairwise comparison.
    public static List<string[]> DeleteDuplicateFoldersBySignatureGrouping(string[][] paths)
    {
        var root = BuildTree(paths);

        var signatureCounts = new HashMap<string, int>();
        Serialize(root, signatureCounts);
        MarkRepeatedSignatures(root, signatureCounts);

        return CollectSurvivingPaths(root);
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

    private static void CollectNonLeafFolders(FolderNode node, List<FolderNode> accumulator)
    {
        foreach (var child in node.Children.Values)
        {
            if (child.Children.Count > 0)
            {
                accumulator.Add(child);
            }

            CollectNonLeafFolders(child, accumulator);
        }
    }

    private static bool IsIdenticalSubtree(FolderNode left, FolderNode right)
    {
        if (left.Children.Count != right.Children.Count)
        {
            return false;
        }

        foreach (var name in left.Children.Keys)
        {
            if (!right.Children.TryGetValue(name, out var rightChild))
            {
                return false;
            }

            left.Children.TryGetValue(name, out var leftChild);

            if (!IsIdenticalSubtree(leftChild, rightChild))
            {
                return false;
            }
        }

        return true;
    }

    // Leaves return without ever touching signatureCounts, so a leaf's empty Signature
    // can never collide with another leaf's in the duplicate-count map.
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
            pieces[i++] = name + SignatureOpen + child.Signature + SignatureClose;
        }

        var sequence = new ArrayIndexedSequence<string>(pieces);
        MergeSort.Sort<string, ArrayIndexedSequence<string>>(sequence, StringComparer.Ordinal);

        node.Signature = string.Concat(pieces);
        var newCount = signatureCounts.TryGetValue(node.Signature, out var existing) ? NextSignatureCount(existing) : 1;
        signatureCounts.Set(node.Signature, newCount);
    }

    // The count to record for a signature that has now been seen one more time.
    private static int NextSignatureCount(int previousCount) => previousCount + 1;

    private static void MarkRepeatedSignatures(FolderNode node, HashMap<string, int> signatureCounts)
    {
        foreach (var name in node.Children.Keys)
        {
            node.Children.TryGetValue(name, out var child);

            if (IsRepeatedFolder(child, signatureCounts))
            {
                child.Deleted = true;
            }

            MarkRepeatedSignatures(child, signatureCounts);
        }
    }

    // A folder is deleted when a non-leaf subtree's own signature turns up more than
    // once anywhere in the tree - leaves carry no signature and so are never counted.
    private static bool IsRepeatedFolder(FolderNode child, HashMap<string, int> signatureCounts)
        => child.Children.Count > 0
            && signatureCounts.TryGetValue(child.Signature, out var count)
            && count > 1;

    private static List<string[]> CollectSurvivingPaths(FolderNode root)
    {
        var surviving = new List<string[]>();
        Collect(root, new List<string>(), surviving);

        return surviving;
    }

    private static void Collect(FolderNode node, List<string> pathSoFar, List<string[]> surviving)
    {
        foreach (var name in node.Children.Keys)
        {
            node.Children.TryGetValue(name, out var child);

            if (child.Deleted)
            {
                continue;
            }

            pathSoFar.Add(name);
            surviving.Add(pathSoFar.ToArray());
            Collect(child, pathSoFar, surviving);
            pathSoFar.RemoveAt(pathSoFar.Count - 1);
        }
    }
}
