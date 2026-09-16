using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.NumberOfWaysToPaintN3Grid;

// LeetCode 1411. Number of Ways to Paint N x 3 Grid: count the colorings of an
// n x 3 grid with three colors where no two adjacent cells share a color, the
// answer reported mod 1e9+7.
//
// Every valid row is one of two shapes: "Same" - its two end cells match, an
// ABA pattern, 6 of them - or "Different" - all three cells distinct, an ABC
// pattern, also 6. Only the shape matters for what may follow, so the whole
// state is the pair (same[row], different[row]):
//
//   same[r]      = 3 * same[r-1] + 2 * different[r-1]
//   different[r] = 2 * same[r-1] + 2 * different[r-1]
//
// seeded at same[1] = different[1] = 6, and the answer is their sum.
//
// Both strategies evaluate exactly that recurrence in O(n) and differ only in
// direction: a bottom-up scan carrying the pair in two locals, or this repo's
// own Memoizer driving the same pair top-down - the NumberOfMusicPlaylists
// pairing, on a state that is just the row index.
internal static class NumberOfWaysToPaintN3GridSolution
{
    private const int FirstRow = 1;
    private const int SecondRow = 2;
    private const long FirstRowPatternCount = 6;
    private const long SameFollowerWeight = 3;
    private const long DifferentFollowerWeight = 2;

    // The textbook answer: a bottom-up scan carrying (same, different) forward
    // in two locals. Deliberately written without this repo's primitives - it is
    // the arm the memoized strategy below has to justify itself against.
    public static long CountWaysByTabulation(int rowCount)
    {
        var same = FirstRowPatternCount;
        var different = FirstRowPatternCount;

        for (var row = SecondRow; row <= rowCount; row++)
        {
            var nextSame = ((SameFollowerWeight * same) + (DifferentFollowerWeight * different))
                % ModularArithmetic.Modulo;
            var nextDifferent = ((DifferentFollowerWeight * same) + (DifferentFollowerWeight * different))
                % ModularArithmetic.Modulo;
            same = nextSame;
            different = nextDifferent;
        }

        return (same + different) % ModularArithmetic.Modulo;
    }

    // This repo's own top-down engine: Memoizer.Memoize caches each row's
    // (same, different) pair the first time it is reached, so the recurrence
    // reads as ordinary recursion with no hand-rolled cache dictionary.
    public static long CountWaysByMemoizedRecurrence(int rowCount)
    {
        var (same, different) = Memoizer.Memoize<int, (long Same, long Different)>(rowCount, new RowPatternCounts());
        return (same + different) % ModularArithmetic.Modulo;
    }

    // The recurrence, as a named type: the row-pattern counts one row on are the previous
    // row's pair, each weighted by what the shape it paints may be followed by - seeded at
    // the first row, where all six patterns of each shape paint without conflict.
    private sealed class RowPatternCounts : IRecurrence<int, (long Same, long Different)>
    {
        public (long Same, long Different) Replay(
            int row, IRecurrence<int, (long Same, long Different)> rest)
        {
            if (row == FirstRow)
            {
                return (FirstRowPatternCount, FirstRowPatternCount);
            }

            var (prevSame, prevDifferent) = rest.Replay(row - 1, rest);

            return (
                ((SameFollowerWeight * prevSame) + (DifferentFollowerWeight * prevDifferent))
                    % ModularArithmetic.Modulo,
                ((DifferentFollowerWeight * prevSame) + (DifferentFollowerWeight * prevDifferent))
                    % ModularArithmetic.Modulo);
        }
    }
}
