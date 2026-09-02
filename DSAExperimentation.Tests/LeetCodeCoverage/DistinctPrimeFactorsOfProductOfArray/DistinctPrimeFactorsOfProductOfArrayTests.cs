using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DistinctPrimeFactorsOfProductOfArray;

// LeetCode 2521. Distinct Prime Factors of Product of Array: the product's prime
// factors are exactly the union of each element's own prime factors, so every
// element (each <= 1000, so cheap to trial-divide) is factored on its own and fed
// into this repo's own Set<int> for dedup - the same trial-division-into-Set<int>
// composition NumberOfCommonFactorsTests/LargestComponentSizeByCommonFactorTests
// already use for their own factor enumeration, applied here without ever forming
// the (potentially astronomically large) product itself.
public sealed class DistinctPrimeFactorsOfProductOfArrayTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 4, 3, 7, 10, 6], 4 },
            { [2, 4, 8, 16], 1 },
            { [997], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void DistinctPrimeFactors_LeetCodeExamples_ReturnsCount(int[] nums, int expected)
        => Assert.Equal(expected, DistinctPrimeFactors(nums));

    private static int DistinctPrimeFactors(int[] nums)
    {
        var primes = new Set<int>();

        foreach (var num in nums)
        {
            AddPrimeFactors(num, primes);
        }

        return primes.Count;
    }

    private static void AddPrimeFactors(int value, Set<int> primes)
    {
        var remaining = value;

        for (var divisor = 2; divisor * divisor <= remaining; divisor++)
        {
            if (remaining % divisor != 0)
            {
                continue;
            }

            primes.TryAdd(divisor);

            while (remaining % divisor == 0)
            {
                remaining /= divisor;
            }
        }

        if (remaining > 1)
        {
            primes.TryAdd(remaining);
        }
    }
}
