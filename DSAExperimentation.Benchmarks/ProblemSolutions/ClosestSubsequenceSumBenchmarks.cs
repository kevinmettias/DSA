using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Closest Subsequence Sum (LC 1755): a full O(2^n) direct subset-sum scan of
// the whole array vs. meet-in-the-middle - Backtrack.Search enumerating each
// half's O(2^(n/2)) subset sums separately (the Subsets precedent), MergeSort
// ordering one half's sums over an ArrayIndexedSequence, and
// BinarySearch.LowerBound locating the other half's closest partner sum.
// Neither strategy short-circuits on an exact match, so both always scan to
// completion regardless of Goal.
[MemoryDiagnoser]
public class ClosestSubsequenceSumBenchmarks
{
    private const int Goal = 1_000_000;
    private const int RandomSeed = 1755; // LC problem number
    private const int ValueMagnitudeBound = 50;
    private const int MidpointDivisor = 2;

    [Params(16, 20)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueMagnitudeBound, ValueMagnitudeBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceAllSubsets()
    {
        var best = int.MaxValue;

        for (var mask = 0; mask < (1 << _nums.Length); mask++)
        {
            var sum = 0;

            for (var i = 0; i < _nums.Length; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    sum += _nums[i];
                }
            }

            best = Math.Min(best, Math.Abs(sum - Goal));
        }

        return best;
    }

    [Benchmark]
    public int MeetInTheMiddle()
    {
        var (leftSums, rightSums) = SplitIntoSubsetSums(_nums);
        var sequence = SortAndWrap(rightSums);

        return FindClosestSumToGoal(leftSums, rightSums, sequence);
    }

    private static (List<int> LeftSums, int[] RightSums) SplitIntoSubsetSums(int[] nums)
    {
        var mid = nums.Length / MidpointDivisor;
        var leftSums = SubsetSums(nums[..mid]);
        var rightSums = SubsetSums(nums[mid..]).ToArray();

        return (leftSums, rightSums);
    }

    private static ArraySequence<int> SortAndWrap(int[] rightSums)
    {
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(rightSums));
        return new ArraySequence<int>(rightSums);
    }

    private static int FindClosestSumToGoal(List<int> leftSums, int[] rightSums, ArraySequence<int> sequence)
    {
        var best = int.MaxValue;

        foreach (var leftSum in leftSums)
        {
            var index = BinarySearch.LowerBound<int, ArraySequence<int>>(sequence, Goal - leftSum);

            if (index < rightSums.Length)
            {
                best = Math.Min(best, Math.Abs(leftSum + rightSums[index] - Goal));
            }

            if (index > 0)
            {
                best = Math.Min(best, Math.Abs(leftSum + rightSums[index - 1] - Goal));
            }
        }

        return best;
    }

    private static List<int> SubsetSums(int[] part)
    {
        var sums = new List<int>();
        var state = new SumState();

        Backtrack.Search<SumState, int>(
            state,
            isSolution: s => s.NextIndex == part.Length,
            candidates: s => s.NextIndex < part.Length ? new[] { 0, 1 } : Array.Empty<int>(),
            choose: (s, take) => ChooseSubsetElement(s, take, part),
            unchoose: (s, take) => UnchooseSubsetElement(s, take, part),
            onSolution: s => sums.Add(s.Sum));

        return sums;
    }

    private static void ChooseSubsetElement(SumState state, int take, int[] part)
    {
        if (take == 1)
        {
            state.Sum += part[state.NextIndex];
        }

        state.NextIndex++;
    }

    private static void UnchooseSubsetElement(SumState state, int take, int[] part)
    {
        state.NextIndex--;

        if (take == 1)
        {
            state.Sum -= part[state.NextIndex];
        }
    }

    private sealed class SumState
    {
        public int Sum { get; set; }
        public int NextIndex { get; set; }
    }
}
