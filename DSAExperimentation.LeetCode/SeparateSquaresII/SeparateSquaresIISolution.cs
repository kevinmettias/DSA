using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.LeetCode.SeparateSquaresII;

// LC 3454: squares may overlap, so the union of their areas (not the sum) is what
// has to be split evenly by a horizontal line. Both strategies are the same sweep:
// walk the squares' y-coordinates as band boundaries, and within each band the
// union of squares' area is (covered x-length) * (band height) since every square
// active in a band spans it fully. The two strategies differ only in how the
// covered x-length of a band's active squares is computed - that one step is
// pulled out behind a strategy type so the sweep bookkeeping itself (which this
// problem contributes, not either primitive) is written once.
internal static class SeparateSquaresIISolution
{
    // Both coverage arms are stateless, so one instance each serves every call and the
    // benchmark arms that measure these two sweeps allocate nothing to pick one.
    private static readonly ICoveredXLength ListMergeCoverage = new CoveredXLengthByListMerge();
    private static readonly ICoveredXLength IntervalSetCoverage = new CoveredXLengthByIntervalSet();

    // The textbook arm: covered x-length via a hand-sorted, hand-merged BCL List -
    // deliberately not this repo's own IntervalSet, so it is what the composed arm
    // below has to justify itself against.
    public static double MinYByEventSweep(int[][] squares) => MinYByEventSweep(ParseSquares(squares));

    public static double MinYByEventSweep(IReadOnlyList<Square> squares) =>
        Sweep(squares, ListMergeCoverage);

    // This repo's own IntervalSet<long> already IS "add an interval, keep the set
    // merged" - covered x-length is just the sum of its merged intervals' lengths,
    // no hand-written merge loop needed.
    public static double MinYByIntervalSetSweep(int[][] squares) => MinYByIntervalSetSweep(ParseSquares(squares));

    public static double MinYByIntervalSetSweep(IReadOnlyList<Square> squares) =>
        Sweep(squares, IntervalSetCoverage);

    // The one question the two arms answer differently: how much of the band's x-axis
    // its active squares cover between them. Both of its inputs are named here, and what
    // the answer is NOT - the sum of the squares' own widths - has somewhere to be
    // stated: squares that overlap x-wise must contribute their union once, not twice.
    private interface ICoveredXLength
    {
        long CoveredBy(List<Square> active);
    }

    private static double Sweep(IReadOnlyList<Square> squares, ICoveredXLength coveredXLength)
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

            var width = coveredXLength.CoveredBy(active);
            bands.Add((y0, y1, width));
            totalArea += width * (y1 - y0);
            RemoveEndingSquares(active, removeAt[y1]);
        }

        return FindSplitLine(bands, totalArea);
    }

    // A square stops contributing to every band at or above the y it ends on, so
    // each band closes by retiring the squares that end at its upper boundary.
    private static void RemoveEndingSquares(List<Square> active, IEnumerable<Square> ending)
    {
        foreach (var square in ending)
        {
            active.Remove(square);
        }
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

        foreach (var band in bands)
        {
            var split = SplitWithinBand(band, half - cumulative);

            if (split.HasValue)
            {
                return split.Value;
            }

            cumulative += band.Width * (band.Y1 - band.Y0);
        }

        return bands.Count > 0 ? bands[^1].Y1 : 0;
    }

    // The split line inside one band, or null when the running total has not yet
    // reached half and the band is merely consumed whole.
    private static double? SplitWithinBand((long Y0, long Y1, long Width) band, double remaining)
    {
        if (remaining <= 0)
        {
            return band.Y0;
        }

        var bandArea = band.Width * (band.Y1 - band.Y0);

        if (bandArea >= remaining)
        {
            return band.Y0 + (remaining / band.Width);
        }

        return null;
    }

    private sealed class CoveredXLengthByListMerge : ICoveredXLength
    {
        public long CoveredBy(List<Square> active)
        {
            var intervals = active
                .Select(square => (Start: square.X, End: square.X + square.L))
                .OrderBy(interval => interval.Start)
                .ToList();

            return MergedLengthOf(intervals);
        }
    }

    // The total length of the merged intervals: the running [mergedStart, mergedEnd)
    // span is closed and counted whenever the next interval starts past its end.
    private static long MergedLengthOf(List<(long Start, long End)> intervals)
    {
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

    private sealed class CoveredXLengthByIntervalSet : ICoveredXLength
    {
        public long CoveredBy(List<Square> active)
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
