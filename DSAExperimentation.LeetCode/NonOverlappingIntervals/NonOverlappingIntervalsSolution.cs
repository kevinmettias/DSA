using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.NonOverlappingIntervals;

// LeetCode 435. Non-overlapping Intervals: the minimum number of intervals to
// remove so the rest are pairwise non-overlapping. Unlike LC 452's
// touching-counts-as-overlap semantics, LC 435 treats touching endpoints as
// non-overlapping (Start >= previous End keeps both intervals).
internal static class NonOverlappingIntervalsSolution
{
    // The textbook answer: repeatedly find the still-active interval with the
    // smallest end, keep it, then eliminate every remaining interval it
    // strictly overlaps - O(n^2). Deliberately written without this repo's
    // primitives; it is the arm the composed solution below has to justify
    // itself against.
    public static int EraseOverlapIntervalsByBruteForce(int[][] intervals) =>
        EraseOverlapIntervalsByBruteForce(ToTuples(intervals));

    public static int EraseOverlapIntervalsByBruteForce((int Start, int End)[] intervals)
    {
        var eliminated = new bool[intervals.Length];
        var remaining = intervals.Length;
        var removedCount = 0;

        while (remaining > 0)
        {
            removedCount += EliminateRound(intervals, eliminated, ref remaining);
        }

        return removedCount;
    }

    private static int EliminateRound((int Start, int End)[] intervals, bool[] eliminated, ref int remaining)
    {
        var (minEnd, minIndex) = FindMinUneliminatedEnd(intervals, eliminated);
        eliminated[minIndex] = true;
        remaining--;

        return EliminateOverlapping(intervals, eliminated, minEnd, ref remaining);
    }

    private static (int MinEnd, int MinIndex) FindMinUneliminatedEnd((int Start, int End)[] intervals, bool[] eliminated)
    {
        var minEnd = int.MaxValue;
        var minIndex = -1;

        for (var i = 0; i < intervals.Length; i++)
        {
            if (!eliminated[i] && intervals[i].End < minEnd)
            {
                minEnd = intervals[i].End;
                minIndex = i;
            }
        }

        return (minEnd, minIndex);
    }

    private static int EliminateOverlapping((int Start, int End)[] intervals, bool[] eliminated, int minEnd, ref int remaining)
    {
        var removedCount = 0;

        for (var i = 0; i < intervals.Length; i++)
        {
            if (!eliminated[i] && intervals[i].Start < minEnd)
            {
                eliminated[i] = true;
                remaining--;
                removedCount++;
            }
        }

        return removedCount;
    }

    // This repo's own O(n log n) MergeSort over ArrayIndexedSequence to sort once
    // by end coordinate, followed by a single O(n) greedy pass: keep an interval
    // whenever its start does not precede the previously kept interval's end. The
    // removal count is simply the leftover.
    public static int EraseOverlapIntervalsBySortThenGreedy(int[][] intervals) =>
        EraseOverlapIntervalsBySortThenGreedy(ToTuples(intervals));

    public static int EraseOverlapIntervalsBySortThenGreedy((int Start, int End)[] intervals)
    {
        MergeSort.Sort<(int Start, int End), ArrayIndexedSequence<(int Start, int End)>>(
            new ArrayIndexedSequence<(int Start, int End)>(intervals),
            Comparer<(int Start, int End)>.Create((a, b) => a.End.CompareTo(b.End)));

        var kept = 1;
        var lastEnd = intervals[0].End;

        for (var i = 1; i < intervals.Length; i++)
        {
            if (intervals[i].Start >= lastEnd)
            {
                kept++;
                lastEnd = intervals[i].End;
            }
        }

        return intervals.Length - kept;
    }

    private static (int Start, int End)[] ToTuples(int[][] intervals) =>
        intervals.Select(p => (Start: p[0], End: p[1])).ToArray();
}
