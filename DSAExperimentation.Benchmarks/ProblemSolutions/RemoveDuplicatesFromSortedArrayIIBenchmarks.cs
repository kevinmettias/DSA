using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RemoveDuplicatesFromSortedArrayII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveDuplicatesFromSortedArrayIISolution's, the
// same methods RemoveDuplicatesFromSortedArrayIITests proves correct. Both
// strategies mutate the array they are handed, so each call gets its own copy of
// _values rather than reusing one shared array a later iteration would find
// already compacted.
[MemoryDiagnoser]
public class RemoveDuplicatesFromSortedArrayIIBenchmarks
{
    private const int DuplicateRunLength = 3; // every N consecutive elements share the same value in the seeded input

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, Length).Select(i => i / DuplicateRunLength).ToArray();

    [Benchmark(Baseline = true)]
    public int LinqGroupCapTwo() =>
        RemoveDuplicatesFromSortedArrayIISolution.RemoveDuplicatesByLinqGroupCapTwo(_values.ToArray());

    [Benchmark]
    public int ArrayIndexedSequenceCompact() =>
        RemoveDuplicatesFromSortedArrayIISolution.RemoveDuplicatesByArrayIndexedSequenceCompact(_values.ToArray());
}
