using DSAExperimentation.LeetCode.Solutions.CreateGridWithExactlyKPathsI;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CreateGridWithExactlyKPathsI;

public sealed partial class CreateGridWithExactlyKPathsIAnswerCheckTests
{
    public static TheoryData<int, int, int, string[], string[], bool> Answers =>
        new()
        {
            { 2, 3, 2, ["...", "#.."], ["...", "#.."], true },
            { 2, 3, 2, ["..#", "..."], ["...", "#.."], true },
            { 1, 4, 2, [], [], true },
            { 2, 3, 2, ["...", "..."], ["...", "#.."], false },
            { 1, 4, 2, ["...."], [], false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForAGridWithExactlyKPaths(int gridRowCount, int gridColumnCount, int targetPathCount, string[] answer, string[] recorded, bool expected)
        => Assert.Equal(expected, CreateGridWithExactlyKPathsIAnswerCheck.IsAccepted(gridRowCount, gridColumnCount, targetPathCount, answer, recorded));
}
