using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AllPathsFromSourceToTargetBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot
// pin: how many paths Setup's layered-complete graph holds, known from its construction rather than from either
// arm. Every path from node 0 to node n-1 picks an arbitrary subset of the n-2 intermediate nodes, so the answer
// holds exactly 2^(n-2) paths.
public sealed partial class AllPathsFromSourceToTargetBenchmarksTests
{
    // The smaller of Setup's [Params(10, 15)] node counts.
    private const int SmallestNodeCount = 10;

    // 2^(SmallestNodeCount - 2): every subset of the 8 intermediate nodes is one path.
    private const int ExpectedPathCount = 256;

    [Fact]
    public void SpecializedRecursive_TenNodeLayeredGraph_FindsEverySubsetPath() =>
        Assert.Equal(ExpectedPathCount, BuildHarness().SpecializedRecursive().Count);

    [Fact]
    public void Backtracking_TenNodeLayeredGraph_FindsEverySubsetPath() =>
        Assert.Equal(ExpectedPathCount, BuildHarness().Backtracking().Count);

    private static AllPathsFromSourceToTargetBenchmarks BuildHarness()
    {
        var harness = new AllPathsFromSourceToTargetBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
