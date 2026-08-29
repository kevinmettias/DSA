using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class SearchInRotatedSortedArrayIIBenchmarks
{
    [Benchmark(Baseline = true)] public int Baseline() => 1;
    [Benchmark] public int PrimitiveComposed() => 1;
}
