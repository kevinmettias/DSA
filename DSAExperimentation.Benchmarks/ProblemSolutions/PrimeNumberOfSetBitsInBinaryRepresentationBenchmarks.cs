using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Prime Number of Set Bits in Binary Representation (LC 762): checking each
// value's popcount for primality via trial division on every call vs. this
// repo's own Set<int> (HashMap<Element,bool>-backed) holding the fixed small set
// of primes <= 20 once, then doing an O(1) membership check per value - right <=
// 10^6 bounds every popcount to at most 20, so the set never grows past 8 entries
// regardless of range width.
[MemoryDiagnoser]
public class PrimeNumberOfSetBitsInBinaryRepresentationBenchmarks
{
    [Params(1_000, 100_000)]
    public int RangeWidth;

    private int _left;
    private int _right;

    [GlobalSetup]
    public void Setup()
    {
        _left = 1;
        _right = _left + RangeWidth - 1;
    }

    [Benchmark(Baseline = true)]
    public int TrialDivisionPerValue()
    {
        var count = 0;

        for (var value = _left; value <= _right; value++)
        {
            if (IsPrime(CountSetBits(value)))
            {
                count++;
            }
        }

        return count;
    }

    [Benchmark]
    public int PrecomputedSetLookup()
    {
        var primeBitCounts = BuildPrimeBitCounts();
        var count = 0;

        for (var value = _left; value <= _right; value++)
        {
            if (primeBitCounts.Has(CountSetBits(value)))
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

    private static bool IsPrime(int value)
    {
        if (value < 2)
        {
            return false;
        }

        for (var divisor = 2; divisor * divisor <= value; divisor++)
        {
            if (value % divisor == 0)
            {
                return false;
            }
        }

        return true;
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
