using DSAExperimentation.LeetCode.Solutions.BuildAMatrixWithConditions;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BuildAMatrixWithConditions;

public sealed partial class BuildAMatrixWithConditionsAnswerCheckTests
{
    public static TheoryData<int, int[][], int[][], int[][], int[][], bool> Answers =>
        new()
        {
            { 3, [[1, 2], [3, 2]], [[2, 1], [3, 2]], [[3, 0, 0], [0, 0, 1], [0, 2, 0]], [[3, 0, 0], [0, 0, 1], [0, 2, 0]], true },
            { 3, [[1, 2], [3, 2]], [[2, 1], [3, 2]], [[0, 0, 1], [3, 0, 0], [0, 2, 0]], [[3, 0, 0], [0, 0, 1], [0, 2, 0]], true },
            { 3, [[1, 2], [2, 3], [3, 1], [2, 3]], [[2, 1]], [], [], true },
            { 3, [[1, 2], [3, 2]], [[2, 1], [3, 2]], [[1, 0, 0], [0, 2, 0], [0, 0, 3]], [[3, 0, 0], [0, 0, 1], [0, 2, 0]], false },
            { 3, [[1, 2], [3, 2]], [[2, 1], [3, 2]], [], [[3, 0, 0], [0, 0, 1], [0, 2, 0]], false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForAMatrixMeetingEveryCondition(int valueCount, int[][] rowConditions, int[][] colConditions, int[][] answer, int[][] recorded, bool expected)
        => Assert.Equal(expected, BuildAMatrixWithConditionsAnswerCheck.IsAccepted(valueCount, rowConditions, colConditions, answer, recorded));
}
