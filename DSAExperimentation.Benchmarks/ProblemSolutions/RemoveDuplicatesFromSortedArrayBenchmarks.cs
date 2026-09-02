using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RemoveDuplicatesFromSortedArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RemoveDuplicatesFromSortedArraySolution's, the same
// methods RemoveDuplicatesFromSortedArrayTests proves correct. Both strategies
// mutate the array they are handed, so each call gets its own copy of _values
// rather than reusing one shared array a later iteration would find already
// compacted.
[MemoryDiagnoser]
public class RemoveDuplicatesFromSortedArrayBenchmarks
{
    private const int DuplicateRunLength = 3;

    private int[] _values = null!;

    [Params(200, 5_000)]
    public int Length;

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, Length).Select(i => i / DuplicateRunLength).ToArray();

    [Benchmark(Baseline = true)]
    public int LinqDistinct() =>
        RemoveDuplicatesFromSortedArraySolution.RemoveDuplicatesByLinqDistinct(_values.ToArray());

    [Benchmark]
    public int ArrayIndexedSequenceCompact() =>
        RemoveDuplicatesFromSortedArraySolution.RemoveDuplicatesByArrayIndexedSequenceCompact(_values.ToArray());
}
