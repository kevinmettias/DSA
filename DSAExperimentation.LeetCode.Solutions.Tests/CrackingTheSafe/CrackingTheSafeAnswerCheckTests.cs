using DSAExperimentation.LeetCode.Solutions.CrackingTheSafe;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CrackingTheSafe;

public sealed partial class CrackingTheSafeAnswerCheckTests
{
    public static TheoryData<int, int, string, string, bool> Answers =>
        new()
        {
            { 1, 2, "01", "10", true },
            { 2, 2, "01100", "00110", true },
            { 2, 2, "00011", "00110", false },
            { 1, 2, "02", "10", false },
            { 1, 2, "011", "10", false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForAShortestStringHoldingEveryPassword(int passwordLength, int alphabetSize, string answer, string recorded, bool expected)
        => Assert.Equal(expected, CrackingTheSafeAnswerCheck.IsAccepted(passwordLength, alphabetSize, answer, recorded));
}
