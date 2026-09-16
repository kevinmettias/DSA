using DSAExperimentation.LeetCode.NumberOfIntegersWithPopcountDepthEqualToKI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfIntegersWithPopcountDepthEqualToKI;

// Harness only. The popcount-depth counting itself is
// NumberOfIntegersWithPopcountDepthEqualToKISolution's - this file pins both
// strategies to LeetCode's published examples plus two hand-verified boundary
// cases: desiredDepth = 0 (only x = 1 qualifies) and upperBound = 10,
// desiredDepth = 1 (the powers of two {2, 4, 8}, deliberately excluding x = 1
// itself - the case the combinatorial strategy's popcount-1-bucket adjustment
// exists for).
public sealed class NumberOfIntegersWithPopcountDepthEqualToKITests
{
    public static TheoryData<long, int, long> Examples =>
        new()
        {
            { 4L, 1, 2L },
            { 7L, 2, 3L },
            { 1L, 0, 1L },
            { 10L, 1, 3L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PopcountDepthByBruteForce_LeetCodeExamples_ReturnsCountWithMatchingPopcountDepth(
        long upperBound, int desiredDepth, long expected)
    {
        var actual = NumberOfIntegersWithPopcountDepthEqualToKISolution.PopcountDepthByBruteForce(upperBound, desiredDepth);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void PopcountDepthByPopcountCombinatorics_LeetCodeExamples_ReturnsCountWithMatchingPopcountDepth(
        long upperBound, int desiredDepth, long expected)
    {
        var actual = NumberOfIntegersWithPopcountDepthEqualToKISolution.PopcountDepthByPopcountCombinatorics(upperBound, desiredDepth);

        Assert.Equal(expected, actual);
    }
}
