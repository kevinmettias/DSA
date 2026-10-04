using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Algorithms.Geometry;

// A point set's convex hull by Andrew's monotone chain: sort the points by (X, Y), then sweep them
// left to right and back, popping every point that would make the chain turn right or run straight,
// so each half keeps only strict corners. Flat - there is no topology to be generic over, only
// points in the plane - and in tier 2 because it composes Sorting/MergeSort, the reasoning that put
// HammingDistances there.
//
// Corners come out counter-clockwise from the smallest (X, Y), each distinct point at most once and
// no point that lies on an edge between two corners. Degenerate inputs follow from that: no points
// give no corners, coincident points give the one point, and collinear points give the two ends.
// The cross product runs in Int128, so it is exact for every int coordinate and the hull carries no
// range precondition. The chain is built in one array rather than a stack: it is scratch state
// inside one call (§16.4), indexed from both halves of the sweep.
internal static class ConvexHull
{
    // A turn is judged from the two points already on the chain plus the incoming one.
    private const int TurnWindow = 2;

    // The sweep runs out and back, and each half pushes every point at most once.
    private const int SweepHalves = 2;

    public static (int X, int Y)[] Corners(ReadOnlySpan<(int X, int Y)> points)
    {
        var buffer = points.ToArray();
        var sorted = DistinctInCoordinateOrder(buffer);

        if (sorted.Length <= 1)
        {
            return sorted.ToArray();
        }

        var chain = new (int X, int Y)[sorted.Length * SweepHalves];
        var lowerLength = SweepOut(chain, sorted);
        var length = SweepBack(chain, sorted, lowerLength);

        // The return sweep ends back at the first point, which the chain already starts with.
        return chain[..(length - 1)];
    }

    // The caller's copy, sorted and with repeats moved out of the way: the distinct points are
    // the returned front of the buffer, so no corner is reported twice and nothing is copied again.
    private static ReadOnlySpan<(int X, int Y)> DistinctInCoordinateOrder((int X, int Y)[] buffer)
    {
        MergeSort.Sort<(int X, int Y), ArrayIndexedSequence<(int X, int Y)>>(
            new ArrayIndexedSequence<(int X, int Y)>(buffer));

        var distinct = 0;

        foreach (var point in buffer)
        {
            if (distinct == 0 || point != buffer[distinct - 1])
            {
                buffer[distinct++] = point;
            }
        }

        return buffer.AsSpan(0, distinct);
    }

    // The lower half: every point left to right. Returns the chain's length.
    private static int SweepOut((int X, int Y)[] chain, ReadOnlySpan<(int X, int Y)> sorted)
    {
        var length = 0;

        foreach (var point in sorted)
        {
            length = Extend(chain, length, TurnWindow, point);
        }

        return length;
    }

    // The upper half: back from the point before the last, which the lower half ended on, never
    // popping into the lower half below it. Returns the chain's length.
    private static int SweepBack((int X, int Y)[] chain, ReadOnlySpan<(int X, int Y)> sorted, int lowerLength)
    {
        var returning = sorted[..^1];
        var upperFloor = lowerLength + 1;
        var length = lowerLength;

        for (var i = returning.Length - 1; i >= 0; i--)
        {
            length = Extend(chain, length, upperFloor, returning[i]);
        }

        return length;
    }

    // Pops every trailing point the incoming one would leave on a right turn or a straight run -
    // never below `floor` - then appends it.
    private static int Extend((int X, int Y)[] chain, int length, int floor, (int X, int Y) point)
    {
        while (length >= floor && Cross(chain[length - TurnWindow], chain[length - 1], point) <= 0)
        {
            length--;
        }

        chain[length] = point;

        return length + 1;
    }

    // Positive when origin -> first -> second turns left (counter-clockwise). Each difference fits
    // a long, and Math.BigMul takes two longs to their exact 128-bit product in one multiply.
    private static Int128 Cross((int X, int Y) origin, (int X, int Y) first, (int X, int Y) second)
        => Math.BigMul((long)first.X - origin.X, (long)second.Y - origin.Y)
            - Math.BigMul((long)first.Y - origin.Y, (long)second.X - origin.X);
}
