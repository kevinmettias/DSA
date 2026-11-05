using DSAExperimentation.LeetCode.Solutions.PoorPigs;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PoorPigs;

// Harness only. Both strategies are PoorPigsSolution's - this file just pins them to
// a thousand buckets with four rounds of testing, which need five pigs, and the
// zero-pigs case where a single bucket needs no testing at all. LeetCode's published
// examples run in CatalogCaseTests.
public sealed partial class PoorPigsSolutionTests
{
    public static TheoryData<int, int, int, int> Cases =>
        new()
        {
            { 1000, 15, 60, 5 },
            { 1, 15, 15, 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinPigsByLinearRecompute_Cases_ReturnsMinimumPigCount(
        int buckets, int minutesToDie, int minutesToTest, int expected)
    {
        var actual = PoorPigsSolution.MinPigsByLinearRecompute(buckets, minutesToDie, minutesToTest);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinPigsByBinarySearch_Cases_ReturnsMinimumPigCount(
        int buckets, int minutesToDie, int minutesToTest, int expected)
    {
        var actual = PoorPigsSolution.MinPigsByBinarySearch(buckets, minutesToDie, minutesToTest);

        Assert.Equal(expected, actual);
    }
}
