using DSAExperimentation.LeetCode.Solutions.LastStoneWeightII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LastStoneWeightII;

// Harness only: both strategies live in LastStoneWeightIISolution, so the bottom-up
// tabulation that used to be the benchmark's unasserted baseline is now held to the
// same cases as the memoized recursion: two equal stones that cancel, two unequal
// stones, and the powers of two from 1 to 16, whose best split leaves 1. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class LastStoneWeightIISolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [2, 2], 0 },
            { [1, 3], 2 },
            { [1, 2, 4, 8, 16], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinWeightByTabulation_Cases_ReturnsMinimumPossibleWeight(int[] stones, int expected) =>
        Assert.Equal(expected, LastStoneWeightIISolution.MinWeightByTabulation(stones));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinWeightByMemoizer_Cases_ReturnsMinimumPossibleWeight(int[] stones, int expected) =>
        Assert.Equal(expected, LastStoneWeightIISolution.MinWeightByMemoizer(stones));
}
