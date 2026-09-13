using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MaximizeGridHappiness;

// LeetCode 1659. Maximize Grid Happiness: seat up to introvertsCount introverts and
// extrovertsCount extroverts (some may be left out) in an m x n grid to maximize
// total happiness. An introvert starts at 120 and an extrovert at 40; every
// orthogonally adjacent pair costs the introverts in it 30 each and pays the
// extroverts in it 20 each.
//
// Filling cells in row-major order means the only neighbors that can already be
// decided are the cell above and the cell to the left, both of which live in a
// sliding window of the last n placements - so the state is (Pos, Mask, Introverts,
// Extroverts), with GridLayout owning the base-3 window encoding.
//
// Both strategies run that identical recursion; they differ only in whether a state
// reached by several different placement sequences is recomputed each time or once.
internal static class MaximizeGridHappinessSolution
{
    // The occupancy digit each cell contributes to the profile window.
    private const int EmptyTypeCode = 0;
    private const int IntrovertTypeCode = 1;
    private const int ExtrovertTypeCode = 2;

    // LC 1659's own happiness table: what a person is worth alone, and what each
    // orthogonal neighbor does to them.
    private const int IntrovertBaseHappiness = 120;
    private const int ExtrovertBaseHappiness = 40;
    private const int IntrovertAdjacencyDelta = -30;
    private const int ExtrovertAdjacencyDelta = 20;

    // The recursion's own state: which cell to fill next, the trailing occupancy
    // profile, and how many of each person type are left to place.
    private readonly record struct GridState(int Pos, int Mask, int Introverts, int Extroverts);

    // The textbook baseline: the same recursion with no memoization at all, so the
    // identical state is re-explored once per earlier placement sequence that
    // happens to land on it. Deliberately plain recursion over ints - the arm the
    // memoized strategy below has to justify itself against.
    public static int GetMaxGridHappinessByBruteForceRecursion(
        int m, int n, int introvertsCount, int extrovertsCount) =>
        GetMaxGridHappinessByBruteForceRecursion(GridLayout.Build(m, n), introvertsCount, extrovertsCount);

    public static int GetMaxGridHappinessByBruteForceRecursion(
        GridLayout layout, int introvertsCount, int extrovertsCount) =>
        BestFromBruteForce(layout, new GridState(0, 0, introvertsCount, extrovertsCount));

    private static int BestFromBruteForce(GridLayout layout, GridState state)
    {
        if (IsTerminal(layout, state))
        {
            return 0;
        }

        var neighbors = layout.Neighbors(state.Pos, state.Mask);
        var best = BestFromBruteForce(layout, SkipCell(layout, state));

        if (state.Introverts > 0)
        {
            var placed = IntrovertGain(neighbors) + BestFromBruteForce(layout, PlaceIntrovert(layout, state));
            best = Math.Max(best, placed);
        }

        if (state.Extroverts > 0)
        {
            var placed = ExtrovertGain(neighbors) + BestFromBruteForce(layout, PlaceExtrovert(layout, state));
            best = Math.Max(best, placed);
        }

        return best;
    }

    // This repo's own Memoizer, keyed on the whole (Pos, Mask, Introverts,
    // Extroverts) state - the same (Row, PrevMask) profile-DP memo
    // MaximumStudentsTakingExamSolution already establishes, widened to a 4-tuple
    // since two independently-exhaustible people pools replace that problem's single
    // per-row choice. Every reachable state is computed exactly once.
    public static int GetMaxGridHappinessByMemoizedProfileDp(
        int m, int n, int introvertsCount, int extrovertsCount) =>
        GetMaxGridHappinessByMemoizedProfileDp(GridLayout.Build(m, n), introvertsCount, extrovertsCount);

    public static int GetMaxGridHappinessByMemoizedProfileDp(
        GridLayout layout, int introvertsCount, int extrovertsCount) =>
        Memoizer.Memoize<GridState, int>(
            new GridState(0, 0, introvertsCount, extrovertsCount),
            (state, bestFrom) => BestFrom(layout, state, bestFrom));

    private static int BestFrom(GridLayout layout, GridState state, Func<GridState, int> bestFrom)
    {
        if (IsTerminal(layout, state))
        {
            return 0;
        }

        var neighbors = layout.Neighbors(state.Pos, state.Mask);
        var best = bestFrom(SkipCell(layout, state));

        if (state.Introverts > 0)
        {
            var placed = IntrovertGain(neighbors) + bestFrom(PlaceIntrovert(layout, state));
            best = Math.Max(best, placed);
        }

        if (state.Extroverts > 0)
        {
            var placed = ExtrovertGain(neighbors) + bestFrom(PlaceExtrovert(layout, state));
            best = Math.Max(best, placed);
        }

        return best;
    }

    // Nothing is left to decide once every cell is filled or both pools are empty -
    // an empty cell is worth nothing, so the remaining grid cannot add happiness.
    private static bool IsTerminal(GridLayout layout, GridState state)
        => state.Pos == layout.TotalCells || (state.Introverts == 0 && state.Extroverts == 0);

    private static GridState SkipCell(GridLayout layout, GridState state)
        => state with { Pos = state.Pos + 1, Mask = layout.ShiftIn(state.Mask, EmptyTypeCode) };

    private static GridState PlaceIntrovert(GridLayout layout, GridState state)
        => state with
        {
            Pos = state.Pos + 1,
            Mask = layout.ShiftIn(state.Mask, IntrovertTypeCode),
            Introverts = state.Introverts - 1,
        };

    private static GridState PlaceExtrovert(GridLayout layout, GridState state)
        => state with
        {
            Pos = state.Pos + 1,
            Mask = layout.ShiftIn(state.Mask, ExtrovertTypeCode),
            Extroverts = state.Extroverts - 1,
        };

    private static int IntrovertGain((int Up, int Left) neighbors)
        => IntrovertBaseHappiness
            + NeighborDelta(IntrovertTypeCode, neighbors.Up)
            + NeighborDelta(IntrovertTypeCode, neighbors.Left);

    private static int ExtrovertGain((int Up, int Left) neighbors)
        => ExtrovertBaseHappiness
            + NeighborDelta(ExtrovertTypeCode, neighbors.Up)
            + NeighborDelta(ExtrovertTypeCode, neighbors.Left);

    // Both people in an adjacent pair are affected, so a placement collects its own
    // delta and its neighbor's at the same time; an empty neighbor costs nothing.
    private static int NeighborDelta(int typeCode, int neighborTypeCode)
    {
        if (neighborTypeCode == EmptyTypeCode)
        {
            return 0;
        }

        var selfDelta = typeCode == IntrovertTypeCode ? IntrovertAdjacencyDelta : ExtrovertAdjacencyDelta;
        var neighborDelta = neighborTypeCode == IntrovertTypeCode
            ? IntrovertAdjacencyDelta
            : ExtrovertAdjacencyDelta;

        return selfDelta + neighborDelta;
    }
}
