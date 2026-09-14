using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ClosestRoom;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ClosestRoomSolution's, the same methods
// ClosestRoomTests proves correct - an O(rooms * queries) per-query linear scan
// against this repo's MergeSort (descending sweep over both rooms and queries)
// plus BinarySearch.LowerBound over an always-sorted DynamicArray<int> of eligible
// room ids. Both arms are handed the same jagged arrays LeetCode itself passes, so
// no hoisted overload is needed: [GlobalSetup] already builds exactly the input
// shape the measured methods take, and generating it is charged there.
[MemoryDiagnoser]
public class ClosestRoomBenchmarks
{
    private const int MaxRoomSizeExclusive = 4;

    // LC problem number, used as the deterministic random seed.
    private const int RandomSeed = 1847;

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
    public int[] PerQueryScan() => ClosestRoomSolution.FindClosestRoomsByPerQueryScan(_rooms, _queries);

    [Benchmark]
    public int[] SortedSweepWithBinarySearch() =>
        ClosestRoomSolution.FindClosestRoomsBySortedSweep(_rooms, _queries);
}
