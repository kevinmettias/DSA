using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class RemoveDuplicatesFromSortedArrayBenchmarks
{
    private int[] _values = null!;
    [Params(200, 5_000)] public int Length;
    [GlobalSetup] public void Setup() => _values = Enumerable.Range(0, Length).Select(i => i / 3).ToArray();
    [Benchmark(Baseline = true)] public int LinqDistinct() => _values.Distinct().Count();
    [Benchmark] public int ArrayIndexedSequenceCompact() { var copy = _values.ToArray(); var sequence = new ArrayIndexedSequence<int>(copy); var write = 1; for (var read = 1; read < sequence.Length; read++) if (sequence.Get(read) != sequence.Get(write - 1)) sequence.Set(write++, sequence.Get(read)); return write; }
}
