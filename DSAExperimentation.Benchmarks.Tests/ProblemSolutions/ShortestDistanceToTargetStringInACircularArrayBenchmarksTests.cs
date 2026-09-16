using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ShortestDistanceToTargetStringInACircularArrayBenchmarks (ARCHITECTURE 17.9):
// both arms answer the same circular-distance question about the same word circle, so a harness whose
// arms disagree is timing two different circles. Setup builds that circle through the shared
// CircularArrayWorkloads fixture and the graph the BFS arm walks from the same words, so the same
// Length must rebuild both. The fixture places the single target word diametrically opposite
// StartIndex, which pins the answer independently of either strategy: the shorter way round the
// circle is half its length, so the Setup test asserts that derived distance rather than only the
// rebuild.
public sealed partial class ShortestDistanceToTargetStringInACircularArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    // The fixture's target sits diametrically opposite StartIndex, so half the circle is the
    // shortest way round.
    private const int DiametricDivisor = 2;
    private const int ExpectedDistance = SmallestLength / DiametricDivisor;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());
        Assert.Equal(ExpectedDistance, BuildHarness().LinearScan());
    }

    [Fact]
    public void LinearScan_DiametricTarget_AgreesWithReduceGraphBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReduceGraphBfs(), harness.LinearScan());
    }

    [Fact]
    public void ReduceGraphBfs_DiametricTarget_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.ReduceGraphBfs());
    }

    private static ShortestDistanceToTargetStringInACircularArrayBenchmarks BuildHarness()
    {
        var harness = new ShortestDistanceToTargetStringInACircularArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
