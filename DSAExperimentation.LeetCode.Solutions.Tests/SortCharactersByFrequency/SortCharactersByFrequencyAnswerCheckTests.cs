using DSAExperimentation.LeetCode.Solutions.SortCharactersByFrequency;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SortCharactersByFrequency;

public sealed partial class SortCharactersByFrequencyAnswerCheckTests
{
    public static TheoryData<string, string, string, bool> Answers =>
        new()
        {
            { "tree", "eert", "eert", true },
            { "tree", "eetr", "eert", true },
            { "Aabb", "bbaA", "bbAa", true },
            { "cccaaa", "aaaccc", "cccaaa", true },
            { "tree", "tree", "eert", false },
            { "tree", "eetx", "eert", false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForTheTextsCharactersInRunsLongestFirst(string text, string answer, string recorded, bool expected)
        => Assert.Equal(expected, SortCharactersByFrequencyAnswerCheck.IsAccepted(text, answer, recorded));
}
