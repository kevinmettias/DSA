using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PrimeNumberOfSetBitsInBinaryRepresentation;

// LeetCode 762. Prime Number of Set Bits in Binary Representation: right <= 10^6
// bounds every candidate's popcount to at most 20, so "is this bit count prime"
// reduces to membership in the fixed small set of primes <= 20 - this repo's own
// Set<int> (HashMap<Element,bool>-backed), the same closed-membership-check role
// TwoSumTests'/RepeatedDNASequencesTests' HashMap/Set already play.
public sealed partial class PrimeNumberOfSetBitsInBinaryRepresentationTests
{
    private static readonly Set<int> PrimeBitCounts = BuildPrimeBitCounts();

    [Fact]
    public void CountPrimeSetBits_ClassicExampleOne_ReturnsExpectedCount()
    {
        Assert.Equal(4, CountPrimeSetBits(6, 10));
    }

    [Fact]
    public void CountPrimeSetBits_ClassicExampleTwo_ReturnsExpectedCount()
    {
        Assert.Equal(5, CountPrimeSetBits(10, 15));
    }

    private static int CountPrimeSetBits(int left, int right)
    {
        var count = 0;

        for (var value = left; value <= right; value++)
        {
            if (PrimeBitCounts.Has(CountSetBits(value)))
            {
                count++;
            }
        }

        return count;
    }

    private static int CountSetBits(int value)
    {
        var bits = 0;
        while (value != 0)
        {
            value &= value - 1;
            bits++;
        }

        return bits;
    }

    private static Set<int> BuildPrimeBitCounts()
    {
        var primes = new Set<int>();
        foreach (var candidate in new[] { 2, 3, 5, 7, 11, 13, 17, 19 })
        {
            primes.TryAdd(candidate);
        }

        return primes;
    }
}
