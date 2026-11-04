using DSAExperimentation.LeetCode.Solutions.QueryKthSmallestTrimmedNumber;

namespace DSAExperimentation.LeetCode.Solutions.Tests.QueryKthSmallestTrimmedNumber;

public sealed partial class QueryKthSmallestTrimmedNumberPromiseTests
{
    public static TheoryData<string[], int[][], bool> Inputs =>
        new()
        {
            { ["102", "473", "251", "814"], [[1, 1], [2, 3], [4, 2], [1, 2]], true },
            { ["111", "222", "333", "444"], [[4, 4]], false },
            { ["1", "22"], [[1, 1]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryQueryFitsTheNumbers(string[] nums, int[][] queries, bool expected)
        => Assert.Equal(expected, QueryKthSmallestTrimmedNumberPromise.IsKeptBy(nums, queries));
}
