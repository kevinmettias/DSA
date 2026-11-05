using DSAExperimentation.LeetCode.Solutions.NumberOfIntegersWithPopcountDepthEqualToKI;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfIntegersWithPopcountDepthEqualToKI;

// Harness only. The popcount-depth counting itself is
// NumberOfIntegersWithPopcountDepthEqualToKISolution's - this file pins both
// strategies to two hand-verified boundary
// cases: desiredDepth = 0 (only x = 1 qualifies) and upperBound = 10,
// desiredDepth = 1 (the powers of two {2, 4, 8}, deliberately excluding x = 1
// itself - the case the combinatorial strategy's popcount-1-bucket adjustment
// exists for).
public sealed partial class NumberOfIntegersWithPopcountDepthEqualToKISolutionTests
{
    public static TheoryData<long, int, long> Cases =>
        new()
        {
            { 1L, 0, 1L },
            { 10L, 1, 3L },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void PopcountDepthByBruteForce_Cases_ReturnsCountWithMatchingPopcountDepth(
        long upperBound, int desiredDepth, long expected)
    {
        var actual = NumberOfIntegersWithPopcountDepthEqualToKISolution.PopcountDepthByBruteForce(upperBound, desiredDepth);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void PopcountDepthByPopcountCombinatorics_Cases_ReturnsCountWithMatchingPopcountDepth(
        long upperBound, int desiredDepth, long expected)
    {
        var actual = NumberOfIntegersWithPopcountDepthEqualToKISolution.PopcountDepthByPopcountCombinatorics(upperBound, desiredDepth);

        Assert.Equal(expected, actual);
    }
}
