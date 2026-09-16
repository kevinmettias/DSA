using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TwoSumIIInputArrayIsSortedBenchmarks (ARCHITECTURE 17.9): the class carries a
// single arm, so there is no second strategy to reconcile and the assertion has to be an oracle
// rather than an agreement.
//
// Setup's nums is 0..Length-1 and its target is 2 * Length - 3, which only nums[Length - 2] and
// nums[Length - 1] sum to - every earlier index's complement is negative, outside the array's value
// range - so the scan cannot resolve early and the one valid pair is the last two positions,
// reported 1-indexed the way LC 167 asks. That is the decisive value asserted below, and because
// the returned indices are derived from the array, asserting them also pins that the same Length
// rebuilt the same workload.
public sealed partial class TwoSumIIInputArrayIsSortedBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] lengths.
    private const int SmallestLength = 200;

    // LC 167 is 1-indexed: the last two entries are the only pair summing to 2 * Length - 3.
    private static readonly int[] ExpectedIndices = [SmallestLength - 1, SmallestLength];

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BinarySearch()),
            AnswerText.Of(BuildHarness().BinarySearch()));

    [Fact]
    public void BinarySearch_SmallestLength_FindsTheLastTwoPositions()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(ExpectedIndices), AnswerText.Of(harness.BinarySearch()));
    }

    private static TwoSumIIInputArrayIsSortedBenchmarks BuildHarness()
    {
        var harness = new TwoSumIIInputArrayIsSortedBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
