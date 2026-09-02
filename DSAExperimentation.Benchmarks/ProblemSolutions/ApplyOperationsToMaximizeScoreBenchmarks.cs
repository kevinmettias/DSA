using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.Domain.Modular;
using NumberStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Apply Operations to Maximize Score (LC 2818): both arms share the same prime-score
// precompute (trial division), value-descending sort (this repo's own MergeSort +
// ArrayIndexedSequence), and greedy modular-power consumption (ModularArithmetic.Power)
// - the one thing that varies is how each index's "how many subarrays pick me"
// boundary is found. LinearBoundaryScan expands outward from each index with a plain
// while loop - O(n) worst case per index, O(n^2) overall whenever scores run long
// ties, which is common here since a prime score only ranges over a handful of small
// integers. StackBoundaryScan instead finds both boundaries with a single monotonic
// pass each over this repo's own Stack<T> (the same LIFO primitive
// AddTwoNumbersIIBenchmarks already uses) - O(n) overall, since every index is
// pushed and popped at most once per pass.
[MemoryDiagnoser]
public class ApplyOperationsToMaximizeScoreBenchmarks
{
    private const int RandomSeed = 2818; // LeetCode problem number
    private const int MaxValueExclusive = 100_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;
    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(2, MaxValueExclusive)).ToArray();
        _k = Length;
    }

    [Benchmark(Baseline = true)]
    public long LinearBoundaryScan()
    {
        var scores = ComputePrimeScores(_nums);
        var (left, right) = ComputeBoundariesByLinearScan(scores);
        return GreedyMaximumScore(_nums, left, right, _k);
    }

    [Benchmark]
    public long StackBoundaryScan()
    {
        var scores = ComputePrimeScores(_nums);
        var left = ComputeLeftBoundaries(scores);
        var right = ComputeRightBoundaries(scores);
        return GreedyMaximumScore(_nums, left, right, _k);
    }

    private static int[] ComputePrimeScores(int[] nums)
    {
        var scores = new int[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            scores[i] = PrimeScore(nums[i]);
        }

        return scores;
    }

    private static int PrimeScore(int value)
    {
        var score = 0;

        for (var factor = 2; (long)factor * factor <= value; factor++)
        {
            if (value % factor != 0)
            {
                continue;
            }

            score++;

            while (value % factor == 0)
            {
                value /= factor;
            }
        }

        if (value > 1)
        {
            score++;
        }

        return score;
    }

    private static (int[] Left, int[] Right) ComputeBoundariesByLinearScan(int[] scores)
    {
        var n = scores.Length;
        var left = new int[n];
        var right = new int[n];

        for (var i = 0; i < n; i++)
        {
            var l = i - 1;
            while (l >= 0 && scores[l] < scores[i])
            {
                l--;
            }

            left[i] = l;

            var r = i + 1;
            while (r < n && scores[r] <= scores[i])
            {
                r++;
            }

            right[i] = r;
        }

        return (left, right);
    }

    private static int[] ComputeLeftBoundaries(int[] scores)
    {
        var left = new int[scores.Length];
        var stack = new NumberStack();

        for (var i = 0; i < scores.Length; i++)
        {
            while (stack.TryPeek(out var top) && scores[top] < scores[i])
            {
                stack.TryPop(out _);
            }

            left[i] = stack.TryPeek(out var boundary) ? boundary : -1;
            stack.Push(i);
        }

        return left;
    }

    private static int[] ComputeRightBoundaries(int[] scores)
    {
        var right = new int[scores.Length];
        var stack = new NumberStack();

        for (var i = scores.Length - 1; i >= 0; i--)
        {
            while (stack.TryPeek(out var top) && scores[top] <= scores[i])
            {
                stack.TryPop(out _);
            }

            right[i] = stack.TryPeek(out var boundary) ? boundary : scores.Length;
            stack.Push(i);
        }

        return right;
    }

    private static long GreedyMaximumScore(int[] nums, int[] left, int[] right, int k)
    {
        var order = new IndexedValue[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            order[i] = new IndexedValue(nums[i], i);
        }

        MergeSort.Sort<IndexedValue, ArrayIndexedSequence<IndexedValue>>(
            new ArrayIndexedSequence<IndexedValue>(order),
            Comparer<IndexedValue>.Create((first, second) => second.Value.CompareTo(first.Value)));

        var result = 1L;
        var remaining = (long)k;

        foreach (var entry in order)
        {
            if (remaining <= 0)
            {
                break;
            }

            var i = entry.Index;
            var available = (long)(i - left[i]) * (right[i] - i);
            var uses = Math.Min(available, remaining);

            result = result * ModularArithmetic.Power(entry.Value, uses) % ModularArithmetic.Modulo;
            remaining -= uses;
        }

        return result;
    }

    private readonly record struct IndexedValue(int Value, int Index);
}
