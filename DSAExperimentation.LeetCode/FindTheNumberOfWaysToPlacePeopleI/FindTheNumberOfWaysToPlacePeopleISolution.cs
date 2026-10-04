using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.LeetCode.FindTheNumberOfWaysToPlacePeopleII;

namespace DSAExperimentation.LeetCode.FindTheNumberOfWaysToPlacePeopleI;

// LeetCode 3025. Find the Number of Ways to Place People I: count ordered
// pairs (Alice, Bob) where Alice's point sits at the upper-left corner
// (xi <= xj, yi >= yj) of the axis-aligned rectangle Bob's point sits at the
// lower-right corner of, and no third point lies inside or on that
// rectangle's boundary.
//
// It is LC 3027 at n <= 50, so LC 3027's class owns both arms (ARCHITECTURE 17.3):
// the O(n^3) "check every third point" baseline, already fast enough here, and the
// O(n^2 log n) sorted sweep LC 3027 needs at its own bound, exactly as correct at
// this smaller one. Each arm here calls through; both problems answer an int, so
// nothing narrows, and this problem's own test and benchmark still run them at its
// bound.
internal static class FindTheNumberOfWaysToPlacePeopleISolution
{
    // The textbook O(n^3) scan: every ordered pair, checked against every third point
    // for one that would block the rectangle. The arm the sorted sweep has to beat.
    public static int CountPairsByBruteForce(int[][] points) =>
        FindTheNumberOfWaysToPlacePeopleIISolution.CountPairsByBruteForce(points);

    public static int CountPairsBySortedSweep(int[][] points) =>
        FindTheNumberOfWaysToPlacePeopleIISolution.CountPairsBySortedSweep(points);

    // Prepared-input overload: `sortedPoints` must already be sorted by x ascending,
    // y descending on ties - PointOrder.ByXThenDescendingY, which a benchmark's
    // [GlobalSetup] sorts with so the sort is not charged to the sweep.
    public static int CountPairsBySortedSweep(ArrayIndexedSequence<int[]> sortedPoints) =>
        FindTheNumberOfWaysToPlacePeopleIISolution.CountPairsBySortedSweep(sortedPoints);
}
