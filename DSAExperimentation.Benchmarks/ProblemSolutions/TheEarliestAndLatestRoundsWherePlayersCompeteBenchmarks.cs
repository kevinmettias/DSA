using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.TheEarliestAndLatestRoundsWherePlayersCompete;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// TheEarliestAndLatestRoundsWherePlayersCompeteSolution's, the same methods
// TheEarliestAndLatestRoundsWherePlayersCompeteTests proves correct - the textbook
// unmemoized recursion, which re-explores an identical (roundSize, low, high)
// state down every elimination branch that reaches it, against the same recurrence
// routed through this repo's own Memoizer. The tracked players are LeetCode's own
// scalar input shape, so neither arm needs a hoisted overload; [GlobalSetup] only
// picks the second player for the bracket size.
[MemoryDiagnoser]
public class TheEarliestAndLatestRoundsWherePlayersCompeteBenchmarks
{
    private const int FirstPlayer = 2;

    private int _secondPlayer;

    [Params(10, 16)]
    public int PlayerCount { get; set; }

    [GlobalSetup]
    public void Setup() => _secondPlayer = PlayerCount / AlgorithmConstants.HalvingFactor;

    [Benchmark(Baseline = true)]
    public (int Earliest, int Latest) UnmemoizedRecursion() =>
        TheEarliestAndLatestRoundsWherePlayersCompeteSolution.EarliestAndLatestByPlainRecursion(
            PlayerCount, FirstPlayer, _secondPlayer);

    [Benchmark]
    public (int Earliest, int Latest) MemoizedRecursion() =>
        TheEarliestAndLatestRoundsWherePlayersCompeteSolution.EarliestAndLatestByMemoizedRecurrence(
            PlayerCount, FirstPlayer, _secondPlayer);
}
