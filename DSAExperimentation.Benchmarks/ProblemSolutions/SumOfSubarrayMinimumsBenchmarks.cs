using BenchmarkDotNet.Attributes;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Sum of Subarray Minimums (LC 907): BruteForce is the textbook O(n^2) - for every
// start index, extend a running minimum right across the rest of the array. The
// monotonic-stack contribution technique instead sweeps this repo's own Stack<int>
// of pending indices twice (DailyTemperatures/NextGreaterElementI precedent) to find
// each index's distance to its previous-strictly-smaller and next-smaller-or-equal
// neighbors, then sums arr[i] * left[i] * right[i] - O(n). _arr is a random
// permutation so BruteForce's inner loop always runs its full remaining length
// (no distinct-value shortcut to break out of early).
[MemoryDiagnoser]
public class SumOfSubarrayMinimumsBenchmarks
{
    private const int Modulus = 1_000_000_007;
    private const int RandomSeed = 907;

    [Params(200, 5_000)]
    public int Length;

    private int[] _arr = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _arr = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        long sum = 0;

        for (var start = 0; start < _arr.Length; start++)
        {
            var min = _arr[start];

            for (var end = start; end < _arr.Length; end++)
            {
                min = Math.Min(min, _arr[end]);
                sum = (sum + min) % Modulus;
            }
        }

        return (int)sum;
    }

    [Benchmark]
    public int MonotonicStackContribution()
    {
        var left = ComputeDistanceToPreviousSmaller(_arr);
        var right = ComputeDistanceToNextSmallerOrEqual(_arr);

        return (int)SumMinimumContributions(_arr, left, right);
    }

    private static int[] ComputeDistanceToPreviousSmaller(int[] arr)
    {
        var left = new int[arr.Length];
        var stack = new RepoIntStack();

        for (var i = 0; i < arr.Length; i++)
        {
            while (stack.TryPeek(out var top) && arr[top] >= arr[i])
            {
                stack.TryPop(out _);
            }

            left[i] = stack.TryPeek(out var previous) ? i - previous : i + 1;
            stack.Push(i);
        }

        return left;
    }

    private static int[] ComputeDistanceToNextSmallerOrEqual(int[] arr)
    {
        var right = new int[arr.Length];
        var stack = new RepoIntStack();

        for (var i = arr.Length - 1; i >= 0; i--)
        {
            while (stack.TryPeek(out var top) && arr[top] > arr[i])
            {
                stack.TryPop(out _);
            }

            right[i] = stack.TryPeek(out var next) ? next - i : arr.Length - i;
            stack.Push(i);
        }

        return right;
    }

    private static long SumMinimumContributions(int[] arr, int[] left, int[] right)
    {
        long sum = 0;

        for (var i = 0; i < arr.Length; i++)
        {
            sum = (sum + ((long)arr[i] * left[i] * right[i])) % Modulus;
        }

        return sum;
    }
}
