using DSAExperimentation.LeetCode.FindTheNumberOfWaysToPlacePeopleI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheNumberOfWaysToPlacePeopleI;

// Harness only. Both strategies live in
// FindTheNumberOfWaysToPlacePeopleISolution - this file just pins them to
// LeetCode's published examples.
public sealed partial class FindTheNumberOfWaysToPlacePeopleITests
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
        Assert.Equal(expected, FindTheNumberOfWaysToPlacePeopleISolution.CountPairsByBruteForce(points));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsBySortedSweep_LeetCodeExamples_ReturnsVisiblePairCount(int[][] points, int expected) =>
        Assert.Equal(expected, FindTheNumberOfWaysToPlacePeopleISolution.CountPairsBySortedSweep(points));
}
