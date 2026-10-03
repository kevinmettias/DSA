using DSAExperimentation.LeetCode.RemoveDuplicatesFromSortedArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveDuplicatesFromSortedArraySolution's, the same
// methods RemoveDuplicatesFromSortedArraySolutionTests proves correct. Both strategies
// compact the array they are handed in place, so each call first copies _values into
// a buffer [GlobalSetup] allocated once, rather than handing over an array a later
// iteration would find already compacted. The copy is timed on purpose because the
// strategies mutate their input, and every arm pays the same cost.
//
// The values climb from -100 in runs of equal values, at least DuplicateRunLength long
// and longer where Length needs it: LC 26 holds every value in [-100, 100], 201 of them,
// so a 5,000-long array repeats each about 25 times.
public class RemoveDuplicatesFromSortedArrayBenchmarks
{
    private const int DuplicateRunLength = 3;
    private const int MinValue = -100;
    private const int DistinctValueCount = 201;

    private int[] _values = [];
    private int[] _nums = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var fittingRunLength = RunLengthToFit(Length);
        var runLength = Math.Max(DuplicateRunLength, fittingRunLength);
        _values = Enumerable.Range(0, Length).Select(i => MinValue + (i / runLength)).ToArray();
        _nums = new int[Length];
    }

    // The shortest run that spreads length values over DistinctValueCount of them.
    private static int RunLengthToFit(int length) => (length + DistinctValueCount - 1) / DistinctValueCount;

    [Benchmark(Baseline = true)]
    public int LinqDistinct()
    {
        _values.CopyTo(_nums, 0);
        return RemoveDuplicatesFromSortedArraySolution.RemoveDuplicatesByLinqDistinct(_nums);
    }

    [Benchmark]
    public int ArrayIndexedSequenceCompact()
    {
        _values.CopyTo(_nums, 0);
        return RemoveDuplicatesFromSortedArraySolution.RemoveDuplicatesByArrayIndexedSequenceCompact(_nums);
    }
}
