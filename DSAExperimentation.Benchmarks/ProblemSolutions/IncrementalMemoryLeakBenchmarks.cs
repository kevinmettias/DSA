using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.IncrementalMemoryLeak;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are IncrementalMemoryLeakSolution's, the same methods
// IncrementalMemoryLeakTests proves correct. The two sticks start equal, so the
// heap arm pays its tie-break on every single round - the worst case for routing
// this allocation through a priority structure rather than an if/else.
[MemoryDiagnoser]
public class IncrementalMemoryLeakBenchmarks
{
    [Params(1_000_000, 100_000_000)]
    public int Capacity;

    [Benchmark(Baseline = true)]
    public int[] Arithmetic() => IncrementalMemoryLeakSolution.MemoryLeakByArithmetic(Capacity, Capacity);

    [Benchmark]
    public int[] HeapSimulation() => IncrementalMemoryLeakSolution.MemoryLeakByMaxHeap(Capacity, Capacity);
}
