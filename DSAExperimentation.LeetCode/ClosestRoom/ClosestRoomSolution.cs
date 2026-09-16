using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.ClosestRoom;

// LeetCode 1847. Closest Room: for each query (preferred, minSize), the id of the
// room of at least that size whose id is closest to preferred, breaking a distance
// tie towards the smaller id and reporting -1 when no room is large enough.
//
// The two strategies differ in whether the size filter is re-applied per query. The
// scan re-reads every room for every query; the sweep sorts both sides descending
// by size once, so a room becomes eligible exactly once and never leaves - which
// turns the per-query work into an insertion-point probe over the eligible ids.
internal static class ClosestRoomSolution
{
    // The textbook baseline this composition has to justify itself against: for
    // each query, walk every room, skip the ones that are too small and keep the
    // nearest id seen. O(rooms * queries) with a BCL array walk and nothing else -
    // deliberately written without this repo's sorting or searching primitives.
    public static int[] FindClosestRoomsByPerQueryScan(int[][] rooms, int[][] queries)
    {
        var results = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            results[i] = BestRoomIdForQuery(rooms, queries[i]);
        }

        return results;
    }

    private static int BestRoomIdForQuery(int[][] rooms, int[] query)
    {
        var preferred = query[0];
        var minSize = query[1];
        var bestId = LeetCodeAnswer.None;
        var bestDifference = int.MaxValue;

        foreach (var room in rooms)
        {
            if (room[1] < minSize)
            {
                continue;
            }

            var difference = Math.Abs(room[0] - preferred);

            if (IsBetterRoom(difference, bestDifference, room[0], bestId))
            {
                bestDifference = difference;
                bestId = room[0];
            }
        }

        return bestId;
    }

    // A room wins when it sits strictly nearer to the preferred id, or ties on distance
    // and carries the smaller id - LC 1847's own tie-break.
    private static bool IsBetterRoom(int difference, int bestDifference, int id, int bestId) =>
        difference < bestDifference || (difference == bestDifference && id < bestId);

    // The composed answer: sort rooms by size descending and queries by minSize
    // descending (both via this repo's MergeSort), then sweep the queries in that
    // order, inserting each newly-eligible room id into an always-sorted
    // DynamicArray<int> at its BinarySearch.LowerBound insertion point - the same
    // insertion-point primitive Search Insert Position (LC 35) already proves out.
    //
    // A query's answer is then just the closer of that insertion point's floor and
    // ceiling neighbour; on an exact distance tie the floor (always strictly less
    // than the ceiling, since ids are distinct) is the smaller id, which is exactly
    // LC 1847's own tie-break rule, so no extra tie-break logic is needed beyond
    // the "<=" comparison.
    public static int[] FindClosestRoomsBySortedSweep(int[][] rooms, int[][] queries)
    {
        var roomEntries = BuildSortedRoomEntries(rooms);
        var queryEntries = BuildSortedQueryEntries(queries);

        return SweepQueries(roomEntries, queryEntries);
    }

    private static Room[] BuildSortedRoomEntries(int[][] rooms)
    {
        var roomEntries = new Room[rooms.Length];
        for (var i = 0; i < rooms.Length; i++)
        {
            roomEntries[i] = new Room(rooms[i][0], rooms[i][1]);
        }

        MergeSort.Sort<Room, ArrayIndexedSequence<Room>>(
            new ArrayIndexedSequence<Room>(roomEntries),
            Comparer<Room>.Create((first, second) => second.Size.CompareTo(first.Size)));

        return roomEntries;
    }

    private static Query[] BuildSortedQueryEntries(int[][] queries)
    {
        var queryEntries = new Query[queries.Length];
        for (var i = 0; i < queries.Length; i++)
        {
            queryEntries[i] = new Query(queries[i][0], queries[i][1], i);
        }

        MergeSort.Sort<Query, ArrayIndexedSequence<Query>>(
            new ArrayIndexedSequence<Query>(queryEntries),
            Comparer<Query>.Create((first, second) => second.MinSize.CompareTo(first.MinSize)));

        return queryEntries;
    }

    private static int[] SweepQueries(Room[] roomEntries, Query[] queryEntries)
    {
        var eligibleIds = new DynamicArray<int>();
        var results = new int[queryEntries.Length];
        var roomIndex = 0;

        foreach (var query in queryEntries)
        {
            roomIndex = AdmitEligibleRooms(roomEntries, eligibleIds, roomIndex, query.MinSize);
            results[query.OriginalIndex] = ClosestEligibleId(eligibleIds, query.Preferred);
        }

        return results;
    }

    private static int AdmitEligibleRooms(Room[] roomEntries, DynamicArray<int> eligibleIds, int roomIndex, int minSize)
    {
        while (roomIndex < roomEntries.Length && roomEntries[roomIndex].Size >= minSize)
        {
            InsertSorted(eligibleIds, roomEntries[roomIndex].Id);
            roomIndex++;
        }

        return roomIndex;
    }

    private static void InsertSorted(DynamicArray<int> ids, int id)
    {
        var index = BinarySearch.LowerBound(new DynamicArraySequence<int>(ids), id);
        ids.Insert(index, id);
    }

    private static int ClosestEligibleId(DynamicArray<int> ids, int preferred)
    {
        if (ids.Count == 0)
        {
            return LeetCodeAnswer.None;
        }

        var index = BinarySearch.LowerBound(new DynamicArraySequence<int>(ids), preferred);

        if (IsExactMatch(ids, index, preferred))
        {
            return preferred;
        }

        return ClosestNeighborId(ids, index, preferred);
    }

    private static bool IsExactMatch(DynamicArray<int> ids, int index, int preferred)
        => index < ids.Count && ids.Get(index) == preferred;

    private static int ClosestNeighborId(DynamicArray<int> ids, int index, int preferred)
    {
        var hasFloor = index > 0;
        var hasCeiling = index < ids.Count;

        if (!hasFloor)
        {
            return ids.Get(index);
        }

        if (!hasCeiling)
        {
            return ids.Get(index - 1);
        }

        var floor = ids.Get(index - 1);
        var ceiling = ids.Get(index);
        var floorIsAtLeastAsClose = preferred - floor <= ceiling - preferred;

        return floorIsAtLeastAsClose ? floor : ceiling;
    }

    private readonly record struct Room(int Id, int Size);

    private readonly record struct Query(int Preferred, int MinSize, int OriginalIndex);
}
