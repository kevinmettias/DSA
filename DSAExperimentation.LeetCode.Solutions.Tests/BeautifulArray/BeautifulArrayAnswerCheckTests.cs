using DSAExperimentation.LeetCode.Solutions.BeautifulArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.BeautifulArray;

public sealed partial class BeautifulArrayAnswerCheckTests
{
    public static TheoryData<int, int[], int[], bool> Answers =>
        new()
        {
            { 4, [2, 1, 4, 3], [2, 1, 4, 3], true },
            { 4, [1, 3, 2, 4], [2, 1, 4, 3], true },
            { 5, [1, 5, 3, 2, 4], [3, 1, 2, 5, 4], true },
            { 4, [1, 2, 3, 4], [2, 1, 4, 3], false },
            { 4, [2, 1, 4, 4], [2, 1, 4, 3], false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForAPermutationWithNoAverageBetweenAPair(int length, int[] answer, int[] recorded, bool expected)
        => Assert.Equal(expected, BeautifulArrayAnswerCheck.IsAccepted(length, answer, recorded));
}
