using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MeetingRoomsIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MeetingRoomsIIISolution's, the same methods
// MeetingRoomsIIITests proves correct - the textbook O(meetings * rooms) freeAt[]
// scan against this repo's own free-room + busy-room heap pair. The meeting list is
// generated wide enough relative to RoomCount (MeetingsPerRoom arrivals, durations up
// to 3x RoomCount) that rooms routinely contend and delay, forcing real pool churn
// instead of every meeting finding an immediately free room; [GlobalSetup] owns that
// construction.
[MemoryDiagnoser]
public class MeetingRoomsIIIBenchmarks
{
    private const int MeetingsPerRoom = 50;
    private const int MaxDurationMultiplier = 3;
    private const int RandomSeed = 1;

    [Params(20, 80)]
    public int RoomCount;

    private int[][] _meetings = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var meetingCount = RoomCount * MeetingsPerRoom;
        _meetings = new int[meetingCount][];

        for (var i = 0; i < meetingCount; i++)
        {
            _meetings[i] = [i, i + random.Next(1, (RoomCount * MaxDurationMultiplier) + 1)];
        }
    }

    [Benchmark(Baseline = true)]
    public int LinearScanFreeAt() => MeetingRoomsIIISolution.MostBookedByLinearScanFreeAt(RoomCount, _meetings);

    [Benchmark]
    public int TwoHeapPool() => MeetingRoomsIIISolution.MostBookedByTwoHeapPool(RoomCount, _meetings);
}
