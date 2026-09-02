using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Absolute Sum Difference (LC 1818): the textbook O(n^2) brute force that
// rescans all of nums1 for every index vs. sorting nums1 once with this repo's own
// MergeSort and then probing it per index with BinarySearch.LowerBound -
// O(n log n) total instead of O(n^2).
[MemoryDiagnoser]
public class MinimumAbsoluteSumDifferenceBenchmarks
{
    private const int MaxValueExclusive = 100_000;
    private const int Mod = 1_000_000_007;

    [Params(200, 2_000)]
    public int Length;

    private int[] _nums1 = null!;
    private int[] _nums2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _nums1 = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _nums2 = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        long baseSum = 0;
        long maxReduction = 0;

        for (var i = 0; i < _nums1.Length; i++)
        {
            var diff = Math.Abs(_nums1[i] - _nums2[i]);
            baseSum += diff;

            var bestDiff = diff;
            for (var j = 0; j < _nums1.Length; j++)
            {
                bestDiff = Math.Min(bestDiff, Math.Abs(_nums1[j] - _nums2[i]));
            }

            maxReduction = Math.Max(maxReduction, diff - bestDiff);
        }

        return (int)((baseSum - maxReduction) % Mod);
    }

    [Benchmark]
    public int SortedBinarySearch()
    {
        var sorted = _nums1.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));
        var sortedSequence = new ArraySequence<int>(sorted);

        long baseSum = 0;
        long maxReduction = 0;

        for (var i = 0; i < _nums1.Length; i++)
        {
            var (diff, bestDiff) = ClosestDiffAt(i, sorted, sortedSequence);
            baseSum += diff;
            maxReduction = Math.Max(maxReduction, diff - bestDiff);
        }

        return (int)((baseSum - maxReduction) % Mod);
    }

    private (int Diff, int BestDiff) ClosestDiffAt(int i, int[] sorted, ArraySequence<int> sortedSequence)
    {
        var diff = Math.Abs(_nums1[i] - _nums2[i]);
        var insertion = BinarySearch.LowerBound<int, ArraySequence<int>>(sortedSequence, _nums2[i]);
        var bestDiff = diff;

        if (insertion < sorted.Length)
        {
            bestDiff = Math.Min(bestDiff, Math.Abs(sorted[insertion] - _nums2[i]));
        }

        if (insertion > 0)
        {
            bestDiff = Math.Min(bestDiff, Math.Abs(sorted[insertion - 1] - _nums2[i]));
        }

        return (diff, bestDiff);
    }
}
