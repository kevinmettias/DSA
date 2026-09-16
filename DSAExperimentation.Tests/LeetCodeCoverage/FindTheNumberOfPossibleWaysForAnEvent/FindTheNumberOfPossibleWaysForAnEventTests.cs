using DSAExperimentation.LeetCode.FindTheNumberOfPossibleWaysForAnEvent;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheNumberOfPossibleWaysForAnEvent;

// Harness only. Both counting strategies are
// FindTheNumberOfPossibleWaysForAnEventSolution's - this file just pins them to
// LeetCode's published examples.
public sealed class FindTheNumberOfPossibleWaysForAnEventTests
{
    public static TheoryData<int, int, int, int> Examples =>
        new()
        {
            { 1, 2, 3, 6 },
            { 5, 2, 1, 32 },
            { 3, 3, 4, 684 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfWaysByBruteForceEnumeration_LeetCodeExamples_ReturnsWaysModuloLargePrime(
        int n, int x, int y, int expected)
    {
        var actual =
            FindTheNumberOfPossibleWaysForAnEventSolution.NumberOfWaysByBruteForceEnumeration(n, x, y);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfWaysByStagePartitionMemo_LeetCodeExamples_ReturnsWaysModuloLargePrime(
        int n, int x, int y, int expected)
    {
        var actual = FindTheNumberOfPossibleWaysForAnEventSolution.NumberOfWaysByStagePartitionMemo(n, x, y);

        Assert.Equal(expected, actual);
    }
}
