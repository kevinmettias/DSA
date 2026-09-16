using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MirrorReflection;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MirrorReflectionSolution's, the same methods
// MirrorReflectionTests proves correct - naive O(roomSide) step-by-step unfolding
// against the O(log(min(roomSide, rayHeight))) Euclidean-GCD closed form.
// LaserHeight = RoomSide - 1 keeps every pair coprime (consecutive integers always
// are), forcing the simulation through its full O(roomSide) worst case instead of an
// early exit at a small common factor. The inputs are two ints, so there is nothing
// to hoist into a [GlobalSetup].
[MemoryDiagnoser]
public class MirrorReflectionBenchmarks
{
    private int LaserHeight => RoomSide - 1;

    [Params(50_000, 500_000)]
    public int RoomSide { get; set; }

    [Benchmark(Baseline = true)]
    public int SimulatedUnfolding() =>
        MirrorReflectionSolution.ReceptorBySimulatedUnfolding(RoomSide, LaserHeight);

    [Benchmark]
    public int GcdClosedForm() =>
        MirrorReflectionSolution.ReceptorByGcdReduction(RoomSide, LaserHeight);
}
