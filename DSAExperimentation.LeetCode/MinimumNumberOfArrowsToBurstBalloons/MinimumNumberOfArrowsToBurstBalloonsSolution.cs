using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MinimumNumberOfArrowsToBurstBalloons;

// LeetCode 452. Minimum Number of Arrows to Burst Balloons: fewest points needed so
// every balloon's [start, end] interval contains at least one of them.
//
// FindMinArrowShotsByBruteForceRescan is the textbook O(n^2) baseline: repeatedly
// rescan every unburst balloon for the minimum end, then burst everything that
// reaches it. FindMinArrowShotsBySortEndsThenGreedyScan sorts once by end with this
// repo's own MergeSort.Sort<Element,TSequence> over an ArrayIndexedSequence - the
// same custom-comparer shape QueueReconstructionByHeightTests exercises - then makes
// a single O(n) greedy pass: an arrow placed at the end of the earliest-ending
// unburst balloon always bursts the largest possible set of remaining balloons, so
// sorting once is enough. This is NOT IntervalSet.Count: transitively-merged overlap
// groups can still need more than one stabbing point, e.g. [1,2],[2,3],[3,4] merge
// into one interval but need two arrows.
internal static class MinimumNumberOfArrowsToBurstBalloonsSolution
{
    // LeetCode's own shape.
    public static int FindMinArrowShotsByBruteForceRescan(int[][] points) =>
        FindMinArrowShotsByBruteForceRescan(ToPairs(points));

    // Deliberately written without this repo's primitives - the baseline the sorted
    // greedy scan below has to justify itself against.
    public static int FindMinArrowShotsByBruteForceRescan((int Start, int End)[] points)
    {
        var burst = new bool[points.Length];
        var remaining = points.Length;
        var arrows = 0;

        while (remaining > 0)
        {
            remaining = FireArrow(points, burst, remaining);
            arrows++;
        }

        return arrows;
    }

    private static int FireArrow((int Start, int End)[] points, bool[] burst, int remaining)
    {
        var minEnd = int.MaxValue;

        for (var i = 0; i < points.Length; i++)
        {
            if (!burst[i] && points[i].End < minEnd)
            {
                minEnd = points[i].End;
            }
        }

        for (var i = 0; i < points.Length; i++)
        {
            if (!burst[i] && points[i].Start <= minEnd && minEnd <= points[i].End)
            {
                burst[i] = true;
                remaining--;
            }
        }

        return remaining;
    }

    // LeetCode's own shape.
    public static int FindMinArrowShotsBySortEndsThenGreedyScan(int[][] points) =>
        FindMinArrowShotsBySortEndsThenGreedyScan(ToPairs(points));

    // Prepared-input overload: takes the (Start, End) pairs a benchmark's
    // [GlobalSetup] already generated in that shape, so the points[]-of-points[]
    // unpack isn't charged to the measured method.
    public static int FindMinArrowShotsBySortEndsThenGreedyScan((int Start, int End)[] points)
    {
        var items = ((int Start, int End)[])points.Clone();

        MergeSort.Sort<(int Start, int End), ArrayIndexedSequence<(int Start, int End)>>(
            new ArrayIndexedSequence<(int Start, int End)>(items),
            Comparer<(int Start, int End)>.Create((a, b) => a.End.CompareTo(b.End)));

        var arrows = 1;
        var arrowPosition = items[0].End;

        for (var i = 1; i < items.Length; i++)
        {
            if (items[i].Start > arrowPosition)
            {
                arrows++;
                arrowPosition = items[i].End;
            }
        }

        return arrows;
    }

    private static (int Start, int End)[] ToPairs(int[][] points) =>
        points.Select(p => (Start: p[0], End: p[1])).ToArray();
}
