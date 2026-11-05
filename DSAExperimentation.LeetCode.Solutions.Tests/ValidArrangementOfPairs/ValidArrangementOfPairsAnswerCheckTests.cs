using DSAExperimentation.LeetCode.Solutions.ValidArrangementOfPairs;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ValidArrangementOfPairs;

public sealed partial class ValidArrangementOfPairsAnswerCheckTests
{
    public static TheoryData<int[][], int[][], int[][], bool> Answers =>
        new()
        {
            { [[5, 1], [4, 5], [11, 9], [9, 4]], [[11, 9], [9, 4], [4, 5], [5, 1]], [[11, 9], [9, 4], [4, 5], [5, 1]], true },
            { [[1, 3], [3, 2], [2, 1]], [[3, 2], [2, 1], [1, 3]], [[1, 3], [3, 2], [2, 1]], true },
            { [[1, 2], [2, 3]], [[2, 3], [1, 2]], [[1, 2], [2, 3]], false },
            { [[1, 2], [2, 3]], [[1, 2], [2, 4]], [[1, 2], [2, 3]], false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForThePairsChainedEndToStart(int[][] pairs, int[][] answer, int[][] recorded, bool expected)
        => Assert.Equal(expected, ValidArrangementOfPairsAnswerCheck.IsAccepted(pairs, answer, recorded));
}
