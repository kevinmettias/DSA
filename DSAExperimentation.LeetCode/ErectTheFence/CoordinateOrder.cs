using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.ErectTheFence;

// The order Andrew's monotone chain is defined over: (x, y) ascending, x deciding
// unless two points share an x, in which case y breaks the tie. Sweeping this order
// left to right is what lets a single pop-while-not-a-left-turn pass build each half
// of the hull, so the order is the sweep's precondition rather than a step inside it.
//
// LC 587 and LC 812 both sweep a hull this way - LC 587 to find the boundary points
// its fence needs, LC 812 to reduce the candidate set its cubic search runs over -
// and the order is the one thing they have to agree on, so it is written once, here
// in LC 587's own folder, and LC 812 calls in for it rather than sorting by
// coordinate a second way - the arrangement SqrtX's SquareExceedsSequence has with
// FourDivisors, ClosestDivisors and ThreeDivisors. What each problem then does with
// the sorted chain stays its own: LC 587 needs every point collinear-and-between on a
// hull edge, LC 812 only the strict corners.
internal static class CoordinateOrder
{
    // A sorted copy, so the caller's own array is left as it was given.
    public static (int X, int Y)[] SortedByCoordinates((int X, int Y)[] points)
    {
        var sorted = points.ToArray();

        MergeSort.Sort<(int X, int Y), ArrayIndexedSequence<(int X, int Y)>>(
            new ArrayIndexedSequence<(int X, int Y)>(sorted),
            Comparer<(int X, int Y)>.Create((a, b) => a.X != b.X ? a.X.CompareTo(b.X) : a.Y.CompareTo(b.Y)));

        return sorted;
    }
}
