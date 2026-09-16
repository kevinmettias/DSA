using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ErectTheFenceBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - testing every ordered pair of points as a candidate hull line
// against Andrew's monotone chain over this repo's own Stack - so a harness whose arms disagree is
// timing two different problems. AnswerText.OfUnorderedSet, not Of: LC 587's answer is the set of
// trees the fence passes through, and both arms return it out of a HashSet, so the order the points
// come back in is genuinely unfixed by the problem and by both implementations - only membership is
// promised. Setup draws Length points from one fixed seed inside a bounded grid, and a fence around
// a non-degenerate point set holds at least the three corners any triangle of them forces.
public sealed partial class ErectTheFenceBenchmarksTests
{
    private const int SmallestLength = 50;
    private const int MinimumFencePointCountForAGridPointSet = 3;

    [Fact]
    public void Setup_FiftyGridPoints_FencesTheHullBoundaryAndRebuildsTheSameWorkload()
    {
        var harness = BuildHarness();
        var fence = harness.EveryPairHalfPlaneScan();

        Assert.InRange(fence.Count, MinimumFencePointCountForAGridPointSet, SmallestLength);
        Assert.Equal(
            AnswerText.OfUnorderedSet(fence),
            AnswerText.OfUnorderedSet(BuildHarness().EveryPairHalfPlaneScan()));
    }

    [Fact]
    public void EveryPairHalfPlaneScan_FiftyGridPoints_AgreesWithMonotoneChainThenEdgeScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.OfUnorderedSet(harness.MonotoneChainThenEdgeScan()),
            AnswerText.OfUnorderedSet(harness.EveryPairHalfPlaneScan()));
    }

    [Fact]
    public void MonotoneChainThenEdgeScan_FiftyGridPoints_AgreesWithEveryPairHalfPlaneScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.OfUnorderedSet(harness.EveryPairHalfPlaneScan()),
            AnswerText.OfUnorderedSet(harness.MonotoneChainThenEdgeScan()));
    }

    private static ErectTheFenceBenchmarks BuildHarness()
    {
        var harness = new ErectTheFenceBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
