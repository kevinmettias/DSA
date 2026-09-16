using DSAExperimentation.LeetCode.DistributeCandiesAmongChildrenI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DistributeCandiesAmongChildrenI;

// Harness only: both strategies live in DistributeCandiesAmongChildrenISolution.
// One test method per strategy over one shared set of LeetCode's own examples, so
// a failure names the strategy that broke (TwoSumTests precedent).
public sealed class DistributeCandiesAmongChildrenITests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 5, 2, 3 }, // (1,2,2),(2,1,2),(2,2,1)
            { 3, 3, 10 }, // every nonnegative (a,b,c) with a+b+c=3 already respects limit=3
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountWaysByBruteForce_LeetCodeExamples_ReturnsDistributionCount(int n, int limit, int expected)
    {
        var actual = DistributeCandiesAmongChildrenISolution.CountWaysByBruteForce(n, limit);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountWaysByInclusionExclusion_LeetCodeExamples_ReturnsDistributionCount(int n, int limit, int expected)
    {
        var actual = DistributeCandiesAmongChildrenISolution.CountWaysByInclusionExclusion(n, limit);
        Assert.Equal(expected, actual);
    }
}
