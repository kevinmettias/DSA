using DSAExperimentation.LeetCode.PoorPigs;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PoorPigs;

// Harness only. Both strategies are PoorPigsSolution's - this file just pins them to
// LeetCode's published examples, including the zero-pigs case where a single bucket
// needs no testing at all.
public sealed class PoorPigsTests
{
    public static TheoryData<int, int, int, int> Examples =>
        new()
        {
            { 1000, 15, 60, 5 },
            { 4, 15, 15, 2 },
            { 4, 15, 30, 2 },
            { 1, 15, 15, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinPigsByLinearRecompute_ClassicExamples_ReturnsMinimumPigCount(
        int buckets, int minutesToDie, int minutesToTest, int expected)
    {
        var actual = PoorPigsSolution.MinPigsByLinearRecompute(buckets, minutesToDie, minutesToTest);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinPigsByBinarySearch_ClassicExamples_ReturnsMinimumPigCount(
        int buckets, int minutesToDie, int minutesToTest, int expected)
    {
        var actual = PoorPigsSolution.MinPigsByBinarySearch(buckets, minutesToDie, minutesToTest);

        Assert.Equal(expected, actual);
    }
}
