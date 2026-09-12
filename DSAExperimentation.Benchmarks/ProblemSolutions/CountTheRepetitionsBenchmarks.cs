using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountTheRepetitions;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountTheRepetitionsSolution's. NaiveFullSimulation is
// the O(N1 * |S1|) full walk that times out on LeetCode's real n1 <= 10^6
// constraint; HashMapCycleDetection's cost stays flat as N1 grows because a
// repeated "position within s2" state is guaranteed within |S2| + 1 copies of s1 by
// pigeonhole.
[MemoryDiagnoser]
public class CountTheRepetitionsBenchmarks
{
    private const string S1BuildingBlock = "ab";
    private const int S1BuildingBlockRepeatCount = 25;
    private const string S2Value = "ba";
    private const int N2 = 1;

    [Params(5_000, 100_000)]
    public int N1;

    private string _s1 = null!;

    [GlobalSetup]
    public void Setup() =>
        _s1 = string.Concat(Enumerable.Repeat(S1BuildingBlock, S1BuildingBlockRepeatCount));

    [Benchmark(Baseline = true)]
    public int NaiveFullSimulation() =>
        CountTheRepetitionsSolution.GetMaxRepetitionsByNaiveSimulation(_s1, N1, S2Value, N2);

    [Benchmark]
    public int HashMapCycleDetection() =>
        CountTheRepetitionsSolution.GetMaxRepetitionsByHashMapCycleDetection(_s1, N1, S2Value, N2);
}
