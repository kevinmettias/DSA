using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.NumberOfSetsOfKNonOverlappingLineSegments;

// LeetCode 1621. Number of Sets of K Non-Overlapping Line Segments: n points laid
// out left to right at x = 0..n-1, choose k non-overlapping segments (touching at a
// shared endpoint is allowed), modulo 1e9+7.
//
// Only the point count and k matter, never the actual coordinates - a segment is
// just a pair of increasing indices, and a set of them is non-overlapping iff,
// sorted by left endpoint, each one's right endpoint is <= the next one's left
// endpoint. That combinatorial count is the stars-and-bars identity C(n-1+k, 2k)
// (LeetCode's own published n=4,k=2 -> 5 and n=5,k=3 -> 7 examples agree), so the
// whole problem reduces to Pascal's identity and the two strategies differ only in
// the direction they drive it: bottom-up tabulation or top-down through this repo's
// own Memoizer, the tabulation-vs-Memoizer contrast
// CountAllValidPickupAndDeliveryOptionsSolution draws on its own counting
// recurrence.
//
// The one addition a tree-shaped Choose does not need: a k > n guard, since 2k can
// exceed n-1+k here (whenever there simply are not enough points to seat k
// segments, even with every pair touching). Without it the plain Pascal split
// recurses on negative-n states forever instead of hitting either base case.
internal static class NumberOfSetsOfKNonOverlappingLineSegmentsSolution
{
    // C(n + k - 1, 2k): the identity the problem reduces to.
    private const int PointOffset = 1;
    private const int SegmentEndpointCount = 2;

    // The textbook baseline: fill Pascal's triangle bottom-up into a plain
    // two-dimensional table and read the one cell the identity asks for. BCL
    // arithmetic and a BCL array only.
    public static int NumberOfSetsByTabulation(int n, int k)
    {
        var rows = n + k - PointOffset;
        var target = SegmentEndpointCount * k;
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
    public static int NumberOfSetsByMemoizedPascal(int n, int k) =>
        (int)Choose(n + k - PointOffset, SegmentEndpointCount * k);

    private static long Choose(int n, int k) =>
        Memoizer.Memoize<(int N, int K), long>((n, k), new PascalCoefficient());

    // The recurrence, as a named type: Pascal's identity, C(n, k) = C(n-1, k-1) +
    // C(n-1, k), seeded at the edges of the triangle and zero wherever k falls past n -
    // the guard the plain split would recurse past forever.
    private sealed class PascalCoefficient : IRecurrence<(int N, int K), long>
    {
        public long Replay((int N, int K) state, IRecurrence<(int N, int K), long> rest)
        {
            var (n, k) = state;

            if (k == 0 || k == n)
            {
                return 1;
            }

            if (k > n)
            {
                return 0;
            }

            return (rest.Replay((n - 1, k - 1), rest) + rest.Replay((n - 1, k), rest))
                % ModularArithmetic.Modulo;
        }
    }
}
