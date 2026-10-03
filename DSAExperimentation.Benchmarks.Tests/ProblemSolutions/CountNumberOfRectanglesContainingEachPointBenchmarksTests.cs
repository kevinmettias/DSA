using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountNumberOfRectanglesContainingEachPointBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - checking every point against every
// rectangle against grouping lengths by height and bisecting - so a harness whose arms disagree is
// timing two different problems. Setup seeds both arrays, so the same RectangleCount must rebuild
// the same rectangles and points. AnswerGraphText.Of, not OfUnordered: the answer is one count per
// point, so the position of each count in the array is part of it.
public sealed partial class CountNumberOfRectanglesContainingEachPointBenchmarksTests
{
    private const int SmallestRectangleCount = 500;

    [Fact]
    public void Setup_SmallestRectangleCount_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: Setup draws as many points as rectangles, and the answer is one
        // count per point - each no larger than the number of rectangles that could cover it.
        Assert.Equal(SmallestRectangleCount, first.BruteForce().Length);
        Assert.All(first.BruteForce(), count => Assert.InRange(count, 0, SmallestRectangleCount));
        Assert.Equal(AnswerGraphText.Of(first.BruteForce()), AnswerGraphText.Of(second.BruteForce()));
    }

    [Fact]
    public void BruteForce_SmallestRectangleCount_AgreesWithGroupedSortedBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerGraphText.Of(harness.GroupedSortedBinarySearch()),
            AnswerGraphText.Of(harness.BruteForce()));
    }

    [Fact]
    public void GroupedSortedBinarySearch_SmallestRectangleCount_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerGraphText.Of(harness.BruteForce()),
            AnswerGraphText.Of(harness.GroupedSortedBinarySearch()));
    }

    private static CountNumberOfRectanglesContainingEachPointBenchmarks BuildHarness()
    {
        var harness = new CountNumberOfRectanglesContainingEachPointBenchmarks { RectangleCount = SmallestRectangleCount };
        harness.Setup();

        return harness;
    }
}
