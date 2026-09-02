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
    public static int[] MaintenanceResultsByUnionFindSortedSet(int c, int[][] connections, int[][] queries)
    {
        var disjointSet = new DisjointSet(c + 1);

        foreach (var connection in connections)
        {
            disjointSet.Union(connection[0], connection[1]);
        }

        return MaintenanceResultsByUnionFindSortedSet(c, disjointSet, queries);
    }

    public static int[] MaintenanceResultsByUnionFindSortedSet(int c, DisjointSet disjointSet, int[][] queries)
    {
        var grids = new SortedSet<int>[c + 1];

        for (var station = 1; station <= c; station++)
        {
            var root = disjointSet.Find(station);
            (grids[root] ??= []).Add(station);
        }

        var offline = new bool[c + 1];
        var results = new List<int>();

        foreach (var query in queries)
        {
            var (type, x) = (query[0], query[1]);

            if (type == 2)
            {
                offline[x] = true;
                grids[disjointSet.Find(x)]!.Remove(x);
                continue;
            }

            if (!offline[x])
            {
                results.Add(x);
                continue;
            }

            var grid = grids[disjointSet.Find(x)]!;
            results.Add(grid.Count > 0 ? grid.Min : LeetCodeAnswer.None);
        }

        return [.. results];
    }

    // This repo's own DisjointSet groups stations into grids; each grid's operational
    // ids live in a Heap<int, MinHeapOrder<int>>, popped lazily - a malfunctioning
    // station discovered at the root is discarded for good, since it can never
    // return, but a still-operational one is left in place for the next query.
    public static int[] MaintenanceResultsByUnionFindHeap(int c, int[][] connections, int[][] queries)
    {
        var disjointSet = new DisjointSet(c + 1);

        foreach (var connection in connections)
        {
            disjointSet.Union(connection[0], connection[1]);
        }

        return MaintenanceResultsByUnionFindHeap(c, disjointSet, queries);
    }

    public static int[] MaintenanceResultsByUnionFindHeap(int c, DisjointSet disjointSet, int[][] queries)
    {
        var grids = new Heap<int, MinHeapOrder<int>>[c + 1];

        for (var station = 1; station <= c; station++)
        {
            var root = disjointSet.Find(station);
            (grids[root] ??= new()).Push(station);
        }

        var offline = new bool[c + 1];
        var results = new List<int>();

        foreach (var query in queries)
        {
            var (type, x) = (query[0], query[1]);

            if (type == 2)
            {
                offline[x] = true;
                continue;
            }

            if (!offline[x])
            {
                results.Add(x);
                continue;
            }

            var grid = grids[disjointSet.Find(x)]!;

            while (grid.TryPeek(out var candidate) && offline[candidate])
            {
                grid.TryPop(out _);
            }

            results.Add(grid.TryPeek(out var operational) ? operational : LeetCodeAnswer.None);
        }

        return [.. results];
    }
}
