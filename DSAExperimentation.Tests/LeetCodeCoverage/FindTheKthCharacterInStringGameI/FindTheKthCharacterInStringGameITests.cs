using DSAExperimentation.LeetCode.FindTheKthCharacterInStringGameI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheKthCharacterInStringGameI;

// Harness only: both strategies live in
// FindTheKthCharacterInStringGameISolution. This file just pins them to
// #3304's published examples.
public sealed class FindTheKthCharacterInStringGameITests
{
    public static TheoryData<int, char> Examples =>
        new()
        {
            { 5, 'b' },
            { 10, 'c' },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthCharacterBySimulation_LeetCodeExamples_ReturnsKthCharacter(
        int kthPosition, char expected) =>
        Assert.Equal(
            expected,
            FindTheKthCharacterInStringGameISolution.KthCharacterBySimulation(kthPosition));

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthCharacterByBitCount_LeetCodeExamples_ReturnsKthCharacter(
        int kthPosition, char expected) =>
        Assert.Equal(
            expected,
            FindTheKthCharacterInStringGameISolution.KthCharacterByBitCount(kthPosition));
}
