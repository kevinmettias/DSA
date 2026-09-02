using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Longest Binary Subsequence Less Than or Equal to K (LC 2311): brute-force subset
// enumeration (every one of the 2^n subsequences, value checked against k) vs. the
// O(n) right-to-left greedy (LongestBinarySubsequenceLessThanOrEqualToKTests' own
// algorithm) - always keep every '0' (never adds value, only length), and greedily
// accept the cheapest (rightmost) '1's while still affordable. No repo primitive
// applies here - a pure single-pass greedy scan over the string itself, the same
// "no stronger reusable primitive" shape already established for GasStation/
// JumpGame/MaximumProductSubarray. Length is kept small (<=20) so the 2^n
// brute-force baseline finishes in reasonable time; the bits are randomized (not
// all-1s or all-0s) so brute force can't short-circuit on an early degenerate case.
[MemoryDiagnoser]
public class LongestBinarySubsequenceLessThanOrEqualToKBenchmarks
{
    private const int RandomSeed = 2311; // LC problem number
    private const int MaxAffordablePower = 30; // 2^30 > 1e9, k's maximum possible value
    private const int K = 100;

    [Params(16, 20)]
    public int Length;

    private string _bits = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _bits = new string(Enumerable.Range(0, Length).Select(_ => random.Next(2) == 0 ? '0' : '1').ToArray());
    }

    [Benchmark(Baseline = true)]
    public int BruteForceSubsetEnumeration()
    {
        var best = 0;

        for (var mask = 1; mask < 1 << _bits.Length; mask++)
        {
            var (value, length) = ValueAndLength(mask);

            if (value <= K && length > best)
            {
                best = length;
            }
        }

        return best;
    }

    private (long Value, int Length) ValueAndLength(int mask)
    {
        long value = 0;
        var length = 0;

        for (var i = 0; i < _bits.Length; i++)
        {
            if ((mask & (1 << i)) == 0)
            {
                continue;
            }

            value = (value << 1) + (_bits[i] - '0');
            length++;
        }

        return (value, length);
    }

    [Benchmark]
    public int GreedyRightToLeftScan()
    {
        var length = 0;
        long value = 0;
        var power = 0;

        for (var i = _bits.Length - 1; i >= 0; i--)
        {
            if (_bits[i] == '0')
            {
                length++;
                power++;
                continue;
            }

            if (power >= MaxAffordablePower)
            {
                continue;
            }

            var weight = 1L << power;
            if (value + weight <= K)
            {
                value += weight;
                power++;
                length++;
            }
        }

        return length;
    }
}
