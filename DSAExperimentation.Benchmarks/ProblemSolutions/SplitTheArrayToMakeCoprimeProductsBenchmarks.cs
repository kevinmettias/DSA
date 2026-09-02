using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using System.Numerics;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Split the Array to Make Coprime Products (LC 2584): the direct-definition
// approach - a running BigInteger left product and a full BigInteger right
// product, taking a BigInteger.GreatestCommonDivisor at every candidate split
// (values would overflow long well before n reaches a handful of elements) - vs.
// this repo's HashMap<prime,lastIndex> boundary sweep, which never multiplies
// anything: a split at i is coprime exactly when no prime factor seen in
// nums[0..i] ever reappears past i, so tracking each prime's rightmost occurrence
// once up front turns the whole problem into one linear scan comparing a running
// max index against i. Values are drawn from a small shared prime pool (the same
// "force genuine overlaps" intent LargestComponentSizeByCommonFactorBenchmarks'
// generator already uses) so both arms do real, non-trivial work.
[MemoryDiagnoser]
public class SplitTheArrayToMakeCoprimeProductsBenchmarks
{
    private static readonly int[] SharedPrimes = [2, 3, 5, 7, 11, 13, 17, 19];

    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 2584;

    private const int SmallestPrimeFactor = 2;

    [Params(200, 2_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length)
            .Select(_ => SharedPrimes[random.Next(SharedPrimes.Length)] * SharedPrimes[random.Next(SharedPrimes.Length)])
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BigIntegerProductScan() => FindValidSplitByProduct(_nums);

    private static int FindValidSplitByProduct(int[] nums)
    {
        var rightProduct = BigInteger.One;

        foreach (var value in nums)
        {
            rightProduct *= value;
        }

        var leftProduct = BigInteger.One;

        for (var i = 0; i < nums.Length - 1; i++)
        {
            leftProduct *= nums[i];
            rightProduct /= nums[i];

            if (BigInteger.GreatestCommonDivisor(leftProduct, rightProduct) == BigInteger.One)
            {
                return i;
            }
        }

        return -1;
    }

    [Benchmark]
    public int PrimeLastOccurrenceSweep() => FindValidSplitByBoundary(_nums);

    private static int FindValidSplitByBoundary(int[] nums)
    {
        var lastOccurrence = new HashMap<int, int>();

        for (var i = 0; i < nums.Length; i++)
        {
            foreach (var factor in PrimeFactors(nums[i]))
            {
                lastOccurrence.Set(factor, i);
            }
        }

        var boundary = 0;

        for (var i = 0; i < nums.Length - 1; i++)
        {
            foreach (var factor in PrimeFactors(nums[i]))
            {
                lastOccurrence.TryGetValue(factor, out var last);
                boundary = Math.Max(boundary, last);
            }

            if (boundary == i)
            {
                return i;
            }
        }

        return -1;
    }

    private static IEnumerable<int> PrimeFactors(int value)
    {
        for (var factor = SmallestPrimeFactor; factor * factor <= value; factor++)
        {
            if (value % factor != 0)
            {
                continue;
            }

            yield return factor;

            while (value % factor == 0)
            {
                value /= factor;
            }
        }

        if (value > 1)
        {
            yield return value;
        }
    }
}
