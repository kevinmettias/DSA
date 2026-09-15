using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumPossibleMaximumWaitingTime;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumPossibleMaximumWaitingTimeSolution's,
// the same methods MinimumPossibleMaximumWaitingTimeTests proves correct.
// Fuel is fixed at the LC 4009 maximum (50, 50) so most cars find both
// dispensers still eligible, keeping the unmemoized search's branching
// factor close to 2 per car instead of collapsing into forced single
// choices - the case memoization actually earns its keep on.
[MemoryDiagnoser]
public class MinimumPossibleMaximumWaitingTimeBenchmarks
{
    private const int MaxDemandExclusive = 6;
    private const int Seed = 4009;
    private static readonly int[] Fuel = [50, 50];

    private int[] _demand = [];

    [Params(10, 18)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _demand = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxDemandExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int RecursiveSearch() =>
        MinimumPossibleMaximumWaitingTimeSolution.MinWaitByRecursiveSearch(_demand, Fuel);

    [Benchmark]
    public int MemoizedSearch() =>
        MinimumPossibleMaximumWaitingTimeSolution.MinWaitByMemoizedSearch(_demand, Fuel);
}
