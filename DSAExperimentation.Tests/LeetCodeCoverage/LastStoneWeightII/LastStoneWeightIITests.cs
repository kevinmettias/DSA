using DSAExperimentation.LeetCode.LastStoneWeightII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LastStoneWeightII;

// Harness only: both strategies live in LastStoneWeightIISolution, so the bottom-up
// tabulation that used to be the benchmark's unasserted baseline is now held to the
// same examples as the memoized recursion.
public sealed class LastStoneWeightIITests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 7, 4, 1, 8, 1], 1 },
            { [31, 26, 33, 21, 40], 5 },
            { [1], 1 },
            { [2, 2], 0 },
            { [1, 3], 2 },
            { [10, 10, 10, 10], 0 },
            { [1, 2, 4, 8, 16], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinWeightByTabulation_LeetCodeExamples_ReturnsMinimumPossibleWeight(int[] stones, int expected) =>
        Assert.Equal(expected, LastStoneWeightIISolution.MinWeightByTabulation(stones));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinWeightByMemoizer_LeetCodeExamples_ReturnsMinimumPossibleWeight(int[] stones, int expected) =>
        Assert.Equal(expected, LastStoneWeightIISolution.MinWeightByMemoizer(stones));
}
