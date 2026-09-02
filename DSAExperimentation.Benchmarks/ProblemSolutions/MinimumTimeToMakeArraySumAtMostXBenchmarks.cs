using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Time to Make Array Sum At Most x (LC 2809): both arms sort the (nums1,nums2)
// pairs ascending by nums2 and run the same 0/1-knapsack-shaped recurrence over them, so
// the double loop stays O(n^2) time either way - what differs is the DP's own footprint.
// DenseTableDynamicProgramming is the textbook formulation with Array.Sort (BCL) and a
// dense long[n+1,n+1] table (dp[i,j] = max reduction using the first i sorted pairs with
// exactly j zeroed) - O(n^2) extra space. SortedRollingKnapsack is
// MinimumTimeToMakeArraySumAtMostXTests' approach: this repo's own MergeSort over an
// ArrayIndexedSequence<(int,int)>, then the standard backward-iteration knapsack
// collapses the table to a single long[n+1] rolling row - O(n) extra space. x is set
// below any reachable sum so both arms are forced through every candidate operation
// count instead of returning on the first one checked.
[MemoryDiagnoser]
public class MinimumTimeToMakeArraySumAtMostXBenchmarks
{
    private const int RandomSeed = 2809; // LC problem number
    private const int ValueUpperBoundExclusive = 1_000;
    private const int UnreachableTarget = 0;

    [Params(200, 1_000)]
    public int Length;

    private int[] _nums1 = null!;
    private int[] _nums2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums1 = Enumerable.Range(0, Length).Select(_ => random.Next(1, ValueUpperBoundExclusive)).ToArray();
        _nums2 = Enumerable.Range(0, Length).Select(_ => random.Next(1, ValueUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int DenseTableDynamicProgramming()
    {
        var n = _nums1.Length;
        var pairs = new (int Nums1, int Nums2)[n];

        for (var i = 0; i < n; i++)
        {
            pairs[i] = (_nums1[i], _nums2[i]);
        }

        Array.Sort(pairs, (a, b) => a.Nums2.CompareTo(b.Nums2));

        var dp = new long[n + 1, n + 1];

        for (var i = 1; i <= n; i++)
        {
            var (a1, a2) = pairs[i - 1];

            for (var j = 0; j <= n; j++)
            {
                dp[i, j] = dp[i - 1, j];

                if (j >= 1)
                {
                    dp[i, j] = Math.Max(dp[i, j], dp[i - 1, j - 1] + a1 + ((long)a2 * j));
                }
            }
        }

        return FindMinimumTime(n, t => dp[n, t]);
    }

    [Benchmark]
    public int SortedRollingKnapsack()
    {
        var n = _nums1.Length;
        var pairs = BuildPairsAscendingByNums2();
        var dp = new long[n + 1];

        for (var i = 0; i < n; i++)
        {
            var (a1, a2) = pairs[i];

            for (var j = Math.Min(i + 1, n); j >= 1; j--)
            {
                dp[j] = Math.Max(dp[j], dp[j - 1] + a1 + ((long)a2 * j));
            }
        }

        return FindMinimumTime(n, t => dp[t]);
    }

    private (int Nums1, int Nums2)[] BuildPairsAscendingByNums2()
    {
        var pairs = new (int Nums1, int Nums2)[_nums1.Length];

        for (var i = 0; i < _nums1.Length; i++)
        {
            pairs[i] = (_nums1[i], _nums2[i]);
        }

        var byNums2Ascending = Comparer<(int Nums1, int Nums2)>.Create((a, b) => a.Nums2.CompareTo(b.Nums2));
        MergeSort.Sort<(int Nums1, int Nums2), ArrayIndexedSequence<(int Nums1, int Nums2)>>(
            new ArrayIndexedSequence<(int Nums1, int Nums2)>(pairs), byNums2Ascending);

        return pairs;
    }

    private int FindMinimumTime(int n, Func<int, long> reductionAt)
    {
        long sum1 = 0;
        long sum2 = 0;

        foreach (var value in _nums1)
        {
            sum1 += value;
        }

        foreach (var value in _nums2)
        {
            sum2 += value;
        }

        for (var t = 0; t <= n; t++)
        {
            if (sum1 + (sum2 * t) - reductionAt(t) <= UnreachableTarget)
            {
                return t;
            }
        }

        return -1;
    }
}
