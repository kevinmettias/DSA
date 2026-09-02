using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximize Grid Happiness (LC 1659): un-memoized profile-DP recursion over
// (Pos, Mask, Introverts, Extroverts) - re-exploring every distinct prior-mask
// sequence that happens to land on the same state - vs. the identical recursion
// routed through this repo's own Memoizer (MaximumStudentsTakingExamBenchmarks'
// precedent for this exact un-memoized-vs-Memoizer shape, applied to a 4-tuple state
// instead of a 2-tuple one). Columns is fixed and Rows is kept modest (matching
// CherryPickupBenchmarks' "kept modest" reasoning) since the un-memoized side is
// exponential in cell count.
[MemoryDiagnoser]
public class MaximizeGridHappinessBenchmarks
{
    private const int Columns = 3;
    private const int IntrovertsCount = 3;
    private const int ExtrovertsCount = 2;

    // Ternary occupancy code per cell (empty/introvert/extrovert) baked into `mask`.
    private const int MaskBase = 3;
    private const int ExtrovertTypeCode = 2;

    private const int IntrovertBaseGain = 120;
    private const int ExtrovertBaseGain = 40;
    private const int IntrovertAdjacencyDelta = -30;
    private const int ExtrovertAdjacencyDelta = 20;

    // The recursion's own state: which cell to fill next, the trailing ternary
    // occupancy mask, and how many of each person type remain to place.
    private readonly record struct GridState(int Pos, int Mask, int Introverts, int Extroverts);

    [Params(3, 4)]
    public int Rows;

    private int _oldestDigitScale;
    private int _totalCells;

    [GlobalSetup]
    public void Setup()
    {
        _oldestDigitScale = 1;
        for (var i = 0; i < Columns - 1; i++)
        {
            _oldestDigitScale *= MaskBase;
        }

        _totalCells = Rows * Columns;
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => BestFrom(new GridState(0, 0, IntrovertsCount, ExtrovertsCount));

    private int BestFrom(GridState state)
    {
        if (IsTerminalState(state))
        {
            return 0;
        }

        var neighbors = ComputeNeighbors(state);
        var best = SkipCell(state);

        if (state.Introverts > 0)
        {
            var placeIntrovert = PlaceIntrovert(state, neighbors);
            best = Math.Max(best, placeIntrovert);
        }

        if (state.Extroverts > 0)
        {
            var placeExtrovert = PlaceExtrovert(state, neighbors);
            best = Math.Max(best, placeExtrovert);
        }

        return best;
    }

    private int SkipCell(GridState state)
    {
        var nextState = SkipState(state);
        return BestFrom(nextState);
    }

    private int PlaceIntrovert(GridState state, (int Up, int Left) neighbors)
    {
        var nextState = IntrovertState(state);
        return IntrovertGain(neighbors.Up, neighbors.Left) + BestFrom(nextState);
    }

    private int PlaceExtrovert(GridState state, (int Up, int Left) neighbors)
    {
        var nextState = ExtrovertState(state);
        return ExtrovertGain(neighbors.Up, neighbors.Left) + BestFrom(nextState);
    }

    [Benchmark]
    public int MemoizedRecursion()
        => Memoizer.Memoize<GridState, int>(
            new GridState(0, 0, IntrovertsCount, ExtrovertsCount), BestFromMemoized);

    private int BestFromMemoized(GridState state, Func<GridState, int> bestFrom)
    {
        if (IsTerminalState(state))
        {
            return 0;
        }

        var neighbors = ComputeNeighbors(state);
        var best = SkipCellMemoized(state, bestFrom);

        if (state.Introverts > 0)
        {
            var placeIntrovert = PlaceIntrovertMemoized(state, neighbors, bestFrom);
            best = Math.Max(best, placeIntrovert);
        }

        if (state.Extroverts > 0)
        {
            var placeExtrovert = PlaceExtrovertMemoized(state, neighbors, bestFrom);
            best = Math.Max(best, placeExtrovert);
        }

        return best;
    }

    private int SkipCellMemoized(GridState state, Func<GridState, int> bestFrom)
    {
        var nextState = SkipState(state);
        return bestFrom(nextState);
    }

    private int PlaceIntrovertMemoized(GridState state, (int Up, int Left) neighbors, Func<GridState, int> bestFrom)
    {
        var nextState = IntrovertState(state);
        return IntrovertGain(neighbors.Up, neighbors.Left) + bestFrom(nextState);
    }

    private int PlaceExtrovertMemoized(GridState state, (int Up, int Left) neighbors, Func<GridState, int> bestFrom)
    {
        var nextState = ExtrovertState(state);
        return ExtrovertGain(neighbors.Up, neighbors.Left) + bestFrom(nextState);
    }

    private GridState SkipState(GridState state)
        => state with { Pos = state.Pos + 1, Mask = ShiftIn(state.Mask, 0) };

    private GridState IntrovertState(GridState state)
        => state with
        {
            Pos = state.Pos + 1,
            Mask = ShiftIn(state.Mask, 1),
            Introverts = state.Introverts - 1,
        };

    private GridState ExtrovertState(GridState state)
        => state with
        {
            Pos = state.Pos + 1,
            Mask = ShiftIn(state.Mask, ExtrovertTypeCode),
            Extroverts = state.Extroverts - 1,
        };

    private bool IsTerminalState(GridState state)
        => state.Pos == _totalCells || (state.Introverts == 0 && state.Extroverts == 0);

    private (int Up, int Left) ComputeNeighbors(GridState state)
    {
        var row = state.Pos / Columns;
        var col = state.Pos % Columns;
        var up = row > 0 ? state.Mask / _oldestDigitScale : 0;
        var left = col > 0 ? state.Mask % MaskBase : 0;
        return (up, left);
    }

    private static int IntrovertGain(int up, int left)
        => IntrovertBaseGain + NeighborDelta(1, up) + NeighborDelta(1, left);

    private static int ExtrovertGain(int up, int left)
        => ExtrovertBaseGain + NeighborDelta(ExtrovertTypeCode, up) + NeighborDelta(ExtrovertTypeCode, left);

    private int ShiftIn(int mask, int newType) => (mask % _oldestDigitScale) * MaskBase + newType;

    private static int NeighborDelta(int personType, int neighbor)
    {
        if (neighbor == 0)
        {
            return 0;
        }

        var selfDelta = personType == 1 ? IntrovertAdjacencyDelta : ExtrovertAdjacencyDelta;
        var neighborDelta = neighbor == 1 ? IntrovertAdjacencyDelta : ExtrovertAdjacencyDelta;
        return selfDelta + neighborDelta;
    }
}
