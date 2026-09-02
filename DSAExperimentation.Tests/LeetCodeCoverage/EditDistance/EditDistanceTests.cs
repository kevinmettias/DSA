using DSAExperimentation.LeetCode.EditDistance;

namespace DSAExperimentation.Tests.LeetCodeCoverage.EditDistance;

// Harness only. EditDistanceSolution owns both the tabulated baseline and the
// memoized recurrence; this file pins them to LeetCode's published examples.
public sealed class EditDistanceTests
{
    public static TheoryData<string, string, int> Examples =>
        new()
        {
            { "horse", "ros", 3 },
            { "intention", "execution", 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinDistanceByTabulation_LeetCodeExamples_ReturnsEditDistance(
        string word1, string word2, int expected) =>
        Assert.Equal(expected, EditDistanceSolution.MinDistanceByTabulation(word1, word2));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinDistanceByMemoizedRecurrence_LeetCodeExamples_ReturnsEditDistance(
        string word1, string word2, int expected) =>
        Assert.Equal(expected, EditDistanceSolution.MinDistanceByMemoizedRecurrence(word1, word2));
}
