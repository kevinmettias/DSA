using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// 3Sum With Multiplicity (LC 923): cubic triple-loop counting (baseline, the
// textbook approach) vs. MergeSort plus the sorted two-pointer sweep that counts
// each equal-valued span's combinations directly instead of visiting one pair at
// a time - the same MergeSort/ArrayIndexedSequence primitives ThreeSumBenchmarks
// already uses, extended with multiplicity counting under a modulus.
[MemoryDiagnoser]
public class ThreeSumWithMultiplicityBenchmarks
{
    private const int Modulo = 1_000_000_007;
    private const int Target = 150;
    private const int RandomSeed = 923; // LC 923
    private const int ValueUpperBoundExclusive = 101; // values drawn from [0, 100] per LC 923's constraint
    private const int TripleWindowMargin = 2; // reserve room for the 2 remaining indices in a triple
    private const int ChooseTwoDivisor = 2; // binomial "n choose 2" = n * (n - 1) / 2

    [Params(80, 300)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        // Values bounded to [0, 100], matching LC 923's own constraint - this small
        // range is what forces genuine multiplicities (repeated values), which is
        // the whole point of this problem versus plain 3Sum.
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, ValueUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        long count = 0;

        for (var i = 0; i < _values.Length - TripleWindowMargin; i++)
        {
            for (var j = i + 1; j < _values.Length - 1; j++)
            {
                count += CountTripletsWithThirdIndex(i, j);
            }
        }

        return (int)(count % Modulo);
    }

    private long CountTripletsWithThirdIndex(int i, int j)
    {
        long count = 0;

        for (var k = j + 1; k < _values.Length; k++)
        {
            if (_values[i] + _values[j] + _values[k] == Target)
            {
                count++;
            }
        }

        return count;
    }

    [Benchmark]
    public int MergeSortTwoPointers()
    {
        var sorted = _values.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        long count = 0;

        for (var i = 0; i < sorted.Length - TripleWindowMargin; i++)
        {
            count = ScanPairsForFixedIndex(sorted, i, count);
        }

        return (int)count;
    }

    private long ScanPairsForFixedIndex(int[] sorted, int i, long count)
    {
        var remaining = Target - sorted[i];
        var state = new PairScanState(count, i + 1, sorted.Length - 1);

        while (state.Left < state.Right)
        {
            var result = AdvanceTwoPointers(sorted, remaining, state);
            state = result.State;

            if (result.Done)
            {
                break;
            }
        }

        return state.Count;
    }

    private readonly record struct PairScanState(long Count, int Left, int Right);

    private readonly record struct PairScanResult(PairScanState State, bool Done);

    private PairScanResult AdvanceTwoPointers(int[] sorted, int remaining, PairScanState state)
    {
        var pairSum = sorted[state.Left] + sorted[state.Right];

        if (pairSum < remaining)
        {
            return new PairScanResult(state with { Left = state.Left + 1 }, false);
        }

        if (pairSum > remaining)
        {
            return new PairScanResult(state with { Right = state.Right - 1 }, false);
        }

        if (sorted[state.Left] != sorted[state.Right])
        {
            var (count, left, right) = CountDistinctPairSpan(sorted, state.Count, state.Left, state.Right);
            return new PairScanResult(new PairScanState(count, left, right), false);
        }

        var span = state.Right - state.Left + 1;
        var finalCount = (state.Count + ((long)span * (span - 1) / ChooseTwoDivisor)) % Modulo;
        return new PairScanResult(state with { Count = finalCount }, true);
    }

    private static (long Count, int Left, int Right) CountDistinctPairSpan(int[] sorted, long count, int left, int right)
    {
        var leftCount = 1;
        while (left + 1 < right && sorted[left + 1] == sorted[left])
        {
            leftCount++;
            left++;
        }

        var rightCount = 1;
        while (right - 1 > left && sorted[right - 1] == sorted[right])
        {
            rightCount++;
            right--;
        }

        count = (count + ((long)leftCount * rightCount)) % Modulo;
        left++;
        right--;

        return (count, left, right);
    }
}
