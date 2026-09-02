using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Delete Duplicate Folders in System (LC 1948): the definition-literal O(n^2)
// pairwise "is this subtree structurally identical to that one" brute force
// (RemoveSubFoldersFromTheFilesystemBenchmarks precedent for an O(n^2) pairwise
// baseline over folder paths) vs. a single O(n) post-order pass that canonically
// serializes each non-leaf subtree - via this repo's own MergeSort, so insertion
// order never affects the signature (RemoveSubFoldersFromTheFilesystemBenchmarks
// precedent again) - and groups repeats with a HashMap<string,int>. The tree is
// rebuilt fresh from the immutable _paths on every call (ArrayPartitionBenchmarks'
// "clone per call" precedent, here applied to a mutable tree instead of a mutable
// array) so neither benchmark's Deleted/Signature mutations leak into the next
// invocation. TopLevelCount top-level folders all share the identical two-level
// shape x -> {y} and z, so both strategies must actually find TopLevelCount-choose-2
// worth of duplicate pairs, not skip the comparison via an early structural
// mismatch.
[MemoryDiagnoser]
public class DeleteDuplicateFoldersInSystemBenchmarks
{
    private const int PathsPerTopLevelFolder = 2;
    private const string TopLevelPrefix = "t";
    private const string NestedFolderX = "x";
    private const string NestedFolderY = "y";
    private const string NestedFolderZ = "z";
    private const string SignatureOpenParen = "(";
    private const string SignatureCloseParen = ")";

    [Params(100, 1_000)]
    public int TopLevelCount;

    private string[][] _paths = null!;

    [GlobalSetup]
    public void Setup()
    {
        _paths = new string[TopLevelCount * PathsPerTopLevelFolder][];

        for (var i = 0; i < TopLevelCount; i++)
        {
            var top = TopLevelPrefix + i;
            _paths[i * PathsPerTopLevelFolder] = [top, NestedFolderX, NestedFolderY];
            _paths[(i * PathsPerTopLevelFolder) + 1] = [top, NestedFolderZ];
        }
    }

    [Benchmark(Baseline = true)]
    public int BruteForcePairwiseComparison()
    {
        var root = BuildTree(_paths);
        var nonLeafNodes = new List<FolderNode>();
        CollectNonLeafNodes(root, nonLeafNodes);

        for (var i = 0; i < nonLeafNodes.Count; i++)
        {
            for (var j = i + 1; j < nonLeafNodes.Count; j++)
            {
                if (AreIdenticalSubtrees(nonLeafNodes[i], nonLeafNodes[j]))
                {
                    nonLeafNodes[i].Deleted = true;
                    nonLeafNodes[j].Deleted = true;
                }
            }
        }

        return CountSurviving(root);
    }

    [Benchmark]
    public int HashMapSignatureGrouping()
    {
        var root = BuildTree(_paths);

        var signatureCounts = new HashMap<string, int>();
        Serialize(root, signatureCounts);
        MarkDuplicates(root, signatureCounts);

        return CountSurviving(root);
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

    private static void CollectNonLeafNodes(FolderNode node, List<FolderNode> accumulator)
    {
        foreach (var child in node.Children.Values)
        {
            if (child.Children.Count > 0)
            {
                accumulator.Add(child);
            }

            CollectNonLeafNodes(child, accumulator);
        }
    }

    private static bool AreIdenticalSubtrees(FolderNode a, FolderNode b)
    {
        if (a.Children.Count != b.Children.Count)
        {
            return false;
        }

        foreach (var name in a.Children.Keys)
        {
            if (!b.Children.TryGetValue(name, out var bChild))
            {
                return false;
            }

            a.Children.TryGetValue(name, out var aChild);

            if (!AreIdenticalSubtrees(aChild, bChild))
            {
                return false;
            }
        }

        return true;
    }

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
            pieces[i++] = name + SignatureOpenParen + child.Signature + SignatureCloseParen;
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

    private static int CountSurviving(FolderNode node)
    {
        var count = 0;

        foreach (var child in node.Children.Values)
        {
            if (child.Deleted)
            {
                continue;
            }

            count++;
            count += CountSurviving(child);
        }

        return count;
    }
}
