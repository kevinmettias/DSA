using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for XORAfterRangeMultiplicationQueriesIIBenchmarks (ARCHITECTURE 17.9): both arms
// are competing application strategies for one array and one query batch, so a harness whose arms
// disagree is timing two different problems. Both arms take LeetCode's own array shape, so a single
// call per arm leaves the workload untouched and one harness instance is safe to call twice in
// either order.
public sealed partial class XORAfterRangeMultiplicationQueriesIIBenchmarksTests
{
    private const int SmallestNodeCount = 1_000;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().StridedWalk()),
            AnswerText.Of(BuildHarness().StridedWalk()));

    [Fact]
    public void StridedWalk_AgreesWithSqrtDecomposition()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StridedWalk(), harness.SqrtDecomposition());
    }

    [Fact]
    public void SqrtDecomposition_AgreesWithStridedWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SqrtDecomposition(), harness.StridedWalk());
    }

    private static XORAfterRangeMultiplicationQueriesIIBenchmarks BuildHarness()
    {
        var harness = new XORAfterRangeMultiplicationQueriesIIBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
