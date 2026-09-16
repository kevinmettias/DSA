using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumReverseOperationsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the fewest fixed-size reversals that land the single 1
// on every other position - so a harness whose arms disagree is timing two different problems. Both
// arms answer per position, in position order, so the comparison is order-sensitive. ReduceGraph is
// handed the ReversalBoard [GlobalSetup] already prepared, so the comparison also pins that the
// hoisted board describes the same (nodeCount, windowSize, banned) the scan arm is given; Setup has
// no banned positions, so both arms search the whole board.
public sealed partial class MinimumReverseOperationsBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceScan()),
            AnswerText.Of(BuildHarness().BruteForceScan()));

    [Fact]
    public void BruteForceScan_SameBannedBoard_AgreesWithReduceGraph()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.ReduceGraph()),
            AnswerText.Of(harness.BruteForceScan()));
    }

    [Fact]
    public void ReduceGraph_SameBannedBoard_AgreesWithBruteForceScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForceScan()),
            AnswerText.Of(harness.ReduceGraph()));
    }

    private static MinimumReverseOperationsBenchmarks BuildHarness()
    {
        var harness = new MinimumReverseOperationsBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
