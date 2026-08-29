using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class InterleavingStringBenchmarks
{
    [Benchmark(Baseline = true)] public int Baseline() => 1;
    [Benchmark] public int PrimitiveComposed() => 1;
}
