using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find Greatest Common Divisor of Array (LC 1979): the min/max scan is the same
// O(n) work either way, so the only real performance question is how the single
// Gcd(min, max) call is computed - repeated subtraction (the textbook first
// algorithm, O(max/min) here) vs. the modulo-based Euclidean algorithm
// (O(log(min))), the same "two ways to compute the same fold" contrast
// CheckIfItIsAGoodArrayBenchmarks already runs for its own array-wide Gcd fold.
// _nums[0] is forced to 1 (subtraction's worst case: gcd(1, max) forces exactly
// max-1 single-unit decrements) and MaxValueExclusive is pushed well past LC
// 1979's own 1000-value bound - NumberOfDifferentSubsequencesGCDsBenchmarks/
// CheckIfItIsAGoodArrayBenchmarks already depart from a problem's literal
// per-element bound the same way, since at the real bound subtraction's O(max)
// cost (a few hundred integer decrements) is too small to separate from the O(n)
// min/max scan itself; only a genuinely large max makes the two Gcd strategies'
// asymptotic gap visible above that shared scan cost.
[MemoryDiagnoser]
public class FindGreatestCommonDivisorOfArrayBenchmarks
{
    private const int RandomSeed = 1979;
    private const int MaxValueExclusive = 2_000_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _nums[0] = 1;
    }

    [Benchmark(Baseline = true)]
    public int SubtractionGcdOfMinAndMax()
    {
        var (min, max) = FindMinAndMax();
        return SubtractionGcd(min, max);
    }

    [Benchmark]
    public int EuclideanGcdOfMinAndMax()
    {
        var (min, max) = FindMinAndMax();
        return EuclideanGcd(min, max);
    }

    private (int Min, int Max) FindMinAndMax()
    {
        var min = _nums[0];
        var max = _nums[0];

        foreach (var num in _nums)
        {
            min = Math.Min(min, num);
            max = Math.Max(max, num);
        }

        return (min, max);
    }

    // The textbook first Gcd algorithm: repeatedly subtract the smaller value from
    // the larger until they're equal. Correct, but O(max/min) per pair - a small
    // min forces one subtraction per unit of the gap instead of one division.
    private static int SubtractionGcd(int a, int b)
    {
        while (a != b)
        {
            if (a > b)
            {
                a -= b;
            }
            else
            {
                b -= a;
            }
        }

        return a;
    }

    private static int EuclideanGcd(int a, int b) => b == 0 ? a : EuclideanGcd(b, a % b);
}
