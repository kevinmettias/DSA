using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheKthCharacterInStringGameI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheKthCharacterInStringGameISolution's, the
// same methods FindTheKthCharacterInStringGameITests proves correct. k is
// capped at #3304's own published bound (500), where the simulation arm is
// already cheap - BitCount is measured to show the O(log k) closed form's
// payoff, not to rescue an arm that would otherwise be infeasible.
[MemoryDiagnoser]
public class FindTheKthCharacterInStringGameIBenchmarks
{
    [Params(10, 500)]
    public int K;

    [Benchmark(Baseline = true)]
    public char Simulation() => FindTheKthCharacterInStringGameISolution.KthCharacterBySimulation(K);

    [Benchmark]
    public char BitCount() => FindTheKthCharacterInStringGameISolution.KthCharacterByBitCount(K);
}
