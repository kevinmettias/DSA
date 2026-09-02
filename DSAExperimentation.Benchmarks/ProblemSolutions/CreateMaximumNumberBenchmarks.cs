using BenchmarkDotNet.Attributes;
using MaxDigitsStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Create Maximum Number (LC 321): both benchmarks try every valid k-split and
// merge identically - what varies is how the per-array "largest subsequence of
// length L" is pulled out. NaiveSubsequenceScan is the O(n * L) greedy people
// reach for first (repeatedly scan the still-valid window for its max digit);
// MonotonicStackSubsequence is this repo's own O(n) Stack<int> sweep
// (LargestRectangleInHistogram/MultiplyStrings precedent) instead.
[MemoryDiagnoser]
public class CreateMaximumNumberBenchmarks
{
    private const int RandomSeed = 321; // LeetCode problem number

    private const int DigitUpperBoundExclusive = 10;

    [Params(20, 100)]
    public int Length;

    private int[] _nums1 = null!;
    private int[] _nums2 = null!;
    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums1 = Enumerable.Range(0, Length).Select(_ => random.Next(0, DigitUpperBoundExclusive)).ToArray();
        _nums2 = Enumerable.Range(0, Length).Select(_ => random.Next(0, DigitUpperBoundExclusive)).ToArray();
        _k = Length;
    }

    [Benchmark(Baseline = true)]
    public int[] NaiveSubsequenceScan() => MaxNumber(_nums1, _nums2, _k, NaiveMaxSubsequence);

    [Benchmark]
    public int[] MonotonicStackSubsequence() => MaxNumber(_nums1, _nums2, _k, StackMaxSubsequence);

    private static int[] MaxNumber(int[] nums1, int[] nums2, int k, Func<int[], int, int[]> maxSubsequence)
    {
        var best = Array.Empty<int>();

        var lowI = Math.Max(0, k - nums2.Length);
        var highI = Math.Min(k, nums1.Length);

        for (var i = lowI; i <= highI; i++)
        {
            var subsequence1 = maxSubsequence(nums1, i);
            var subsequence2 = maxSubsequence(nums2, k - i);
            var candidate = MergePreferringLarger(subsequence1, subsequence2);
            if (IsGreaterOrEqual(candidate, 0, best, 0))
            {
                best = candidate;
            }
        }

        return best;
    }

    // The naive O(n * length) approach: repeatedly scan the still-eligible window
    // for its largest digit (leftmost on ties), then continue past it.
    private static int[] NaiveMaxSubsequence(int[] nums, int length)
    {
        var result = new int[length];
        var start = 0;
        var n = nums.Length;

        for (var remaining = length; remaining >= 1; remaining--)
        {
            var end = n - remaining + 1;
            var maxIndex = start;
            for (var i = start + 1; i < end; i++)
            {
                if (nums[i] > nums[maxIndex])
                {
                    maxIndex = i;
                }
            }

            result[length - remaining] = nums[maxIndex];
            start = maxIndex + 1;
        }

        return result;
    }

    private static int[] StackMaxSubsequence(int[] nums, int length)
    {
        var stack = BuildMonotonicStack(nums, nums.Length - length);
        return ExtractSubsequence(stack, length);
    }

    private static MaxDigitsStack BuildMonotonicStack(int[] nums, int drop)
    {
        var stack = new MaxDigitsStack();

        foreach (var num in nums)
        {
            while (drop > 0 && stack.TryPeek(out var top) && top < num)
            {
                stack.TryPop(out _);
                drop--;
            }

            stack.Push(num);
        }

        return stack;
    }

    private static int[] ExtractSubsequence(MaxDigitsStack stack, int length)
    {
        while (stack.Count > length)
        {
            stack.TryPop(out _);
        }

        var result = new int[length];
        for (var i = length - 1; i >= 0; i--)
        {
            stack.TryPop(out result[i]);
        }

        return result;
    }

    private static int[] MergePreferringLarger(int[] a, int[] b)
    {
        var merged = new int[a.Length + b.Length];
        var ai = 0;
        var bi = 0;

        for (var m = 0; m < merged.Length; m++)
        {
            merged[m] = IsGreaterOrEqual(a, ai, b, bi) ? a[ai++] : b[bi++];
        }

        return merged;
    }

    private static bool IsGreaterOrEqual(int[] a, int ai, int[] b, int bi)
    {
        while (ai < a.Length && bi < b.Length && a[ai] == b[bi])
        {
            ai++;
            bi++;
        }

        return bi == b.Length || (ai < a.Length && a[ai] > b[bi]);
    }
}
