using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Closest Room (LC 1847): an O(rooms * queries) per-query linear scan
// baseline vs. this repo's MergeSort (descending sweep over both rooms and
// queries) plus BinarySearch.LowerBound over an always-sorted DynamicArray
// <int> of eligible room ids - the same insertion-point primitive Search
// Insert Position (LC 35) already proves out, reused here for floor/ceiling
// nearest-neighbor lookups instead of a single exact match.
[MemoryDiagnoser]
public class ClosestRoomBenchmarks
{
    private const int MaxRoomSizeExclusive = 4;

    // LC problem number, used as the deterministic random seed.
    private const int RandomSeed = 1847;

    private readonly record struct Room(int Id, int Size);

    private readonly record struct Query(int Preferred, int MinSize, int OriginalIndex);

    [Params(50, 400)]
    public int RoomCount;

    private int[][] _rooms = null!;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _rooms = Enumerable.Range(1, RoomCount)
            .Select(id => new[] { id, random.Next(1, RoomCount * MaxRoomSizeExclusive) })
            .ToArray();
        _queries = Enumerable.Range(0, RoomCount)
            .Select(_ => new[] { random.Next(1, RoomCount + 1), random.Next(1, RoomCount * MaxRoomSizeExclusive) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var total = 0;

        foreach (var query in _queries)
        {
            total += BestRoomIdForQuery(query);
        }

        return total;
    }

    private int BestRoomIdForQuery(int[] query)
    {
        var preferred = query[0];
        var minSize = query[1];
        var bestId = -1;
        var bestDifference = int.MaxValue;

        foreach (var room in _rooms)
        {
            if (room[1] < minSize)
            {
                continue;
            }

            var difference = Math.Abs(room[0] - preferred);

            if (difference < bestDifference || (difference == bestDifference && room[0] < bestId))
            {
                bestDifference = difference;
                bestId = room[0];
            }
        }

        return bestId;
    }

    [Benchmark]
    public int SortedSweepWithBinarySearch()
    {
        var roomEntries = BuildRoomEntriesBySizeDescending();
        var queryEntries = BuildQueryEntriesByMinSizeDescending();
        var results = SweepQueries(roomEntries, queryEntries);

        var total = 0;
        foreach (var value in results)
        {
            total += value;
        }

        return total;
    }

    private Room[] BuildRoomEntriesBySizeDescending()
    {
        var roomEntries = new Room[_rooms.Length];
        for (var i = 0; i < _rooms.Length; i++)
        {
            roomEntries[i] = new Room(_rooms[i][0], _rooms[i][1]);
        }

        MergeSort.Sort<Room, ArrayIndexedSequence<Room>>(
            new ArrayIndexedSequence<Room>(roomEntries),
            Comparer<Room>.Create((first, second) => second.Size.CompareTo(first.Size)));

        return roomEntries;
    }

    private Query[] BuildQueryEntriesByMinSizeDescending()
    {
        var queryEntries = new Query[_queries.Length];
        for (var i = 0; i < _queries.Length; i++)
        {
            queryEntries[i] = new Query(_queries[i][0], _queries[i][1], i);
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
            while (roomIndex < roomEntries.Length && roomEntries[roomIndex].Size >= query.MinSize)
            {
                InsertSorted(eligibleIds, roomEntries[roomIndex].Id);
                roomIndex++;
            }

            results[query.OriginalIndex] = ClosestEligibleId(eligibleIds, query.Preferred);
        }

        return results;
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

        return preferred - floor <= ceiling - preferred ? floor : ceiling;
    }
}
