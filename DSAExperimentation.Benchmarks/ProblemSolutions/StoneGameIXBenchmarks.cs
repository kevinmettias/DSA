using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.StoneGameIX;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StoneGameIXSolution's, the same methods StoneGameIXTests
// proves correct. A full negamax over the remaining (count0, count1, count2,
// runningSumMod3) state - O(n^3) states, memoized by this repo's own
// Memoizer<TState,TResult> - vs. bucketing stones by value mod 3 and reading the answer
// off a closed-form parity rule in O(n).
//
// [GlobalSetup] builds LeetCode's own input shape (the stones array), so there is no
// hoisted overload to add: the remainder bucketing each arm does is O(n) and dwarfed by
// the cubic state space the game-tree arm explores.
[MemoryDiagnoser]
public class StoneGameIXBenchmarks
{
    private const int WorkloadSeed = 1;
    private const int MaxStoneValueExclusive = 1_000;

    private int[] _stones = [];

    [Params(20, 50)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(WorkloadSeed);
        _stones = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxStoneValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool CanAliceWinByGameTreeMinimax() => StoneGameIXSolution.CanAliceWinByGameTreeMinimax(_stones);

    [Benchmark]
    public bool CanAliceWinByClosedFormCounting() => StoneGameIXSolution.CanAliceWinByClosedFormCounting(_stones);
}
