using DSAExperimentation.LeetCode.RemoveDuplicatesFromSortedArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveDuplicatesFromSortedArraySolution's, the same
// methods RemoveDuplicatesFromSortedArraySolutionTests proves correct. Both strategies
// compact the array they are handed in place, so each call first copies _values into
// a buffer [GlobalSetup] allocated once, rather than handing over an array a later
// iteration would find already compacted. The copy is timed on purpose because the
// strategies mutate their input, and every arm pays the same cost.
public class RemoveDuplicatesFromSortedArrayBenchmarks
{
    private const int DuplicateRunLength = 3;

    private int[] _values = [];
    private int[] _nums = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _values = Enumerable.Range(0, Length).Select(i => i / DuplicateRunLength).ToArray();
        _nums = new int[Length];
    }

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
