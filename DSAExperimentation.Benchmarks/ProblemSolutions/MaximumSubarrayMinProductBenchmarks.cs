using BenchmarkDotNet.Attributes;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Subarray Min-Product (LC 1856): BruteForce checks every subarray
// directly, tracking a running min and sum, O(n^2). MonotonicStackSweep
// reuses the Sum of Subarray Minimums (LC 907) technique - two passes over
// this repo's own Stack<int> find each element's maximal span as the
// minimum, then a prefix-sum array turns "min * span sum" into O(1) per
// element, O(n) overall. _arr is a random permutation so BruteForce's inner
// loop always runs its full remaining length.
[MemoryDiagnoser]
public class MaximumSubarrayMinProductBenchmarks
{
    private const int Modulus = 1_000_000_007;

    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 1856;

    [Params(200, 3_000)]
    public int Length;

    private int[] _arr = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _arr = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce()
    {
        var best = 0L;

        for (var start = 0; start < _arr.Length; start++)
        {
            var min = _arr[start];
            var sum = 0L;

            for (var end = start; end < _arr.Length; end++)
            {
                min = Math.Min(min, _arr[end]);
                sum += _arr[end];
                best = Math.Max(best, min * sum);
            }
        }

        return best % Modulus;
    }

    [Benchmark]
    public long MonotonicStackSweep()
    {
        var n = _arr.Length;
        var prefixSum = BuildPrefixSums(n);

        var leftBound = ComputeNearestNotSmallerIndex(n, forward: true);
        var rightBound = ComputeNearestNotSmallerIndex(n, forward: false);

        return ComputeBestMinProduct(n, prefixSum, leftBound, rightBound) % Modulus;
    }

    private long[] BuildPrefixSums(int n)
    {
        var prefixSum = new long[n + 1];
        for (var i = 0; i < n; i++)
        {
            prefixSum[i + 1] = prefixSum[i] + _arr[i];
        }

        return prefixSum;
    }

    // For each index, the nearest neighbor (in the given direction) whose value is
    // strictly smaller than _arr[i] - the boundary of the maximal span where _arr[i]
    // is the minimum. Scanning forward yields the left bound (exclusive, +1'd into an
    // inclusive start); scanning backward yields the right bound (exclusive).
    private int[] ComputeNearestNotSmallerIndex(int n, bool forward)
    {
        var bound = new int[n];
        var stack = new RepoIntStack();
        var start = forward ? 0 : n - 1;
        var step = forward ? 1 : -1;

        for (var count = 0; count < n; count++)
        {
            var i = start + count * step;

            while (stack.TryPeek(out var top) && _arr[top] >= _arr[i])
            {
                stack.TryPop(out _);
            }

            var fallback = forward ? 0 : n;
            var offset = forward ? 1 : 0;
            bound[i] = stack.TryPeek(out var neighbor) ? neighbor + offset : fallback;
            stack.Push(i);
        }

        return bound;
    }

    private long ComputeBestMinProduct(int n, long[] prefixSum, int[] leftBound, int[] rightBound)
    {
        var best = 0L;
        for (var i = 0; i < n; i++)
        {
            var sum = prefixSum[rightBound[i]] - prefixSum[leftBound[i]];
            best = Math.Max(best, _arr[i] * sum);
        }

        return best;
    }
}
