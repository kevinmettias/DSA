using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AllPathsFromSourceToTargetBenchmarks (ARCHITECTURE 17.9): its two arms are
// AllPathsFromSourceToTargetSolution's competing strategies for the same question - a specialized recursive walk
// against generic backtracking - so a harness whose arms disagree is enumerating two different graphs. Both arms
// report only the path count, which on Setup's layered-complete graph is decisive rather than a proxy: every path
// from node 0 to node n-1 picks an arbitrary subset of the n-2 intermediate nodes, so the count is exactly
// 2^(n-2) and the harness asserts that literal alongside the arms' agreement.
public sealed partial class AllPathsFromSourceToTargetBenchmarksTests
{
    // The smaller of Setup's [Params(10, 15)] node counts.
    private const int SmallestNodeCount = 10;

    // 2^(SmallestNodeCount - 2): every subset of the 8 intermediate nodes is one path.
    private const int ExpectedPathCount = 256;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().SpecializedRecursive(), BuildHarness().SpecializedRecursive());

    [Fact]
    public void SpecializedRecursive_TenNodeLayeredGraph_AgreesWithBacktracking()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedPathCount, harness.SpecializedRecursive());
        Assert.Equal(harness.Backtracking(), harness.SpecializedRecursive());
    }

    [Fact]
    public void Backtracking_TenNodeLayeredGraph_AgreesWithSpecializedRecursive()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedPathCount, harness.Backtracking());
        Assert.Equal(harness.SpecializedRecursive(), harness.Backtracking());
    }

    private static AllPathsFromSourceToTargetBenchmarks BuildHarness()
    {
        var harness = new AllPathsFromSourceToTargetBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
