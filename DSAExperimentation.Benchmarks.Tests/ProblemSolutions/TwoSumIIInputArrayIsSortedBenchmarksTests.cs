using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TwoSumIIInputArrayIsSortedBenchmarks (ARCHITECTURE 17.9): both arms are
// competing strategies for the same question - a per-index binary search and a two-pointer squeeze
// - so a harness whose arms disagree is finding two different pairs.
//
// Setup's nums and target come from TwoSumIIInputArrayIsSortedWorkloads, whose target only
// nums[Length - 2] and nums[Length - 1] sum to - every earlier index's complement is past the
// array's largest value - so the binary-search arm cannot resolve early and the one valid pair is
// the last two positions, reported 1-indexed the way LC 167 asks. That is the decisive value asserted below, and
// because the returned indices are derived from the array, asserting them also pins that the same
// Length rebuilt the same workload.
public sealed partial class TwoSumIIInputArrayIsSortedBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] lengths.
    private const int SmallestLength = 200;

    // LC 167 is 1-indexed: the last two entries are the only pair summing to the target.
    private static readonly int[] ExpectedIndices = [SmallestLength - 1, SmallestLength];

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerGraphText.Of(BuildHarness().BinarySearch()),
            AnswerGraphText.Of(BuildHarness().BinarySearch()));

    [Fact]
    public void BinarySearch_SmallestLength_FindsTheLastTwoPositions()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerGraphText.Of(ExpectedIndices), AnswerGraphText.Of(harness.BinarySearch()));
    }

    [Fact]
    public void TwoPointerSqueeze_SmallestLength_FindsTheLastTwoPositions()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerGraphText.Of(ExpectedIndices), AnswerGraphText.Of(harness.TwoPointerSqueeze()));
    }

    [Fact]
    public void TwoPointerSqueeze_AgreesWithBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerGraphText.Of(harness.BinarySearch()), AnswerGraphText.Of(harness.TwoPointerSqueeze()));
    }

    private static TwoSumIIInputArrayIsSortedBenchmarks BuildHarness()
    {
        var harness = new TwoSumIIInputArrayIsSortedBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
