using BenchmarkDotNet.Attributes;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Sum of Total Strength of Wizards (LC 2281): BruteForce is the textbook O(n^2) - for every
// start index, extend a running minimum and running sum right across the rest of the array.
// MonotonicStackContribution instead reuses the exact same monotonic-stack sweep
// SumOfSubarrayMinimumsBenchmarks already runs over this repo's own Stack<int> (previous
// strictly-smaller / next smaller-or-equal boundaries per index), extended with a
// prefix-sum-of-prefix-sums array so each index's weighted contribution (min * sum, over every
// subarray it is the minimum of) is computed in O(1) - O(n) overall instead of O(n^2). _strength
// is a random permutation so BruteForce's inner loop always runs its full remaining length.
[MemoryDiagnoser]
public class SumOfTotalStrengthOfWizardsBenchmarks
{
    private const int Modulus = 1_000_000_007;
    private const int RandomSeed = 2281;

    [Params(200, 2_000)]
    public int Length;

    private int[] _strength = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _strength = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        long total = 0;

        for (var start = 0; start < _strength.Length; start++)
        {
            var min = _strength[start];
            long sum = 0;

            for (var end = start; end < _strength.Length; end++)
            {
                min = Math.Min(min, _strength[end]);
                sum += _strength[end];
                total = (total + ((min * (sum % Modulus)) % Modulus)) % Modulus;
            }
        }

        return (int)total;
    }

    [Benchmark]
    public int MonotonicStackContribution()
    {
        var previousSmaller = ComputePreviousSmallerIndex(_strength);
        var nextSmallerOrEqual = ComputeNextSmallerOrEqualIndex(_strength);
        var prefix = ComputePrefixSums(_strength);
        var prefixOfPrefix = ComputePrefixOfPrefixSums(prefix);

        return SumWeightedContributions(_strength, previousSmaller, nextSmallerOrEqual, prefixOfPrefix);
    }

    private static int[] ComputePreviousSmallerIndex(int[] strength)
    {
        var left = new int[strength.Length];
        var stack = new RepoIntStack();

        for (var i = 0; i < strength.Length; i++)
        {
            while (stack.TryPeek(out var top) && strength[top] >= strength[i])
            {
                stack.TryPop(out _);
            }

            left[i] = stack.TryPeek(out var previous) ? previous : -1;
            stack.Push(i);
        }

        return left;
    }

    private static int[] ComputeNextSmallerOrEqualIndex(int[] strength)
    {
        var right = new int[strength.Length];
        var stack = new RepoIntStack();

        for (var i = strength.Length - 1; i >= 0; i--)
        {
            while (stack.TryPeek(out var top) && strength[top] > strength[i])
            {
                stack.TryPop(out _);
            }

            right[i] = stack.TryPeek(out var next) ? next : strength.Length;
            stack.Push(i);
        }

        return right;
    }

    private static long[] ComputePrefixSums(int[] strength)
    {
        var prefix = new long[strength.Length + 1];

        for (var i = 0; i < strength.Length; i++)
        {
            prefix[i + 1] = prefix[i] + strength[i];
        }

        return prefix;
    }

    private static long[] ComputePrefixOfPrefixSums(long[] prefix)
    {
        var prefixOfPrefix = new long[prefix.Length + 1];

        for (var i = 0; i < prefix.Length; i++)
        {
            prefixOfPrefix[i + 1] = prefixOfPrefix[i] + prefix[i];
        }

        return prefixOfPrefix;
    }

    private static int SumWeightedContributions(int[] strength, int[] previousSmaller, int[] nextSmallerOrEqual, long[] prefixOfPrefix)
    {
        long total = 0;

        for (var i = 0; i < strength.Length; i++)
        {
            var left = previousSmaller[i];
            var right = nextSmallerOrEqual[i];

            var sumOfSumsEndingAtOrAfterI = prefixOfPrefix[right + 1] - prefixOfPrefix[i + 1];
            var sumOfSumsStartingAtOrBeforeI = prefixOfPrefix[i + 1] - prefixOfPrefix[left + 1];

            var weightedSum = ((long)(i - left) * sumOfSumsEndingAtOrAfterI) - ((long)(right - i) * sumOfSumsStartingAtOrBeforeI);
            var contribution = Mod(strength[i]) * Mod(weightedSum) % Modulus;

            total = (total + contribution) % Modulus;
        }

        return (int)total;
    }

    private static long Mod(long value) => ((value % Modulus) + Modulus) % Modulus;
}
