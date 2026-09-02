using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.FindTheNumberOfWaysToPlacePeopleI;

// LeetCode 3025. Find the Number of Ways to Place People I: count ordered
// pairs (Alice, Bob) where Alice's point sits at the upper-left corner
// (xi <= xj, yi >= yj) of the axis-aligned rectangle Bob's point sits at the
// lower-right corner of, and no third point lies inside or on that
// rectangle's boundary.
//
// n <= 50 here, so the O(n^3) "check every third point" baseline is already
// fast enough - but the O(n^2 log n) sorted-sweep strategy LC 3027 actually
// needs to clear ITS bound is exactly as correct at this smaller one, so both
// strategies are proven here too, the same way MaximumStrongPairXORI still
// carries the bucket strategy its own bound doesn't strictly require. See
// FindTheNumberOfWaysToPlacePeopleIISolution for the sweep strategy's own
// reasoning - identical here, just restated at LC 3025's scale.
internal static class FindTheNumberOfWaysToPlacePeopleISolution
{
    // The textbook O(n^3) scan: every ordered pair, checked against every
    // third point for one that would block the rectangle. Correct at any
    // scale, and the arm the sorted-sweep strategy below has to beat.
    public static int CountPairsByBruteForce(int[][] points)
    {
        var count = 0;

        for (var aliceIndex = 0; aliceIndex < points.Length; aliceIndex++)
        {
            for (var bobIndex = 0; bobIndex < points.Length; bobIndex++)
            {
                if (aliceIndex != bobIndex &&
                    IsUpperLeftOf(points[aliceIndex], points[bobIndex]) &&
                    NoPointBlocks(points, aliceIndex, bobIndex))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool IsUpperLeftOf(int[] alice, int[] bob) => alice[0] <= bob[0] && alice[1] >= bob[1];

    private static bool NoPointBlocks(int[][] points, int aliceIndex, int bobIndex)
    {
        var (aliceX, aliceY) = (points[aliceIndex][0], points[aliceIndex][1]);
        var (bobX, bobY) = (points[bobIndex][0], points[bobIndex][1]);

        for (var k = 0; k < points.Length; k++)
        {
            if (k == aliceIndex || k == bobIndex)
            {
                continue;
            }

            var (x, y) = (points[k][0], points[k][1]);

            if (x >= aliceX && x <= bobX && y <= aliceY && y >= bobY)
            {
                return false;
            }
        }

        return true;
    }

    public static int CountPairsBySortedSweep(int[][] points)
    {
        var sorted = new ArrayIndexedSequence<int[]>((int[][])points.Clone());
        MergeSort.Sort<int[], ArrayIndexedSequence<int[]>>(sorted, ByXThenDescendingY);

        return CountPairsBySortedSweep(sorted);
    }

    // Prepared-input overload: `sortedPoints` must already be sorted by x
    // ascending, y descending on ties (ByXThenDescendingY below - exposed so a
    // benchmark's [GlobalSetup] can sort with the exact rule this strategy's
    // precondition depends on, instead of duplicating it). For a fixed Alice
    // i, every point to its right in that order already has x >= points[i].x,
    // so a single left-to-right scan tracking the highest y counted so far
    // (maxY) finds every visible Bob: a closer point k (i < k < j) only
    // blocks j when points[k].y falls in (points[j].y, points[i].y], which is
    // exactly the range maxY has already absorbed once such a point has been
    // counted.
    public static int CountPairsBySortedSweep(ArrayIndexedSequence<int[]> sortedPoints)
    {
        var count = 0;

        for (var i = 0; i < sortedPoints.Length; i++)
        {
            var aliceY = sortedPoints.Get(i)[1];
            var maxY = int.MinValue;

            for (var j = i + 1; j < sortedPoints.Length; j++)
            {
                var bobY = sortedPoints.Get(j)[1];

                if (bobY <= aliceY && bobY > maxY)
                {
                    count++;
                    maxY = bobY;
                }
            }
        }

        return count;
    }

    public static readonly IComparer<int[]> ByXThenDescendingY =
        Comparer<int[]>.Create((a, b) => a[0] != b[0] ? a[0].CompareTo(b[0]) : b[1].CompareTo(a[1]));
}
