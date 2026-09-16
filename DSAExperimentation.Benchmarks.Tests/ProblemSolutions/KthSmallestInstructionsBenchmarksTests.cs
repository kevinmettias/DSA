using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for KthSmallestInstructionsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one question - the rank-th lexicographically smallest route to the
// destination - so a harness whose arms disagree is timing two different problems. Both arms
// answer with the route string itself, and AnswerText renders a string with its own quoting, so
// the comparison is on the exact instruction sequence rather than on its length or character
// multiset. The destination and the median rank both come from the seeded fixture, so the same
// Size must rebuild the same question.
public sealed partial class KthSmallestInstructionsBenchmarksTests
{
    private const int SmallestSize = 5;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameDestinationAndRank() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().EnumerateAndSort()),
            AnswerText.Of(BuildHarness().EnumerateAndSort()));

    [Fact]
    public void EnumerateAndSort_SquareDestinationAtMedianRank_AgreesWithMemoizedGreedy()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.MemoizedGreedy()), AnswerText.Of(harness.EnumerateAndSort()));
    }

    [Fact]
    public void MemoizedGreedy_SquareDestinationAtMedianRank_AgreesWithEnumerateAndSort()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.EnumerateAndSort()), AnswerText.Of(harness.MemoizedGreedy()));
    }

    private static KthSmallestInstructionsBenchmarks BuildHarness()
    {
        var harness = new KthSmallestInstructionsBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
