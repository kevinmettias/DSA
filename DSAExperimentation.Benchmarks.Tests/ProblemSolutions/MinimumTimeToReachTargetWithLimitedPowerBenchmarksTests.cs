using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumTimeToReachTargetWithLimitedPowerBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - expanding the (node, remainingPower)
// states on the fly against searching the PowerStateGraph the same states were hoisted into in
// [GlobalSetup] - so a harness whose arms disagree is timing two different problems. Both arms only
// read the prepared graph and the seeded edge/cost arrays, so one harness instance is safe to call
// twice in either order. Setup draws the workload from one fixed seed, so the same NodeCount must
// rebuild the same graph; otherwise two published numbers were never comparable in the first place.
public sealed partial class MinimumTimeToReachTargetWithLimitedPowerBenchmarksTests
{
    private const int SmallestNodeCount = 100;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BclPriorityQueue()),
            AnswerText.Of(BuildHarness().BclPriorityQueue()));

    [Fact]
    public void BclPriorityQueue_SeededPowerStateWorkload_AgreesWithReduceGraph()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.ReduceGraph()),
            AnswerText.Of(harness.BclPriorityQueue()));
    }

    [Fact]
    public void ReduceGraph_SeededPowerStateWorkload_AgreesWithBclPriorityQueue()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BclPriorityQueue()),
            AnswerText.Of(harness.ReduceGraph()));
    }

    private static MinimumTimeToReachTargetWithLimitedPowerBenchmarks BuildHarness()
    {
        var harness = new MinimumTimeToReachTargetWithLimitedPowerBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
