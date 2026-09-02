using DSAExperimentation.LeetCode.FindTheNumberOfWaysToPlacePeopleII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheNumberOfWaysToPlacePeopleII;

// Harness only. Both strategies live in
// FindTheNumberOfWaysToPlacePeopleIISolution - this file just pins them to
// LeetCode's published examples (the same three LC 3025 publishes, restated
// at LC 3027's own n <= 1000 bound).
public sealed class FindTheNumberOfWaysToPlacePeopleIITests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[1, 1], [2, 2], [3, 3]], 0 },
            { [[6, 2], [4, 4], [2, 6]], 2 },
            { [[3, 1], [1, 3], [1, 1]], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByBruteForce_LeetCodeExamples_ReturnsVisiblePairCount(int[][] points, int expected) =>
        Assert.Equal(expected, FindTheNumberOfWaysToPlacePeopleIISolution.CountPairsByBruteForce(points));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsBySortedSweep_LeetCodeExamples_ReturnsVisiblePairCount(int[][] points, int expected) =>
        Assert.Equal(expected, FindTheNumberOfWaysToPlacePeopleIISolution.CountPairsBySortedSweep(points));
}
