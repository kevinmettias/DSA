using DSAExperimentation.LeetCode.RemoveDuplicatesFromSortedArrayII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveDuplicatesFromSortedArrayIISolution's, the
// same methods RemoveDuplicatesFromSortedArrayIISolutionTests proves correct. Both
// strategies compact the array they are handed in place, so each call first copies
// _values into a buffer [GlobalSetup] allocated once, rather than handing over an
// array a later iteration would find already compacted. The copy is timed on purpose
// because the strategies mutate their input, and every arm pays the same cost.
public class RemoveDuplicatesFromSortedArrayIIBenchmarks
{
    private const int DuplicateRunLength = 3; // every N consecutive elements share the same value in the seeded input

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
    public int LinqGroupCapTwo()
    {
        _values.CopyTo(_nums, 0);
        return RemoveDuplicatesFromSortedArrayIISolution.RemoveDuplicatesByLinqGroupCapTwo(_nums);
    }

    [Benchmark]
    public int ArrayIndexedSequenceCompact()
    {
        _values.CopyTo(_nums, 0);
        return RemoveDuplicatesFromSortedArrayIISolution.RemoveDuplicatesByArrayIndexedSequenceCompact(_nums);
    }
}
