using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.BlockPlacementQueries;

// LeetCode 3161. Block Placement Queries: a number line starting with a
// permanent obstacle at 0. queries[i] = [1, x] places another obstacle at x;
// [2, x, sz] asks whether a size-sz block fits anywhere inside [0, x] without
// crossing an obstacle (touching is fine), against only the obstacles placed
// by *earlier* queries.
internal static class BlockPlacementQueriesSolution
{
    private const int PlaceObstacle = 1;

    // Baseline: obstacles kept in a plain sorted BCL List<long>, rescanned
    // linearly for every type-2 query straight off the problem statement -
    // deliberately no repo primitive, O(m) per query, O(n*m) overall.
    public static bool[] CanPlaceByLinearScan(int[][] queries)
    {
        var obstacles = new List<long> { 0 };
        var results = new List<bool>();

        foreach (var query in queries)
        {
            if (query[0] == PlaceObstacle)
            {
                InsertSorted(obstacles, query[1]);
            }
            else
            {
                var fits = CanPlace(obstacles, query[1], query[2]);
                results.Add(fits);
            }
        }

        return [.. results];
    }

    private static void InsertSorted(List<long> obstacles, long position)
    {
        var index = obstacles.BinarySearch(position);
        obstacles.Insert(~index, position);
    }

    private static bool CanPlace(List<long> obstacles, long x, long size)
    {
        var maxGap = 0L;

        for (var i = 0; i < obstacles.Count && obstacles[i] <= x; i++)
        {
            var nextIsWithinRange = i + 1 < obstacles.Count && obstacles[i + 1] <= x;
            var rightBound = nextIsWithinRange ? NextObstaclePosition(obstacles, i) : x;
            maxGap = Math.Max(maxGap, rightBound - obstacles[i]);
        }

        return size <= maxGap;
    }

    // The obstacle following index i, read only where the caller has already
    // established that one exists at or before x.
    private static long NextObstaclePosition(List<long> obstacles, int i) => obstacles[i + 1];

    // This repo's own point-update SegmentTree<long, MaxOperation<long>> for
    // "the largest gap ending at or before a coordinate", plus this problem's
    // own NearestActiveObstacle (see its own doc comment) for "nearest
    // still-active obstacle" - both driven by walking the queries in reverse,
    // so every type-1 "placement" becomes a *deactivation*: the reverse-time
    // offline trick this problem needs because obstacles are only ever added
    // going forward, and DisjointSetForest's near-O(1) amortized Find only
    // pays off for a structure that only ever shrinks.
    //
    // Every type-2 query resolves to two pieces once its "nearest active
    // obstacle at or before x" (lastActive) is known: the trailing gap from
    // lastActive up to x itself (x - lastActive), and the best gap strictly
    // between two active obstacles at or before lastActive (a segment-tree
    // prefix-max query, indexed by each gap's own right-hand obstacle).
    public static bool[] CanPlaceBySegmentTreeMerge(int[][] queries)
    {
        var maxCoordinate = MaxCoordinate(queries);
        var isFinalObstacle = ObstaclePresence(queries, maxCoordinate);
        var gapTree = BuildGapTree(isFinalObstacle, maxCoordinate);
        var nearest = new NearestActiveObstacle(isFinalObstacle, maxCoordinate);
        var results = new bool[queries.Length];

        for (var t = queries.Length - 1; t >= 0; t--)
        {
            var query = queries[t];

            if (query[0] == PlaceObstacle)
            {
                Deactivate(gapTree, nearest, query[1]);
            }
            else
            {
                results[t] = CanPlaceAmongActive(nearest, gapTree, (query[1], query[2]));
            }
        }

        return ExtractTypeTwoResults(queries, results);
    }

    // The highest coordinate any query names - the size of every coordinate-indexed
    // array the reverse walk builds.
    private static int MaxCoordinate(int[][] queries)
    {
        var maxCoordinate = 0;

        foreach (var query in queries)
        {
            maxCoordinate = Math.Max(maxCoordinate, query[1]);
        }

        return maxCoordinate;
    }

    // Which coordinates hold an obstacle once every type-1 query has been applied,
    // with the permanent obstacle at 0 - the "final" picture the reverse walk starts
    // from and removes obstacles out of.
    private static bool[] ObstaclePresence(int[][] queries, int maxCoordinate)
    {
        var isFinalObstacle = new bool[maxCoordinate + 1];
        isFinalObstacle[0] = true;

        foreach (var query in queries)
        {
            if (query[0] == PlaceObstacle)
            {
                isFinalObstacle[query[1]] = true;
            }
        }

        return isFinalObstacle;
    }

    // The gap ending at each obstacle, indexed by that obstacle's own coordinate -
    // the array the prefix-max segment tree is built over.
    private static SegmentTree<long, MaxOperation<long>> BuildGapTree(
        bool[] isFinalObstacle, int maxCoordinate)
    {
        var gaps = new long[maxCoordinate + 1];
        Array.Fill(gaps, MaxOperation<long>.Identity);

        var previousObstacle = 0;
        for (var coordinate = 1; coordinate <= maxCoordinate; coordinate++)
        {
            if (isFinalObstacle[coordinate])
            {
                gaps[coordinate] = coordinate - previousObstacle;
                previousObstacle = coordinate;
            }
        }

        return new SegmentTree<long, MaxOperation<long>>(gaps);
    }

    // A type-2 query answered against the obstacles still active at this point in the
    // reverse walk: the block fits either in the trailing gap running up to x, or in
    // the largest gap ending at or before the nearest active obstacle.
    private static bool CanPlaceAmongActive(
        NearestActiveObstacle nearest,
        SegmentTree<long, MaxOperation<long>> gapTree,
        (int X, int Size) query)
    {
        var x = query.X;
        var lastActive = nearest.NearestAtOrBefore(x);
        var trailingGap = x - lastActive;
        var interiorMax = gapTree.Query(0, lastActive);

        return query.Size <= Math.Max(trailingGap, interiorMax);
    }

    private static bool[] ExtractTypeTwoResults(int[][] queries, bool[] results)
    {
        var extracted = new List<bool>();

        for (var i = 0; i < queries.Length; i++)
        {
            if (queries[i][0] != PlaceObstacle)
            {
                extracted.Add(results[i]);
            }
        }

        return [.. extracted];
    }

    private static void Deactivate(
        SegmentTree<long, MaxOperation<long>> gapTree, NearestActiveObstacle nearest, int position)
    {
        if (nearest.TryNearestAtOrAfter(position + 1, out var rightNeighbor))
        {
            var leftNeighbor = nearest.NearestAtOrBefore(position - 1);
            gapTree.Update(rightNeighbor, rightNeighbor - leftNeighbor);
        }

        gapTree.Update(position, MaxOperation<long>.Identity);
        nearest.Deactivate(position);
    }
}
