using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ClosestDivisors;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ClosestDivisorsSolution's, the same methods
// ClosestDivisorsTests proves correct - the textbook O(candidate) full-range divisor
// scan against BinarySearch.LowerBound anchoring at floor(sqrt(candidate)) (the same
// technique SqrtXBenchmarks uses for LC 69) followed by a short walk down to the
// first exact divisor.
[MemoryDiagnoser]
public class ClosestDivisorsBenchmarks
{
    [Params(1_000, 100_000)]
    public int Number { get; set; }

    [Benchmark(Baseline = true)]
    public (int First, int Second) BruteForce() => ClosestDivisorsSolution.ClosestPairByDivisorScan(Number);

    [Benchmark]
    public (int First, int Second) BinarySearchAnchored() =>
        ClosestDivisorsSolution.ClosestPairByBinarySearchAnchor(Number);
}
