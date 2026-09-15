using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MirrorReflection;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MirrorReflectionSolution's, the same methods
// MirrorReflectionTests proves correct - naive O(p) step-by-step unfolding against
// the O(log(min(p, q))) Euclidean-GCD closed form. Q = P - 1 keeps every pair
// coprime (consecutive integers always are), forcing the simulation through its
// full O(p) worst case instead of an early exit at a small common factor. The
// inputs are two ints, so there is nothing to hoist into a [GlobalSetup].
[MemoryDiagnoser]
public class MirrorReflectionBenchmarks
{
    private int Q => P - 1;

    [Params(50_000, 500_000)]
    public int P { get; set; }

    [Benchmark(Baseline = true)]
    public int SimulatedUnfolding() =>
        MirrorReflectionSolution.ReceptorBySimulatedUnfolding(P, Q);

    [Benchmark]
    public int GcdClosedForm() =>
        MirrorReflectionSolution.ReceptorByGcdReduction(P, Q);
}
