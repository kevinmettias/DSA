using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.FindTheLargestAreaOfSquareInsideTwoRectangles;

// LeetCode 3047. Find the Largest Area of Square Inside Two Rectangles: over
// every pair of rectangles, the largest square that fits their intersection
// has side length min(overlapWidth, overlapHeight), where overlapWidth =
// min(c_i, c_j) - max(a_i, a_j) and overlapHeight is the same computation on
// the y-axis. Maximize that side across every pair, square it, or report 0
// if no pair overlaps with positive area.
internal static class FindTheLargestAreaOfSquareInsideTwoRectanglesSolution
{
    // Textbook baseline: every pair, in input order, no repo primitives - the
    // arm the sorted-and-pruned strategy below has to beat.
    public static long LargestSquareAreaByBruteForcePairs(int[][] bottomLeft, int[][] topRight)
    {
        var bestSide = 0;

        for (var i = 0; i < bottomLeft.Length; i++)
        {
            for (var j = i + 1; j < bottomLeft.Length; j++)
            {
                var width = Math.Min(topRight[i][0], topRight[j][0]) - Math.Max(bottomLeft[i][0], bottomLeft[j][0]);
                var height = Math.Min(topRight[i][1], topRight[j][1]) - Math.Max(bottomLeft[i][1], bottomLeft[j][1]);
                var side = Math.Min(width, height);

                if (side > bestSide)
                {
                    bestSide = side;
                }
            }
        }

        return (long)bestSide * bestSide;
    }

    // Sort rectangle indices by left x-coordinate with this repo's own
    // MergeSort over an ArrayIndexedSequence<int> - the same composition
    // FindPolygonWithTheLargestPerimeterSolution uses for LC 2971 - so that
    // for a fixed rectangle i and every later rectangle j in sorted order,
    // max(a_i, a_j) is always a_j. overlapWidth is therefore bounded above by
    // topRight[i][0] - a_j, a quantity that only shrinks as j advances
    // through the sorted order: once it can no longer beat the best square
    // found so far, neither can any later j, and the inner loop breaks. Worst
    // case is still every pair (an adversarial input can force that), but a
    // typical one prunes most of them.
    public static long LargestSquareAreaBySortedPrunedPairs(int[][] bottomLeft, int[][] topRight)
    {
        var order = new int[bottomLeft.Length];

        for (var i = 0; i < order.Length; i++)
        {
            order[i] = i;
        }

        var byLeftX = Comparer<int>.Create((x, y) => bottomLeft[x][0].CompareTo(bottomLeft[y][0]));
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(order), byLeftX);

        var bestSide = 0;

        for (var p = 0; p < order.Length; p++)
        {
            bestSide = BestSideFromLaterRectangles((bottomLeft, topRight), order, p, bestSide);
        }

        return (long)bestSide * bestSide;
    }

    // One fixed pivot rectangle, scanned against every later rectangle in sorted order.
    // Because max(a_i, a_j) is a_j there, overlapWidth is at most topRight[i][0] - a_j,
    // which only shrinks as q advances - so once that bound can no longer beat the best
    // square found so far, neither can any later q and the scan stops early.
    private static int BestSideFromLaterRectangles(
        (int[][] BottomLeft, int[][] TopRight) rectangles, int[] order, int pivot, int bestSide)
    {
        var (bottomLeft, topRight) = rectangles;

        for (var q = pivot + 1; q < order.Length; q++)
        {
            var j = order[q];
            var widthBound = topRight[pivot][0] - bottomLeft[j][0];

            if (widthBound <= bestSide)
            {
                break;
            }

            var width = Math.Min(topRight[pivot][0], topRight[j][0]) - bottomLeft[j][0];
            var height = Math.Min(topRight[pivot][1], topRight[j][1]) - Math.Max(bottomLeft[pivot][1], bottomLeft[j][1]);
            var side = Math.Min(width, height);

            bestSide = Math.Max(bestSide, side);
        }

        return bestSide;
    }
}
