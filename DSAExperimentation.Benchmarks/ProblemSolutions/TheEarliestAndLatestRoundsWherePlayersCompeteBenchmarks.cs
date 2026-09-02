using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// The Earliest and Latest Rounds Where Players Compete (LC 1900): the textbook
// unmemoized recursion over (roundSize, low, high) - the same tournament-bracket
// state re-explored from scratch down every branch, since many different
// elimination choices among the OTHER players reach an identical (roundSize, low,
// high) state for the two tracked players - vs. the same recursion routed through
// this repo's own Memoizer, the identical tuple-state shape
// CatAndMouseIIBenchmarks/MinimumCostToConnectTwoGroupsOfPointsBenchmarks already
// use for their own minimax/assignment recurrences. See
// TheEarliestAndLatestRoundsWherePlayersCompeteTests for the zone-based derivation
// of the (leftFree, midFree, midFixed) transition this recurrence relies on.
[MemoryDiagnoser]
public class TheEarliestAndLatestRoundsWherePlayersCompeteBenchmarks
{
    private const int FirstPlayer = 2;
    private const int HalvingFactor = 2;
    private const int HighSeedBaseOffset = 2;

    [Params(10, 16)]
    public int N;

    private int _low;
    private int _high;

    [GlobalSetup]
    public void Setup()
    {
        var secondPlayer = N / HalvingFactor;
        _low = Math.Min(FirstPlayer, secondPlayer);
        _high = Math.Max(FirstPlayer, secondPlayer);
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion()
    {
        var (earliest, latest) = Solve(N, _low, _high);
        return earliest + latest;
    }

    private (int Earliest, int Latest) Solve(int roundSize, int l, int h) =>
        Compute(roundSize, l, h, Solve);

    [Benchmark]
    public int MemoizedRecursion()
    {
        var (earliest, latest) = Memoizer.Memoize<(int N, int Low, int High), (int Earliest, int Latest)>(
            (N, _low, _high), Recurrence);
        return earliest + latest;
    }

    private (int Earliest, int Latest) Recurrence(
        (int N, int Low, int High) state,
        Func<(int N, int Low, int High), (int Earliest, int Latest)> solve)
    {
        var (roundSize, l, h) = state;
        return Compute(roundSize, l, h, (rs, lo, hi) => solve((rs, lo, hi)));
    }

    // Shared shape between the unmemoized self-recursion and the memoized
    // recurrence: normalize by mirror symmetry, stop once the two tracked
    // players must already have met, otherwise fan out over every elimination
    // choice for the other survivors via ExploreRound.
    private (int Earliest, int Latest) Compute(
        int roundSize, int l, int h, Func<int, int, int, (int Earliest, int Latest)> solve)
    {
        if (l + h > roundSize + 1)
        {
            return solve(roundSize, roundSize + 1 - h, roundSize + 1 - l);
        }

        if (l + h == roundSize + 1)
        {
            return (1, 1);
        }

        var zones = Transition(roundSize, l, h);
        var nextRoundSize = (roundSize + 1) / HalvingFactor;
        return ExploreRound(nextRoundSize, zones, solve);
    }

    private static (int Earliest, int Latest) ExploreRound(
        int nextRoundSize,
        (int LeftFree, int MidFree, int MidFixed) zones,
        Func<int, int, int, (int Earliest, int Latest)> solve)
    {
        var earliest = int.MaxValue;
        var latest = int.MinValue;

        for (var x = 0; x <= zones.LeftFree; x++)
        {
            for (var y = 0; y <= zones.MidFree; y++)
            {
                var (e, la) = solve(nextRoundSize, 1 + x, HighSeedBaseOffset + x + y + zones.MidFixed);
                earliest = Math.Min(earliest, e + 1);
                latest = Math.Max(latest, la + 1);
            }
        }

        return (earliest, latest);
    }

    // Shared zone-counting transition (see the coverage test for the full
    // derivation): how many "other" survivors can freely land below low (leftFree)
    // and below high (midFree, plus midFixed forced ones from Mid-Mid pairs / the
    // odd-n bye) after one round.
    private static (int LeftFree, int MidFree, int MidFixed) Transition(int roundSize, int l, int h)
    {
        var leftFree = l - 1;
        var half = roundSize / HalvingFactor;
        var isOdd = roundSize % HalvingFactor == 1;
        var middle = (roundSize + 1) / HalvingFactor;

        if (isOdd && h == middle)
        {
            return (leftFree, half - l, 0);
        }

        var partner = Math.Min(h, roundSize + 1 - h);
        if (partner == h)
        {
            return (leftFree, h - l - 1, 0);
        }

        var midFree = partner - l - 1;
        var pairableInnerRegion = h - 1 - partner - (isOdd ? 1 : 0);
        var midFixed = (pairableInnerRegion / HalvingFactor) + (isOdd ? 1 : 0);
        return (leftFree, midFree, midFixed);
    }
}
