using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Partition Array Into Two Arrays to Minimize Sum Difference (LC 2035): a direct
// O(2^(2n)) scan of every size-n subset (bitmask popcount filter over the whole
// 2n-element array - the same brute-force shape ClosestSubsequenceSumBenchmarks
// uses for LC 1755) vs meet-in-the-middle - Backtrack.Search enumerating each
// n-element half's O(2^n) subset sums grouped by subset size, MergeSort ordering
// each size-group over an ArrayIndexedSequence, and BinarySearch.LowerBound
// locating the closest size-complementary partner sum.
[MemoryDiagnoser]
public class PartitionArrayIntoTwoArraysToMinimizeSumDifferenceBenchmarks
{
    private const int RandomSeed = 2035;
    private const int ValueBound = 50;
    private const int PartitionRatio = 2;
    private const int SumDifferenceScale = 2;

    [Params(16, 20)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueBound, ValueBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceAllEqualSplits()
    {
        var n = _nums.Length / PartitionRatio;
        var total = _nums.Sum();
        var best = int.MaxValue;

        for (var mask = 0; mask < (1 << _nums.Length); mask++)
        {
            var candidateDifference = EqualSplitDifference(mask, n, total);
            best = Math.Min(best, candidateDifference);
        }

        return best;
    }

    private int EqualSplitDifference(int mask, int n, int total)
    {
        var count = 0;
        var sum = 0;

        for (var i = 0; i < _nums.Length; i++)
        {
            if ((mask & (1 << i)) != 0)
            {
                count++;
                sum += _nums[i];
            }
        }

        if (count != n)
        {
            return int.MaxValue;
        }

        return Math.Abs((SumDifferenceScale * sum) - total);
    }

    [Benchmark]
    public int MeetInTheMiddleGroupedBySize()
    {
        var n = _nums.Length / PartitionRatio;
        var total = _nums.Sum();
        var groups = new SumGroups(SubsetSumsByCount(_nums[..n], n), SubsetSumsByCount(_nums[n..], n));

        var best = int.MaxValue;

        for (var k = 0; k <= n; k++)
        {
            var groupDifference = BestForGroupSize(k, n, groups, total);
            best = Math.Min(best, groupDifference);
        }

        return best;
    }

    private static int BestForGroupSize(int k, int n, SumGroups groups, int total)
    {
        var rightSums = groups.Right[n - k].ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(rightSums));
        var sequence = new ArraySequence<int>(rightSums);
        var doubledComparer = Comparer<int>.Create(
            (candidate, target) => (SumDifferenceScale * candidate).CompareTo(target));
        var rightGroup = new RightGroup(rightSums, sequence, doubledComparer);
        var best = int.MaxValue;

        foreach (var leftSum in groups.Left[k])
        {
            var candidateDifference = ClosestComplementDifference(leftSum, rightGroup, total);
            best = Math.Min(best, candidateDifference);
        }

        return best;
    }

    private static int ClosestComplementDifference(int leftSum, RightGroup rightGroup, int total)
    {
        var target = total - (SumDifferenceScale * leftSum);
        var index = BinarySearch.LowerBound<int, ArraySequence<int>>(
            rightGroup.Sequence, target, rightGroup.DoubledComparer);
        var best = int.MaxValue;

        if (index < rightGroup.Values.Length)
        {
            best = Math.Min(best, Math.Abs((SumDifferenceScale * (leftSum + rightGroup.Values[index])) - total));
        }

        if (index > 0)
        {
            best = Math.Min(best, Math.Abs((SumDifferenceScale * (leftSum + rightGroup.Values[index - 1])) - total));
        }

        return best;
    }

    private readonly record struct SumGroups(List<int>[] Left, List<int>[] Right);

    private readonly record struct RightGroup(int[] Values, ArraySequence<int> Sequence, IComparer<int> DoubledComparer);

    private static List<int>[] SubsetSumsByCount(int[] part, int n)
    {
        var sumsByCount = CreateEmptyGroups(n);
        var state = new SumState();

        Backtrack.Search<SumState, int>(
            state,
            isSolution: s => s.NextIndex == part.Length,
            candidates: s => s.NextIndex < part.Length ? new[] { 0, 1 } : Array.Empty<int>(),
            choose: (s, take) => Choose(s, take, part),
            unchoose: (s, take) => Unchoose(s, take, part),
            onSolution: s => sumsByCount[s.Count].Add(s.Sum));

        return sumsByCount;
    }

    private static List<int>[] CreateEmptyGroups(int n)
    {
        var groups = new List<int>[n + 1];
        for (var i = 0; i <= n; i++)
        {
            groups[i] = [];
        }

        return groups;
    }

    private static void Choose(SumState state, int take, int[] part)
    {
        if (take == 1)
        {
            state.Sum += part[state.NextIndex];
            state.Count++;
        }

        state.NextIndex++;
    }

    private static void Unchoose(SumState state, int take, int[] part)
    {
        state.NextIndex--;

        if (take == 1)
        {
            state.Sum -= part[state.NextIndex];
            state.Count--;
        }
    }

    private sealed class SumState
    {
        public int Sum { get; set; }
        public int Count { get; set; }
        public int NextIndex { get; set; }
    }
}
