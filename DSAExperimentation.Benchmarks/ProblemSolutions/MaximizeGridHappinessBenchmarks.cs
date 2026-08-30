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
            _oldestDigitScale *= 3;
        }

        _totalCells = Rows * Columns;
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => BestFrom(0, 0, IntrovertsCount, ExtrovertsCount);

    private int BestFrom(int pos, int mask, int introverts, int extroverts)
    {
        if (pos == _totalCells || (introverts == 0 && extroverts == 0))
        {
            return 0;
        }

        var row = pos / Columns;
        var col = pos % Columns;
        var up = row > 0 ? mask / _oldestDigitScale : 0;
        var left = col > 0 ? mask % 3 : 0;

        var best = BestFrom(pos + 1, ShiftIn(mask, 0), introverts, extroverts);

        if (introverts > 0)
        {
            var gain = 120 + NeighborDelta(1, up) + NeighborDelta(1, left);
            best = Math.Max(best, gain + BestFrom(pos + 1, ShiftIn(mask, 1), introverts - 1, extroverts));
        }

        if (extroverts > 0)
        {
            var gain = 40 + NeighborDelta(2, up) + NeighborDelta(2, left);
            best = Math.Max(best, gain + BestFrom(pos + 1, ShiftIn(mask, 2), introverts, extroverts - 1));
        }

        return best;
    }

    [Benchmark]
    public int MemoizedRecursion()
        => Memoizer.Memoize<(int Pos, int Mask, int Introverts, int Extroverts), int>(
            (0, 0, IntrovertsCount, ExtrovertsCount), BestFromMemoized);

    private int BestFromMemoized(
        (int Pos, int Mask, int Introverts, int Extroverts) state,
        Func<(int Pos, int Mask, int Introverts, int Extroverts), int> bestFrom)
    {
        var (pos, mask, introverts, extroverts) = state;
        if (pos == _totalCells || (introverts == 0 && extroverts == 0))
        {
            return 0;
        }

        var row = pos / Columns;
        var col = pos % Columns;
        var up = row > 0 ? mask / _oldestDigitScale : 0;
        var left = col > 0 ? mask % 3 : 0;

        var best = bestFrom((pos + 1, ShiftIn(mask, 0), introverts, extroverts));

        if (introverts > 0)
        {
            var gain = 120 + NeighborDelta(1, up) + NeighborDelta(1, left);
            best = Math.Max(best, gain + bestFrom((pos + 1, ShiftIn(mask, 1), introverts - 1, extroverts)));
        }

        if (extroverts > 0)
        {
            var gain = 40 + NeighborDelta(2, up) + NeighborDelta(2, left);
            best = Math.Max(best, gain + bestFrom((pos + 1, ShiftIn(mask, 2), introverts, extroverts - 1)));
        }

        return best;
    }

    private int ShiftIn(int mask, int newType) => (mask % _oldestDigitScale) * 3 + newType;

    private static int NeighborDelta(int personType, int neighbor)
    {
        if (neighbor == 0)
        {
            return 0;
        }

        var selfDelta = personType == 1 ? -30 : 20;
        var neighborDelta = neighbor == 1 ? -30 : 20;
        return selfDelta + neighborDelta;
    }
}
