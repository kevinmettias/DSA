using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.AlternatingGroupsIII;

// The bookkeeping LeetCode 3245's fast strategy needs and nothing else answers:
// tiles sit in a circle, and cutting the circle at every "bad" edge (a pair of
// equal-colored neighbors) partitions it into maximal alternating runs. A count
// query for size m is then sum(max(0, runLength - m + 1)) over every run - which
// only needs, per run length bucket, how many runs have that length and how much
// total length they hold. A repaint touches at most two edges, each of which
// either splits one run into two or merges two runs into one.
//
// Composed entirely from this repo's own primitives: DynamicArray<int> keeps the
// bad edges sorted (the same "sorted DynamicArray + BinarySearch.LowerBound"
// shape DataStructures.IntervalSet already uses, down to reusing
// BinarySearch.LowerBound itself for insertion-point lookup instead of a second
// bisection loop), and two point-update FenwickTree<Element, SumOperation<Element>>
// instances - one counting runs per length, one summing lengths per length -
// answer a size query with two prefix lookups instead of ever walking the runs.
internal sealed class AlternatingRunLedger
{
    private readonly int[] _tiles;
    private readonly int _tileCount;
    private readonly DynamicArray<int> _badEdges = new();
    private readonly FenwickTree<int, SumOperation<int>> _runsByLength;
    private readonly FenwickTree<long, SumOperation<long>> _lengthSumByLength;

    public AlternatingRunLedger(int[] colors)
    {
        _tiles = (int[])colors.Clone();
        _tileCount = _tiles.Length;
        _runsByLength = new FenwickTree<int, SumOperation<int>>(_tileCount);
        _lengthSumByLength = new FenwickTree<long, SumOperation<long>>(_tileCount);

        for (var edge = 0; edge < _tileCount; edge++)
        {
            if (IsBad(edge))
            {
                _badEdges.Add(edge);
            }
        }

        var wallCount = _badEdges.Count;

        for (var i = 0; i < wallCount; i++)
        {
            var from = _badEdges.Get(i);
            var to = _badEdges.Get((i + 1) % wallCount);
            AddRun(CircularGap(from, to));
        }
    }

    // Number of size-length circular windows of tiles that alternate throughout.
    public int CountWindows(int size)
    {
        if (_badEdges.Count == 0)
        {
            // No wall anywhere: the whole circle alternates, so every one of the
            // tileCount starting positions gives a valid window (LeetCode caps
            // size at tileCount - 1, so a window never has to wrap past its own
            // start).
            return _tileCount;
        }

        var lastIndex = _tileCount - 1;
        var belowLengthIndex = Math.Min(size - 2, lastIndex);

        var totalRuns = _runsByLength.PrefixQuery(lastIndex);
        var totalLength = _lengthSumByLength.PrefixQuery(lastIndex);
        var runsBelow = belowLengthIndex >= 0 ? _runsByLength.PrefixQuery(belowLengthIndex) : 0;
        var lengthBelow = belowLengthIndex >= 0 ? _lengthSumByLength.PrefixQuery(belowLengthIndex) : 0;

        var runsAtLeast = totalRuns - runsBelow;
        var lengthAtLeast = totalLength - lengthBelow;

        return (int)(lengthAtLeast - (long)(size - 1) * runsAtLeast);
    }

    public void Repaint(int index, int color)
    {
        if (_tiles[index] == color)
        {
            return;
        }

        _tiles[index] = color;

        RefreshEdge((index - 1 + _tileCount) % _tileCount);
        RefreshEdge(index);
    }

    // Edge i sits between tile i and tile (i+1) mod tileCount.
    private bool IsBad(int edge) => _tiles[edge] == _tiles[(edge + 1) % _tileCount];

    private void RefreshEdge(int edge)
    {
        var isBadNow = IsBad(edge);
        var insertionIndex = LocateInsertionIndex(edge);
        var wasBad = insertionIndex < _badEdges.Count && _badEdges.Get(insertionIndex) == edge;

        if (isBadNow == wasBad)
        {
            return;
        }

        if (isBadNow)
        {
            InsertBadEdge(edge, insertionIndex);
        }
        else
        {
            RemoveBadEdge(edge, insertionIndex);
        }
    }

    // Inserting a new wall splits the one run it used to sit inside into two -
    // except when there was no wall at all, in which case the whole circle
    // (an unstored, implicit run of length tileCount) becomes that split's input.
    private void InsertBadEdge(int edge, int insertionIndex)
    {
        var wallCountBefore = _badEdges.Count;

        if (wallCountBefore == 0)
        {
            AddRun(_tileCount);
        }
        else
        {
            var predecessor = _badEdges.Get((insertionIndex - 1 + wallCountBefore) % wallCountBefore);
            var successor = _badEdges.Get(insertionIndex % wallCountBefore);

            RemoveRun(CircularGap(predecessor, successor));
            AddRun(CircularGap(predecessor, edge));
            AddRun(CircularGap(edge, successor));
        }

        _badEdges.Insert(insertionIndex, edge);
    }

    // Removing a wall merges the run ending at it with the run starting after it.
    // When edge was the only wall, predecessor and successor both resolve to edge
    // itself, CircularGap(edge, edge) reports the full circle for both of the
    // "removed" runs and the merged one, and the three point updates net to
    // exactly the single stored run (length tileCount) disappearing - the empty-
    // wall-set state CountWindows already special-cases, so no extra branch is
    // needed here.
    private void RemoveBadEdge(int edge, int edgeIndex)
    {
        var wallCount = _badEdges.Count;
        var predecessor = _badEdges.Get((edgeIndex - 1 + wallCount) % wallCount);
        var successor = _badEdges.Get((edgeIndex + 1) % wallCount);

        RemoveRun(CircularGap(predecessor, edge));
        RemoveRun(CircularGap(edge, successor));
        AddRun(CircularGap(predecessor, successor));

        _badEdges.RemoveAt(edgeIndex);
    }

    private void AddRun(int length)
    {
        _runsByLength.Add(length - 1, 1);
        _lengthSumByLength.Add(length - 1, length);
    }

    private void RemoveRun(int length)
    {
        _runsByLength.Add(length - 1, -1);
        _lengthSumByLength.Add(length - 1, -(long)length);
    }

    // Tiles from just after `from` through `to`, going clockwise. Equal
    // endpoints (the only-wall case) mean "all the way around" - tileCount, not
    // zero.
    private int CircularGap(int from, int to)
    {
        var gap = ((to - from) % _tileCount + _tileCount) % _tileCount;

        return gap == 0 ? _tileCount : gap;
    }

    private int LocateInsertionIndex(int edge) =>
        BinarySearch.LowerBound<int, BadEdgeView>(new BadEdgeView(_badEdges), edge);

    private readonly struct BadEdgeView(DynamicArray<int> badEdges) : IRandomAccessSequence<int>
    {
        public int Length => badEdges.Count;

        public int Get(int index) => badEdges.Get(index);
    }
}
