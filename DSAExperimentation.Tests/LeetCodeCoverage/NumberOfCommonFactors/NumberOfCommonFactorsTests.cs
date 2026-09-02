using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfCommonFactors;

// LeetCode 2427. Number of Common Factors: every common factor of a and b is
// exactly a divisor of gcd(a,b) (Euclid's algorithm - plain arithmetic, no data
// structure of its own), so divisors are enumerated only up to sqrt(gcd) instead of
// scanning up to min(a,b) directly. The two divisors found at each step (i and
// gcd/i) are deduplicated through this repo's own Set<int> (HashMap-backed) for the
// i*i==gcd boundary case, the same role CountLatticePointsInsideACircleTests'
// Set<(int,int)> plays for its own union-of-points dedupe.
public sealed partial class NumberOfCommonFactorsTests
{
    [Theory]
    [InlineData(12, 6, 4)]
    [InlineData(25, 15, 2)]
    [InlineData(1, 1, 1)]
    public void CountCommonFactors_LeetCodeExamples_ReturnsExpectedCount(int a, int b, int expected)
        => Assert.Equal(expected, CountCommonFactors(a, b));

    private static int CountCommonFactors(int a, int b)
    {
        var gcd = Gcd(a, b);
        var divisors = new Set<int>();

        for (var i = 1; (long)i * i <= gcd; i++)
        {
            if (gcd % i == 0)
            {
                divisors.TryAdd(i);
                divisors.TryAdd(gcd / i);
            }
        }

        return divisors.Count;
    }

    private static int Gcd(int a, int b)
    {
        while (b != 0)
        {
            (a, b) = (b, a % b);
        }

        return a;
    }
}
