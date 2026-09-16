using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for XORAfterRangeMultiplicationQueriesIBenchmarks (ARCHITECTURE 17.9): both arms
// are competing application strategies for one array and one query batch, so a harness whose arms
// disagree is timing two different problems. Both arms take LeetCode's own array shape, so a single
// call per arm leaves the workload untouched and one harness instance is safe to call twice in
// either order.
public sealed partial class XORAfterRangeMultiplicationQueriesIBenchmarksTests
{
    private const int SmallestNumberCount = 100;

    [Fact]
    public void Setup_SameNumberCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().RangeScan()),
            AnswerText.Of(BuildHarness().RangeScan()));

    [Fact]
    public void RangeScan_AgreesWithStridedWalk()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RangeScan(), harness.StridedWalk());
    }

    [Fact]
    public void StridedWalk_AgreesWithRangeScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StridedWalk(), harness.RangeScan());
    }

    private static XORAfterRangeMultiplicationQueriesIBenchmarks BuildHarness()
    {
        var harness = new XORAfterRangeMultiplicationQueriesIBenchmarks { NumberCount = SmallestNumberCount };
        harness.Setup();

        return harness;
    }
}
