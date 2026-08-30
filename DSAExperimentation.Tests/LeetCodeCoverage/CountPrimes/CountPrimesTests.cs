using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountPrimes;

// LeetCode 204. Count Primes: Sieve of Eratosthenes over this repo's own
// DynamicArray<bool> as the composite-tracking array, marking multiples of
// each newly found prime starting at its square.
public sealed partial class CountPrimesTests
{
    [Theory]
    [InlineData(10, 4)]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    public void CountPrimesBelow_Examples_ReturnsPrimeCount(int n, int expected) => Assert.Equal(expected, CountPrimesBelowN(n));

    private static int CountPrimesBelowN(int n)
    {
        if (n < 2)
        {
            return 0;
        }

        var isComposite = new DynamicArray<bool>();
        for (var i = 0; i < n; i++)
        {
            isComposite.Add(false);
        }

        for (var i = 2; i * i < n; i++)
        {
            if (isComposite.Get(i))
            {
                continue;
            }

            for (var multiple = i * i; multiple < n; multiple += i)
            {
                isComposite.Set(multiple, true);
            }
        }

        var count = 0;
        for (var i = 2; i < n; i++)
        {
            if (!isComposite.Get(i))
            {
                count++;
            }
        }

        return count;
    }
}
