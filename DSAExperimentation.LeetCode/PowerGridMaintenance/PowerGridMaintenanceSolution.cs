using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.PowerGridMaintenance;

// LeetCode 3607. Power Grid Maintenance: connections[] group stations into grids
// (connected components); a type-2 query takes a station offline permanently, and a
// type-1 query on an offline station resolves to the smallest-id station still
// operational in its grid, or -1 if none remain. Each grid needs "smallest still-
// operational id" queryable as stations go offline one-way, so this reduces to a
// per-grid ordered structure grouped by DisjointSet.
//
// Both strategies group stations with DisjointSet, differing only in the ordered
// structure each grid keeps: a BCL SortedSet<int> pruned by direct removal
// (baseline), or this repo's own Heap<int, MinHeapOrder<int>> with lazy deletion - an
// offline station is popped the first time it would surface as an answer, since it
// can never come back online, but an operational one is only ever peeked.
internal static class PowerGridMaintenanceSolution
{
    // The textbook answer: DisjointSet grouping plus a plain BCL SortedSet<int> per
    // grid, removed from directly as stations go offline. Deliberately written
    // without this repo's Heap - the arm the composed solution below has to justify
    // itself against.
    public static int[] MaintenanceResultsByUnionFindSortedSet(int stationCount, int[][] connections, int[][] queries)
    {
        var disjointSet = new DisjointSet(stationCount + 1);

        foreach (var connection in connections)
        {
            disjointSet.Union(connection[0], connection[1]);
        }

        return MaintenanceResultsByUnionFindSortedSet(stationCount, disjointSet, queries);
    }

    public static int[] MaintenanceResultsByUnionFindSortedSet(int stationCount, DisjointSet disjointSet, int[][] queries)
    {
        var grids = BuildSortedSetGrids(stationCount, disjointSet);
        var offline = new bool[stationCount + 1];
        var results = new List<int>();

        foreach (var query in queries)
        {
            if (query[0] == 2)
            {
                TakeOfflineSortedSet(grids, disjointSet, offline, query[1]);
            }
            else
            {
                var answer = AnswerSortedSetQuery(grids, disjointSet, offline, query);
                results.Add(answer);
            }
        }

        return [.. results];
    }

    // Groups every station under its grid's root, each grid keeping its station ids
    // ordered so its smallest still-operational one is a Min lookup.
    private static SortedSet<int>[] BuildSortedSetGrids(int stationCount, DisjointSet disjointSet)
    {
        var grids = new SortedSet<int>[stationCount + 1];

        for (var station = 1; station <= stationCount; station++)
        {
            var root = disjointSet.Find(station);
            (grids[root] ??= []).Add(station);
        }

        return grids;
    }

    // Type 2: stationId goes offline for good, so it leaves its grid's ordered set
    // rather than lingering there as a stale entry.
    private static void TakeOfflineSortedSet(
        SortedSet<int>[] grids, DisjointSet disjointSet, bool[] offline, int stationId)
    {
        offline[stationId] = true;
        grids[disjointSet.Find(stationId)]!.Remove(stationId);
    }

    // Type 1: the station's own id while it is still operational, otherwise the smallest
    // id its grid still has, or the "none remain" sentinel once that grid is empty.
    private static int AnswerSortedSetQuery(
        SortedSet<int>[] grids, DisjointSet disjointSet, bool[] offline, int[] query)
    {
        var stationId = query[1];

        if (!offline[stationId])
        {
            return stationId;
        }

        var grid = grids[disjointSet.Find(stationId)]!;
        return grid.Count > 0 ? grid.Min : LeetCodeAnswer.None;
    }

    // This repo's own DisjointSet groups stations into grids; each grid's operational
    // ids live in a Heap<int, MinHeapOrder<int>>, popped lazily - a malfunctioning
    // station discovered at the root is discarded for good, since it can never
    // return, but a still-operational one is left in place for the next query.
    public static int[] MaintenanceResultsByUnionFindHeap(int stationCount, int[][] connections, int[][] queries)
    {
        var disjointSet = new DisjointSet(stationCount + 1);

        foreach (var connection in connections)
        {
            disjointSet.Union(connection[0], connection[1]);
        }

        return MaintenanceResultsByUnionFindHeap(stationCount, disjointSet, queries);
    }

    public static int[] MaintenanceResultsByUnionFindHeap(int stationCount, DisjointSet disjointSet, int[][] queries)
    {
        var grids = BuildHeapGrids(stationCount, disjointSet);
        var offline = new bool[stationCount + 1];
        var results = new List<int>();

        foreach (var query in queries)
        {
            if (query[0] == 2)
            {
                offline[query[1]] = true;
            }
            else
            {
                var answer = AnswerHeapQuery(grids, disjointSet, offline, query);
                results.Add(answer);
            }
        }

        return [.. results];
    }

    // Groups every station under its grid's root, each grid keeping a min-heap of its
    // station ids for lazy deletion.
    private static Heap<int, MinHeapOrder<int>>[] BuildHeapGrids(int stationCount, DisjointSet disjointSet)
    {
        var grids = new Heap<int, MinHeapOrder<int>>[stationCount + 1];

        for (var station = 1; station <= stationCount; station++)
        {
            var root = disjointSet.Find(station);
            (grids[root] ??= new()).Push(station);
        }

        return grids;
    }

    // Type 1: the station's own id while it is still operational, otherwise pop every
    // stale id off the front of its grid until an operational one surfaces - the popped
    // ones are discarded for good, since a station that went offline never returns - or
    // report the sentinel once that grid runs out.
    private static int AnswerHeapQuery(
        Heap<int, MinHeapOrder<int>>[] grids, DisjointSet disjointSet, bool[] offline, int[] query)
    {
        var stationId = query[1];

        if (!offline[stationId])
        {
            return stationId;
        }

        var grid = grids[disjointSet.Find(stationId)]!;

        while (grid.TryPeek(out var candidate) && offline[candidate])
        {
            grid.TryPop(out _);
        }

        return grid.TryPeek(out var operational) ? operational : LeetCodeAnswer.None;
    }
}
