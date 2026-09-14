using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.DeleteDuplicateFoldersInSystem;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DeleteDuplicateFoldersInSystemSolution's, the same
// methods DeleteDuplicateFoldersInSystemTests proves correct - the definition-literal
// O(n^2) pairwise "is this subtree structurally identical to that one" brute force
// against a single post-order pass that canonically serializes each non-leaf subtree
// and groups repeats in a HashMap<string,int>.
//
// Neither arm takes a prepared tree: the tree carries the Deleted/Signature marks, so
// each strategy rebuilds it from the immutable paths on every call (ArrayPartition-
// Benchmarks' "clone per call" precedent, here applied to a mutable tree instead of a
// mutable array) and no mutation leaks into the next invocation. Only the paths array
// is hoisted into [GlobalSetup], and that is already LeetCode's own input shape.
//
// Both arms now return the surviving paths rather than a survivor count, because they
// are the methods the tests assert (ARCHITECTURE §17.8's precedent); the harness takes
// .Count so the result is still consumed.
[MemoryDiagnoser]
public class DeleteDuplicateFoldersInSystemBenchmarks
{
    [Params(100, 1_000)]
    public int TopLevelCount;

    private string[][] _paths = null!;

    [GlobalSetup]
    public void Setup() => _paths = FolderPathWorkloads.BuildIdenticalTopLevelFolders(TopLevelCount);

    [Benchmark(Baseline = true)]
    public int BruteForcePairwiseComparison() =>
        DeleteDuplicateFoldersInSystemSolution.DeleteDuplicateFoldersByPairwiseComparison(_paths).Count;

    [Benchmark]
    public int HashMapSignatureGrouping() =>
        DeleteDuplicateFoldersInSystemSolution.DeleteDuplicateFoldersBySignatureGrouping(_paths).Count;
}
