using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountNumberOfRectanglesContainingEachPointBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a fresh harness, so this asserts what it
// cannot: the answer's length and per-point bound, which come from the workload's shape rather than
// from either arm, and that a harness which has already answered once answers as a rebuild does,
// since BenchmarkDotNet calls an arm many times on one harness while the generic check builds a
// fresh harness for every call. Setup seeds both arrays, so the same RectangleCount must rebuild the
// same rectangles and points. AnswerGraphText.Of, not OfUnordered: the answer is one count per
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

    private static CountNumberOfRectanglesContainingEachPointBenchmarks BuildHarness()
    {
        var harness = new CountNumberOfRectanglesContainingEachPointBenchmarks { RectangleCount = SmallestRectangleCount };
        harness.Setup();

        return harness;
    }
}
