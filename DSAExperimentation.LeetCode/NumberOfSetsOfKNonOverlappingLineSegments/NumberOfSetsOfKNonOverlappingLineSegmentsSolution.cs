using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.NumberOfSetsOfKNonOverlappingLineSegments;

// LeetCode 1621. Number of Sets of K Non-Overlapping Line Segments: pointCount
// points laid out left to right at x = 0..pointCount-1, choose segmentCount
// non-overlapping segments (touching at a shared endpoint is allowed), modulo 1e9+7.
//
// Only the point count and the segment count matter, never the actual coordinates -
// a segment is just a pair of increasing indices, and a set of them is non-overlapping
// iff, sorted by left endpoint, each one's right endpoint is <= the next one's left
// endpoint. That combinatorial count is the stars-and-bars identity
// C(pointCount + segmentCount - 1, 2 * segmentCount) (LeetCode's own published
// pointCount=4,segmentCount=2 -> 5 and pointCount=5,segmentCount=3 -> 7 examples
// agree), so the whole problem reduces to Pascal's identity and the two strategies
// differ only in the direction they drive it: bottom-up tabulation or top-down
// through this repo's own Memoizer, the tabulation-vs-Memoizer contrast
// CountAllValidPickupAndDeliveryOptionsSolution draws on its own counting
// recurrence.
//
// The one addition a tree-shaped Choose does not need: a segmentCount > pointCount
// guard, since 2 * segmentCount can exceed pointCount + segmentCount - 1 here
// (whenever there simply are not enough points to seat segmentCount segments, even
// with every pair touching). Without it the plain Pascal split recurses on states
// with a negative upper index forever instead of hitting either base case.
internal static class NumberOfSetsOfKNonOverlappingLineSegmentsSolution
{
    // C(pointCount + segmentCount - 1, 2 * segmentCount): the identity the problem
    // reduces to.
    private const int PointOffset = 1;
    private const int SegmentEndpointCount = 2;

    // The textbook baseline: fill Pascal's triangle bottom-up into a plain
    // two-dimensional table and read the one cell the identity asks for. BCL
    // arithmetic and a BCL array only.
    public static int NumberOfSetsByTabulation(int pointCount, int segmentCount)
    {
        var rows = pointCount + segmentCount - PointOffset;
        var target = SegmentEndpointCount * segmentCount;
        var table = new long[rows + 1, target + 1];

        for (var row = 0; row <= rows; row++)
        {
            for (var col = 0; col <= Math.Min(row, target); col++)
            {
                table[row, col] = col == 0 || col == row
                    ? 1
                    : PascalEntry(table, row, col);
            }
        }

        return (int)table[rows, target];
    }

    private static long PascalEntry(long[,] table, int row, int col) =>
        (table[row - 1, col - 1] + table[row - 1, col]) % ModularArithmetic.Modulo;

    // The same identity driven top-down through this repo's Memoizer, so each
    // binomial state is solved once and shared by every recursive call reaching it.
    public static int NumberOfSetsByMemoizedPascal(int pointCount, int segmentCount) =>
        (int)Choose(
            pointCount + segmentCount - PointOffset,
            SegmentEndpointCount * segmentCount);

    private static long Choose(int totalCount, int chosenCount) =>
        Memoizer.Memoize<(int N, int K), long>(
            (totalCount, chosenCount), new PascalCoefficient());

    // The recurrence, as a named type: Pascal's identity,
    // C(totalCount, chosenCount)
    //     = C(totalCount-1, chosenCount-1) + C(totalCount-1, chosenCount),
    // seeded at the edges of the triangle and zero wherever chosenCount falls past
    // totalCount - the guard the plain split would recurse past forever.
    private sealed class PascalCoefficient : IRecurrence<(int N, int K), long>
    {
        public long Replay((int N, int K) state, IRecurrence<(int N, int K), long> rest)
        {
            var (totalCount, chosenCount) = state;

            if (chosenCount == 0 || chosenCount == totalCount)
            {
                return 1;
            }

            if (chosenCount > totalCount)
            {
                return 0;
            }

            return (rest.Replay((totalCount - 1, chosenCount - 1), rest)
                + rest.Replay((totalCount - 1, chosenCount), rest))
                % ModularArithmetic.Modulo;
        }
    }
}
