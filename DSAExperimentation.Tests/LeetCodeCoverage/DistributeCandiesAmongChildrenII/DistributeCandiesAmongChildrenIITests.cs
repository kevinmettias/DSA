using DSAExperimentation.LeetCode.DistributeCandiesAmongChildrenII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DistributeCandiesAmongChildrenII;

// Harness only: both strategies live in DistributeCandiesAmongChildrenIISolution.
// LC 2929 restates LC 2928's exact examples at its larger (long-valued) bound, so
// the same two inputs still pin the answer, this time through the long-typed
// signature.
public sealed class DistributeCandiesAmongChildrenIITests
{
    public static TheoryData<int, int, long> Examples =>
        new()
        {
            { 5, 2, 3L }, // (1,2,2),(2,1,2),(2,2,1)
            { 3, 3, 10L }, // every nonnegative (a,b,c) with a+b+c=3 already respects limit=3
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountWaysByBruteForce_LeetCodeExamples_ReturnsDistributionCount(int n, int limit, long expected)
    {
        var actual = DistributeCandiesAmongChildrenIISolution.CountWaysByBruteForce(n, limit);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountWaysByInclusionExclusion_LeetCodeExamples_ReturnsDistributionCount(int n, int limit, long expected)
    {
        var actual = DistributeCandiesAmongChildrenIISolution.CountWaysByInclusionExclusion(n, limit);
        Assert.Equal(expected, actual);
    }
}
