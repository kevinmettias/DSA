using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FallingSquaresBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - an O(n^2) overlap scan against coordinate compression plus
// this repo's LazySegmentTree - so a harness whose arms disagree is dropping two different squares.
// Both arms return one height per square, in the order the squares fell, and that order is the
// answer: LeetCode asks for the running stack height after each drop, so the two lists are compared
// as ordered sequences. The footprints come from a fixed seed, so the same SquareCount must rebuild
// the same squares.
public sealed partial class FallingSquaresBenchmarksTests
{
    private const int SmallestSquareCount = 100;

    [Fact]
    public void Setup_SameSquareCount_RebuildsTheSameFootprints()
    {
        Assert.Equal(SmallestSquareCount, BuildHarness().BruteForceOverlapScan().Count);

        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceOverlapScan()),
            AnswerText.Of(BuildHarness().BruteForceOverlapScan()));
    }

    [Fact]
    public void BruteForceOverlapScan_OverlappingSquares_AgreesWithLazySegmentTreeRangeMax()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.LazySegmentTreeRangeMax()),
            AnswerText.Of(harness.BruteForceOverlapScan()));
    }

    [Fact]
    public void LazySegmentTreeRangeMax_OverlappingSquares_AgreesWithBruteForceOverlapScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForceOverlapScan()),
            AnswerText.Of(harness.LazySegmentTreeRangeMax()));
    }

    private static FallingSquaresBenchmarks BuildHarness()
    {
        var harness = new FallingSquaresBenchmarks { SquareCount = SmallestSquareCount };
        harness.Setup();

        return harness;
    }
}
