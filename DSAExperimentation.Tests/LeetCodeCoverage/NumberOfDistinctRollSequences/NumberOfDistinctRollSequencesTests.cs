using DSAExperimentation.LeetCode.NumberOfDistinctRollSequences;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfDistinctRollSequences;

// Harness only: both strategies live in NumberOfDistinctRollSequencesSolution and
// are asserted against the same examples. n = 1 and n = 2 pin the two boundaries
// where the "no roll yet" sentinel is still in the state, n = 4 is LeetCode's own
// published example, and the longer counts keep the two arms honest once the
// recurrence has revisited states.
public sealed class NumberOfDistinctRollSequencesTests
{
    public static TheoryData<int, long> Examples =>
        new()
        {
            { 1, 6L },
            { 2, 22L },
            { 3, 66L },
            { 4, 184L },
            { 5, 516L },
            { 10, 93_120L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DistinctSequencesByBruteForceRecursion_LeetCodeExamples_ReturnsSequenceCount(
        int n, long expected) =>
        Assert.Equal(expected, NumberOfDistinctRollSequencesSolution.DistinctSequencesByBruteForceRecursion(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DistinctSequencesByMemoizedRecursion_LeetCodeExamples_ReturnsSequenceCount(
        int n, long expected) =>
        Assert.Equal(expected, NumberOfDistinctRollSequencesSolution.DistinctSequencesByMemoizedRecursion(n));
}
