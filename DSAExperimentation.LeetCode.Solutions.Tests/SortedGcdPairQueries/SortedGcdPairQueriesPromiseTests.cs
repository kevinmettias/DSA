using DSAExperimentation.LeetCode.Solutions.SortedGcdPairQueries;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SortedGcdPairQueries;

public sealed partial class SortedGcdPairQueriesPromiseTests
{
    public static TheoryData<int[], int[], bool> Inputs =>
        new()
        {
            { [2, 3, 4], [0, 2, 2], true },
            { [5, 10, 15, 20], [0, 1, 2, 3, 4, 5, 6, 8], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryQueryNamesAPair(int[] nums, int[] queries, bool expected)
        => Assert.Equal(expected, SortedGcdPairQueriesPromise.IsKeptBy(nums, queries));
}
