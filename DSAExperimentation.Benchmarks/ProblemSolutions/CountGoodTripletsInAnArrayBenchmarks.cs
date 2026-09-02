using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Good Triplets in an Array (LC 2179): the textbook O(n^2) pairwise scan (for
// each position j, count smaller-to-the-left and larger-to-the-right by direct
// inner loops) vs. two FenwickTree<int,SumOperation<int>> (this repo's own Binary
// Indexed Tree) sweeps - one forward, one backward - O(n log n) overall, the same
// coordinate-free counting shape CountOfSmallerNumbersAfterSelfBenchmarks/
// ReversePairsBenchmarks already establish for LC 315/493.
[MemoryDiagnoser]
public class CountGoodTripletsInAnArrayBenchmarks
{
    private const int RandomSeed = 2179; // LC problem number

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums1 = null!;
    private int[] _nums2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums1 = ShuffledPermutation(random, Length);
        _nums2 = ShuffledPermutation(random, Length);
    }

    [Benchmark(Baseline = true)]
    public long PairwiseScan()
    {
        var a = BuildRankArray(_nums1, _nums2);
        long total = 0;

        for (var j = 0; j < a.Length; j++)
        {
            total += CountGoodTripletContribution(a, j);
        }

        return total;
    }

    private static long CountGoodTripletContribution(int[] a, int j)
    {
        var leftSmaller = 0;
        for (var i = 0; i < j; i++)
        {
            if (a[i] < a[j])
            {
                leftSmaller++;
            }
        }

        var rightLarger = 0;
        for (var k = j + 1; k < a.Length; k++)
        {
            if (a[k] > a[j])
            {
                rightLarger++;
            }
        }

        return (long)leftSmaller * rightLarger;
    }

    [Benchmark]
    public long FenwickTreeSweep()
    {
        var a = BuildRankArray(_nums1, _nums2);
        var leftSmallerCount = ComputeLeftSmallerCounts(a);
        var rightLargerCount = ComputeRightLargerCounts(a);

        return SumTripletCounts(leftSmallerCount, rightLargerCount);
    }

    private static int[] ComputeLeftSmallerCounts(int[] a)
    {
        var n = a.Length;
        var leftSmallerCount = new int[n];
        var leftTree = new FenwickTree<int, SumOperation<int>>(n);
        for (var i = 0; i < n; i++)
        {
            leftSmallerCount[i] = a[i] == 0 ? 0 : leftTree.PrefixQuery(a[i] - 1);
            leftTree.Add(a[i], 1);
        }

        return leftSmallerCount;
    }

    private static int[] ComputeRightLargerCounts(int[] a)
    {
        var n = a.Length;
        var rightLargerCount = new int[n];
        var rightTree = new FenwickTree<int, SumOperation<int>>(n);
        for (var i = n - 1; i >= 0; i--)
        {
            var smallerToRight = a[i] == 0 ? 0 : rightTree.PrefixQuery(a[i] - 1);
            rightLargerCount[i] = (n - 1 - i) - smallerToRight;
            rightTree.Add(a[i], 1);
        }

        return rightLargerCount;
    }

    private static long SumTripletCounts(int[] leftSmallerCount, int[] rightLargerCount)
    {
        long total = 0;
        for (var j = 0; j < leftSmallerCount.Length; j++)
        {
            total += (long)leftSmallerCount[j] * rightLargerCount[j];
        }

        return total;
    }

    private static int[] BuildRankArray(int[] nums1, int[] nums2)
    {
        var n = nums1.Length;
        var pos2 = new int[n];
        for (var i = 0; i < n; i++)
        {
            pos2[nums2[i]] = i;
        }

        var ranks = new int[n];
        for (var i = 0; i < n; i++)
        {
            ranks[i] = pos2[nums1[i]];
        }

        return ranks;
    }

    private static int[] ShuffledPermutation(Random random, int length)
    {
        var values = Enumerable.Range(0, length).ToArray();
        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        return values;
    }
}
