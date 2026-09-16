using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.FindTheNumberOfWaysToPlacePeopleII;

// LeetCode 3027. Find the Number of Ways to Place People II: the same
// question as LC 3025 (count ordered pairs (Alice, Bob) where Alice's point
// sits at the upper-left corner (xi <= xj, yi >= yj) of the axis-aligned
// rectangle Bob's point sits at the lower-right corner of, and no third
// point lies inside or on that rectangle's boundary), at a scale (n up to
// 1000) where the O(n^3) "check every third point" scan is too slow to be
// the intended solution - it stays here as the baseline the sorted-sweep
// strategy has to beat.
//
// The sweep strategy rests on one fact once points are sorted by x
// ascending (y descending on ties): for a fixed Alice i, every candidate Bob
// j > i already satisfies x_j >= x_i, so the only remaining question is
// whether some closer point k (i < k < j) blocks the rectangle - which it
// does exactly when points[k].y falls in (points[j].y, points[i].y], since k
// already sits between i and j on the x-axis by construction of the sort
// order. A single left-to-right scan that tracks the highest y value counted
// so far (maxY) is therefore enough: j is visible from i precisely when
// points[j].y <= points[i].y and points[j].y > maxY, and counting it updates
// maxY to points[j].y - no removal, no auxiliary structure, just the sort.
internal static class FindTheNumberOfWaysToPlacePeopleIISolution
{
    // The textbook O(n^3) scan: every ordered pair, checked against every
    // third point for one that would block the rectangle. Correct at any
    // scale, and the arm the sorted-sweep strategy below has to beat once n
    // reaches LC 3027's own bound.
    public static int CountPairsByBruteForce(int[][] points)
    {
        var count = 0;

        for (var aliceIndex = 0; aliceIndex < points.Length; aliceIndex++)
        {
            for (var bobIndex = 0; bobIndex < points.Length; bobIndex++)
            {
                if (IsValidPlacement(points, aliceIndex, bobIndex))
                {
                    count++;
                }
            }
        }

        return count;
    }

    // An ordered pair counts when the two points are distinct, Alice's sits at the
    // rectangle's upper-left corner, and no third point blocks it.
    private static bool IsValidPlacement(int[][] points, int aliceIndex, int bobIndex) =>
        aliceIndex != bobIndex
        && IsUpperLeftOf(points[aliceIndex], points[bobIndex])
        && HasNoBlockingPoint(points, aliceIndex, bobIndex);

    private static bool IsUpperLeftOf(int[] alice, int[] bob) => alice[0] <= bob[0] && alice[1] >= bob[1];

    private static bool HasNoBlockingPoint(int[][] points, int aliceIndex, int bobIndex)
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

            if (IsWithinXSpan(x, aliceX, bobX) && IsWithinYSpan(y, aliceY, bobY))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsWithinXSpan(int pointX, int left, int right) => pointX >= left && pointX <= right;

    private static bool IsWithinYSpan(int pointY, int top, int bottom) => pointY <= top && pointY >= bottom;

    public static int CountPairsBySortedSweep(int[][] points)
    {
        var sorted = new ArrayIndexedSequence<int[]>((int[][])points.Clone());
        MergeSort.Sort<int[], ArrayIndexedSequence<int[]>>(sorted, XThenDescendingYOrder.Rule);

        return CountPairsBySortedSweep(sorted);
    }

    // Prepared-input overload: `sortedPoints` must already be sorted by x
    // ascending, y descending on ties (the XThenDescendingYOrder rule this
    // strategy's precondition names - a type of its own so a benchmark's
    // [GlobalSetup] can sort with the exact rule instead of duplicating it). See
    // this class's own doc comment for why the maxY sweep is sufficient.
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
}
