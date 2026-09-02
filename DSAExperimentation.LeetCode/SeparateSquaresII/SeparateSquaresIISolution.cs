using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.LeetCode.SeparateSquaresII;

// LC 3454: squares may overlap, so the union of their areas (not the sum) is what
// has to be split evenly by a horizontal line. Both strategies are the same sweep:
// walk the squares' y-coordinates as band boundaries, and within each band the
// union of squares' area is (covered x-length) * (band height) since every square
// active in a band spans it fully. The two strategies differ only in how the
// covered x-length of a band's active squares is computed - that one step is
// pulled out as a delegate so the sweep bookkeeping itself (which this problem
// contributes, not either primitive) is written once.
internal static class SeparateSquaresIISolution
{
    // The textbook arm: covered x-length via a hand-sorted, hand-merged BCL List -
    // deliberately not this repo's own IntervalSet, so it is what the composed arm
    // below has to justify itself against.
    public static double MinYByEventSweep(int[][] squares) => MinYByEventSweep(ParseSquares(squares));

    public static double MinYByEventSweep(IReadOnlyList<Square> squares) =>
        Sweep(squares, CoveredXLengthByListMerge);

    // This repo's own IntervalSet<long> already IS "add an interval, keep the set
    // merged" - covered x-length is just the sum of its merged intervals' lengths,
    // no hand-written merge loop needed.
    public static double MinYByIntervalSetSweep(int[][] squares) => MinYByIntervalSetSweep(ParseSquares(squares));

    public static double MinYByIntervalSetSweep(IReadOnlyList<Square> squares) =>
        Sweep(squares, CoveredXLengthByIntervalSet);

    private static double Sweep(IReadOnlyList<Square> squares, Func<List<Square>, long> coveredXLength)
    {
        var breakpoints = CollectSortedYBreakpoints(squares);
        var addAt = squares.ToLookup(square => square.Y);
        var removeAt = squares.ToLookup(square => square.Y + square.L);
        var active = new List<Square>();
        var bands = new List<(long Y0, long Y1, long Width)>();
        var totalArea = 0L;

        for (var i = 0; i < breakpoints.Count - 1; i++)
        {
            var y0 = breakpoints[i];
            var y1 = breakpoints[i + 1];

            active.AddRange(addAt[y0]);

            var width = coveredXLength(active);
            bands.Add((y0, y1, width));
            totalArea += width * (y1 - y0);

            foreach (var ending in removeAt[y1])
            {
                active.Remove(ending);
            }
        }

        return FindSplitLine(bands, totalArea);
    }

    // Bands are area-monotonic (width*height >= 0), so the split line lies in the
    // first band whose cumulative area reaches half the total - linear
    // interpolation within that band gives the exact y. LC asks for the *minimum*
    // such y: if half is reached exactly on a band boundary, that boundary is
    // returned immediately rather than drifting into a later zero-width band that
    // would also (but less minimally) satisfy the equal-split condition.
    private static double FindSplitLine(List<(long Y0, long Y1, long Width)> bands, long totalArea)
    {
        var half = totalArea / 2.0;
        var cumulative = 0L;

        foreach (var (y0, y1, width) in bands)
        {
            var remaining = half - cumulative;

            if (remaining <= 0)
            {
                return y0;
            }

            var bandArea = width * (y1 - y0);

            if (cumulative + bandArea >= half)
            {
                return y0 + remaining / width;
            }

            cumulative += bandArea;
        }

        return bands.Count > 0 ? bands[^1].Y1 : 0;
    }

    private static long CoveredXLengthByListMerge(List<Square> active)
    {
        var intervals = active
            .Select(square => (Start: square.X, End: square.X + square.L))
            .OrderBy(interval => interval.Start)
            .ToList();

        var total = 0L;
        var mergedEnd = long.MinValue;
        var mergedStart = long.MinValue;

        foreach (var (start, end) in intervals)
        {
            if (start > mergedEnd)
            {
                total += Math.Max(0, mergedEnd - mergedStart);
                mergedStart = start;
                mergedEnd = end;
            }
            else
            {
                mergedEnd = Math.Max(mergedEnd, end);
            }
        }

        total += Math.Max(0, mergedEnd - mergedStart);
        return total;
    }

    private static long CoveredXLengthByIntervalSet(List<Square> active)
    {
        var xIntervals = new IntervalSet<long>();

        foreach (var square in active)
        {
            xIntervals.Add(square.X, square.X + square.L);
        }

        var total = 0L;

        for (var i = 0; i < xIntervals.Count; i++)
        {
            var (start, end) = xIntervals.Get(i);
            total += end - start;
        }

        return total;
    }

    private static List<long> CollectSortedYBreakpoints(IReadOnlyList<Square> squares)
    {
        var breakpoints = new SortedSet<long>();

        foreach (var square in squares)
        {
            breakpoints.Add(square.Y);
            breakpoints.Add(square.Y + square.L);
        }

        return [.. breakpoints];
    }

    private static List<Square> ParseSquares(int[][] squares) =>
        squares.Select(square => new Square(square[0], square[1], square[2])).ToList();
}
