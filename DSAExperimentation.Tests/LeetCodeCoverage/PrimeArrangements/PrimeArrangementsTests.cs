using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PrimeArrangements;

// LeetCode 1175. Prime Arrangements: a Sieve of Eratosthenes over this repo's own
// DynamicArray<bool> (the same composite-tracking array CountPrimesTests uses)
// counts the primes in [1, n]; primes must fill the prime index slots and
// composites the rest, so the answer is primeCount! * compositeCount! mod 1e9+7.
public sealed partial class PrimeArrangementsTests
{
    private const int Modulo = 1_000_000_007;

    [Theory]
    [InlineData(5, 12)]
    [InlineData(100, 682289015)]
    public void NumPrimeArrangements_Examples_ReturnsExpectedCount(int n, int expected)
        => Assert.Equal(expected, NumPrimeArrangements(n));

    private static int NumPrimeArrangements(int n)
    {
        var primeCount = CountPrimesUpTo(n);
        var compositeCount = n - primeCount;

        return (int)(Factorial(primeCount) * Factorial(compositeCount) % Modulo);
    }

    private static long Factorial(int n)
    {
        var result = 1L;
        for (var i = 2; i <= n; i++)
        {
            result = result * i % Modulo;
        }

        return result;
    }

    private static int CountPrimesUpTo(int n)
    {
        if (n < 2)
        {
            return 0;
        }

        var isComposite = new DynamicArray<bool>();
        for (var i = 0; i <= n; i++)
        {
            isComposite.Add(false);
        }

        for (var i = 2; i * i <= n; i++)
        {
            if (isComposite.Get(i))
            {
                continue;
            }

            for (var multiple = i * i; multiple <= n; multiple += i)
            {
                isComposite.Set(multiple, true);
            }
        }

        var count = 0;
        for (var i = 2; i <= n; i++)
        {
            if (!isComposite.Get(i))
            {
                count++;
            }
        }

        return count;
    }
}
