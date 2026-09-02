using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ClosestRoom;

// LeetCode 1847. Closest Room: sort rooms by size descending and queries by
// minSize descending (both via this repo's MergeSort), then sweep queries in
// that order, inserting each newly-eligible room id into an always-sorted
// DynamicArray<int> at its BinarySearch.LowerBound insertion point - the same
// insertion-point primitive Search Insert Position (LC 35) already proves out.
// A query's answer is then just the closer of that insertion point's floor
// and ceiling neighbor; on an exact distance tie the floor (which is always
// strictly less than the ceiling, since ids are distinct) is the smaller id,
// which is exactly LC 1847's own tie-break rule, so no extra tie-break logic
// is needed beyond the "<=" comparison.
public sealed partial class ClosestRoomTests
{
    private readonly record struct Room(int Id, int Size);

    private readonly record struct Query(int Preferred, int MinSize, int OriginalIndex);

    [Fact]
    public void FindClosestRooms_LeetCodeExample1_ReturnsExpectedRoomIds()
    {
        int[][] rooms = [[2, 2], [1, 2], [3, 2]];
        int[][] queries = [[3, 1], [3, 3], [5, 2]];

        var result = FindClosestRooms(rooms, queries);

        Assert.Equal([3, -1, 3], result);
    }

    [Fact]
    public void FindClosestRooms_LeetCodeExample2_ReturnsExpectedRoomIds()
    {
        int[][] rooms = [[1, 4], [2, 3], [3, 5], [4, 1], [5, 2]];
        int[][] queries = [[2, 3], [2, 4], [2, 5]];

        var result = FindClosestRooms(rooms, queries);

        Assert.Equal([2, 1, 3], result);
    }

    private static int[] FindClosestRooms(int[][] rooms, int[][] queries)
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
            return -1;
        }

        var index = BinarySearch.LowerBound(new DynamicArraySequence<int>(ids), preferred);

        if (index < ids.Count && ids.Get(index) == preferred)
        {
            return preferred;
        }

        return ClosestNeighborId(ids, index, preferred);
    }

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

        return preferred - floor <= ceiling - preferred ? floor : ceiling;
    }
}
