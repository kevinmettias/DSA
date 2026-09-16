using DSAExperimentation.LeetCode.FindTheKthCharacterInStringGameII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheKthCharacterInStringGameII;

// Harness only: both strategies live in
// FindTheKthCharacterInStringGameIISolution. This file just pins them to
// #3307's published examples.
public sealed partial class FindTheKthCharacterInStringGameIITests
{
    public static TheoryData<long, int[], char> Examples =>
        new()
        {
            { 5, [0, 0, 0], 'a' },
            { 10, [0, 1, 0, 1], 'b' },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthCharacterByBruteForceSimulation_LeetCodeExamples_ReturnsKthCharacter(
        long targetPosition, int[] operations, char expected)
    {
        var actual = FindTheKthCharacterInStringGameIISolution.KthCharacterByBruteForceSimulation(
            targetPosition, operations);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void KthCharacterByBackwardTrace_LeetCodeExamples_ReturnsKthCharacter(
        long targetPosition, int[] operations, char expected)
    {
        var actual = FindTheKthCharacterInStringGameIISolution.KthCharacterByBackwardTrace(
            targetPosition, operations);

        Assert.Equal(expected, actual);
    }
}
