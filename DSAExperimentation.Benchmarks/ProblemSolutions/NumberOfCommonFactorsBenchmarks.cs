using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Common Factors (LC 2427): the textbook O(min(a,b)) scan (test every
// candidate divisor from 1 up to min(a,b) against both a and b) vs. reducing to
// gcd(a,b) first and enumerating only ITS divisors up to sqrt(gcd) - every common
// factor of a and b is exactly a divisor of gcd(a,b), so nothing above sqrt(gcd)
// ever needs to be tested directly. The divisor found on the "far side" of each
// sqrt(gcd) pair is deduplicated (the i*i==gcd boundary case) through this repo's
// own Set<int> (HashMap-backed), NumberOfCommonFactorsTests' exact composition.
// _a/_b are random within [Magnitude/2, Magnitude], so gcd(_a,_b) stays small
// relative to Magnitude on average - exactly the shape where the sqrt(gcd)
// reduction pays off over the min(a,b) baseline.
[MemoryDiagnoser]
public class NumberOfCommonFactorsBenchmarks
{
    private const int RandomSeed = 2427; // LC problem number

    [Params(10_000, 1_000_000)]
    public int Magnitude;

    private int _a;
    private int _b;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _a = random.Next(Magnitude / 2, Magnitude + 1);
        _b = random.Next(Magnitude / 2, Magnitude + 1);
    }

    [Benchmark(Baseline = true)]
    public int LinearScan()
    {
        var count = 0;
        var limit = Math.Min(_a, _b);

        for (var i = 1; i <= limit; i++)
        {
            if (_a % i == 0 && _b % i == 0)
            {
                count++;
            }
        }

        return count;
    }

    [Benchmark]
    public int GcdDivisorEnumeration()
    {
        var gcd = Gcd(_a, _b);
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
