using DSAExperimentation.LeetCode.Solutions.ValidArrangementOfPairs;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ValidArrangementOfPairs;

public sealed partial class ValidArrangementOfPairsPromiseTests
{
    public static TheoryData<int[][], bool> Inputs =>
        new()
        {
            { [[5, 1], [4, 5], [11, 9], [9, 4]], true },
            { [[1, 3], [3, 2], [2, 1]], true },
            { [[1, 2], [1, 3], [2, 1]], true },
            { [[1, 2], [2, 3], [3, 1], [4, 5], [5, 6], [6, 4]], false },
            { [[1, 2], [1, 3]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenThePairsMakeOnePath(int[][] pairs, bool expected)
        => Assert.Equal(expected, ValidArrangementOfPairsPromise.IsKeptBy(pairs));
}
