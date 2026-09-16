using DSAExperimentation.LeetCode.LongestWellPerformingInterval;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestWellPerformingInterval;

// Harness only: both strategies are LongestWellPerformingIntervalSolution's - the
// O(n^2) baseline and the HashMap first-occurrence-of-a-running-score pass - pinned
// to LeetCode's published examples plus the cases that separate the two branches of
// the linear strategy: an interval that starts at index 0 because the running score
// is already positive, and one found only by matching a score seen earlier.
public sealed partial class LongestWellPerformingIntervalTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [9, 9, 6, 0, 6, 6, 9], 3 },
            { [6, 6, 6], 0 },
            { [9, 9, 9], 3 },
            { [9], 1 },
            { [6], 0 },
            { [6, 9, 9], 3 },
            { [6, 6, 9, 9, 6], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestWpiByBruteForce_LeetCodeExamples_ReturnsLongestPositiveScoreInterval(
        int[] hours, int expected) =>
        Assert.Equal(expected, LongestWellPerformingIntervalSolution.LongestWpiByBruteForce(hours));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestWpiByPrefixScoreMap_LeetCodeExamples_ReturnsLongestPositiveScoreInterval(
        int[] hours, int expected) =>
        Assert.Equal(expected, LongestWellPerformingIntervalSolution.LongestWpiByPrefixScoreMap(hours));
}
