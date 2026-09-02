using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Different Subsequences GCDs (LC 1819): rescanning the whole array for
// every candidate GCD (O(max * n)) vs. walking only each candidate's multiples with
// this repo's own Set<int> for O(1) presence checks (O(max log max), harmonic).
// MaxValueExclusive is held fixed across both [Params] sizes so the array length
// (_nums.Length) varies independently of the value domain (_maxValue) - brute force
// scales with both, the Set-based walk only with the value domain.
[MemoryDiagnoser]
public class NumberOfDifferentSubsequencesGCDsBenchmarks
{
    private const int MaxValueExclusive = 5_000;

    [Params(200, 2_000)]
    public int Length;

    private int[] _nums = null!;
    private int _maxValue;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).Distinct().ToArray();
        _maxValue = _nums.Max();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceScanWholeArray()
    {
        var count = 0;

        for (var candidate = 1; candidate <= _maxValue; candidate++)
        {
            if (HasSubsequenceWithGcd(candidate))
            {
                count++;
            }
        }

        return count;
    }

    private bool HasSubsequenceWithGcd(int candidate)
    {
        var runningGcd = 0;

        foreach (var num in _nums)
        {
            if (num % candidate != 0)
            {
                continue;
            }

            runningGcd = Gcd(runningGcd, num);

            if (runningGcd == candidate)
            {
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public int SetPresenceOverMultiples()
    {
        var present = BuildPresenceSet();
        var count = 0;

        for (var candidate = 1; candidate <= _maxValue; candidate++)
        {
            if (HasMultipleWithGcd(present, candidate))
            {
                count++;
            }
        }

        return count;
    }

    private Set<int> BuildPresenceSet()
    {
        var present = new Set<int>();

        foreach (var num in _nums)
        {
            present.TryAdd(num);
        }

        return present;
    }

    private bool HasMultipleWithGcd(Set<int> present, int candidate)
    {
        var runningGcd = 0;

        for (var multiple = candidate; multiple <= _maxValue; multiple += candidate)
        {
            if (!present.Has(multiple))
            {
                continue;
            }

            runningGcd = Gcd(runningGcd, multiple);

            if (runningGcd == candidate)
            {
                return true;
            }
        }

        return false;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
