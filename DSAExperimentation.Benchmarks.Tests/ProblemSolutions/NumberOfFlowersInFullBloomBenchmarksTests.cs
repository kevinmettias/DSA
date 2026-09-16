using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfFlowersInFullBloomBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a per-person scan of every flower interval against the
// sorted-bounds binary search - so a harness whose arms disagree is answering two different arrival
// queries. Setup builds the seeded flower intervals and arrival times in LeetCode's own input shape,
// so the same Count must rebuild the same intervals and the same persons in the same order.
//
// Both arms return one count per person, and the problem fixes that outer order as the persons' own
// order, so the per-position agreement is pinned by rendering each returned array in sequence rather
// than by reference equality. Neither arm reorders the arrays it is handed (the sorted bounds are its
// own copy), so one harness serves both arms in either order.
public sealed partial class NumberOfFlowersInFullBloomBenchmarksTests
{
    private const int SmallestCount = 200;

    [Fact]
    public void Setup_SameCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_AgreesWithSortThenBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.SortThenBinarySearch()),
            AnswerText.Of(harness.BruteForce()));
    }

    [Fact]
    public void SortThenBinarySearch_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BruteForce()),
            AnswerText.Of(harness.SortThenBinarySearch()));
    }

    private static NumberOfFlowersInFullBloomBenchmarks BuildHarness()
    {
        var harness = new NumberOfFlowersInFullBloomBenchmarks { Count = SmallestCount };
        harness.Setup();

        return harness;
    }
}
