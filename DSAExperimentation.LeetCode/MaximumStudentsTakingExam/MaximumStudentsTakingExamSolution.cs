using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MaximumStudentsTakingExam;

// LeetCode 1349. Maximum Students Taking Exam: seat as many students as possible in a
// classroom where '#' seats are broken and a student can copy from the seat directly
// left, directly right, upper-left or upper-right of their own.
//
// A row's occupancy is a bitmask over its columns, so the state is (row, previous
// row's mask). A candidate mask is legal when it uses no broken seat, has no two
// horizontally adjacent bits (nobody copies sideways), and shares no bit with the
// previous row's mask shifted one column each way (nobody copies diagonally
// forward). The answer maximizes the summed popcount down the rows.
//
// Both strategies run that identical recursion; they differ only in whether a (row,
// prevMask) state reached by several different row sequences is recomputed each time
// or once.
internal static class MaximumStudentsTakingExamSolution
{
    // The textbook baseline: the same recursion with no memoization at all, so the
    // identical (row, prevMask) subtree is re-explored once per earlier row sequence
    // that happens to land on that mask. Deliberately plain recursion over BCL arrays
    // - the arm the memoized strategy below has to justify itself against.
    public static int MaxStudentsByBruteForceRecursion(char[][] seats) =>
        MaxStudentsByBruteForceRecursion(SeatMasks.Build(seats));

    public static int MaxStudentsByBruteForceRecursion(SeatMasks seats) =>
        BestFromBruteForce(seats, 0, 0);

    private static int BestFromBruteForce(SeatMasks seats, int row, int prevMask)
    {
        if (row == seats.Rows)
        {
            return 0;
        }

        var best = 0;

        for (var mask = 0; mask <= seats.FullMask; mask++)
        {
            if (!IsSeatable(mask, prevMask, seats.BrokenMask[row]))
            {
                continue;
            }

            best = Math.Max(best, PopCount(mask) + BestFromBruteForce(seats, row + 1, mask));
        }

        return best;
    }

    // This repo's own Memoizer, keyed on the (Row, PrevMask) tuple - the same 2D
    // memo state LongestIncreasingPathInAMatrix's (Row, Col) uses, applied to a
    // bitmask second component. Every reachable state is computed exactly once.
    public static int MaxStudentsByMemoizedBitmask(char[][] seats) =>
        MaxStudentsByMemoizedBitmask(SeatMasks.Build(seats));

    public static int MaxStudentsByMemoizedBitmask(SeatMasks seats) =>
        Memoizer.Memoize<(int Row, int PrevMask), int>(
            (0, 0), (state, bestFrom) => BestFrom(seats, state, bestFrom));

    private static int BestFrom(
        SeatMasks seats, (int Row, int PrevMask) state, Func<(int Row, int PrevMask), int> bestFrom)
    {
        var (row, prevMask) = state;

        if (row == seats.Rows)
        {
            return 0;
        }

        var best = 0;

        for (var mask = 0; mask <= seats.FullMask; mask++)
        {
            if (!IsSeatable(mask, prevMask, seats.BrokenMask[row]))
            {
                continue;
            }

            best = Math.Max(best, PopCount(mask) + bestFrom((row + 1, mask)));
        }

        return best;
    }

    // No student on a broken seat, none beside another in the same row, and none
    // diagonally ahead of a student in the row above.
    private static bool IsSeatable(int mask, int prevMask, int broken)
        => (mask & broken) == 0
            && (mask & (mask << 1)) == 0
            && (mask & (prevMask << 1)) == 0
            && (mask & (prevMask >> 1)) == 0;

    private static int PopCount(int mask)
    {
        var count = 0;

        while (mask != 0)
        {
            mask &= mask - 1;
            count++;
        }

        return count;
    }
}
