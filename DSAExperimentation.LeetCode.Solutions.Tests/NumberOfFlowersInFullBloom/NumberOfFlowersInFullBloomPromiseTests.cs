using DSAExperimentation.LeetCode.Solutions.NumberOfFlowersInFullBloom;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfFlowersInFullBloom;

public sealed partial class NumberOfFlowersInFullBloomPromiseTests
{
    public static TheoryData<int[][], int[], bool> Inputs =>
        new()
        {
            { [[1, 6], [3, 7], [9, 12], [4, 13]], [2, 3, 7, 11], true },
            { [[6, 5]], [5], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryBloomStartsByItsEnd(int[][] flowers, int[] persons, bool expected)
        => Assert.Equal(expected, NumberOfFlowersInFullBloomPromise.IsKeptBy(flowers, persons));
}
