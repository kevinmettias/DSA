using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Remove Duplicates from Sorted Array II (LC 80): a LINQ GroupBy-and-cap read-only
// baseline (same "convenient LINQ vs. allocation-free primitive" contrast
// RemoveDuplicatesFromSortedArrayBenchmarks already uses for LC 26) vs. the two-
// pointer in-place compaction over this repo's own ArrayIndexedSequence<int>.
[MemoryDiagnoser]
public class RemoveDuplicatesFromSortedArrayIIBenchmarks
{
    private int[] _values = null!;

    [Params(200, 5_000)]
    public int Length;

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, Length).Select(i => i / 3).ToArray();

    [Benchmark(Baseline = true)]
    public int LinqGroupCapTwo() => _values.GroupBy(x => x).Sum(g => Math.Min(2, g.Count()));

    [Benchmark]
    public int ArrayIndexedSequenceCompact()
    {
        var copy = _values.ToArray();
        var sequence = new ArrayIndexedSequence<int>(copy);
        var write = 0;

        for (var read = 0; read < sequence.Length; read++)
        {
            if (write < 2 || sequence.Get(read) != sequence.Get(write - 2))
            {
                sequence.Set(write++, sequence.Get(read));
            }
        }

        return write;
    }
}
