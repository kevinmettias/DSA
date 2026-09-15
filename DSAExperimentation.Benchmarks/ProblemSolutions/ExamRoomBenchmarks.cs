using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ExamRoom;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ExamRoomSolution's, the same strategies ExamRoomTests
// proves correct. Both share the identical Seat() gap scan, so what is being compared
// is Leave(p): the baseline's O(n) List<T>.Remove scan versus this repo's O(log n)
// BinarySearch.LowerBound locate. Drain seats Length students, then has every one of
// them leave in the order they arrived, so both arms run the same call script. The
// room itself is stateful and must be rebuilt per invocation, so [GlobalSetup] only
// fixes the workload SIZE (ARCHITECTURE.md §17.7).
[MemoryDiagnoser]
public class ExamRoomBenchmarks
{
    // Extra room capacity appended beyond Length so the end-of-room gap is never the
    // tightest constraint by construction.
    private const int RoomCapacityPadding = 1_000;

    private int _seatCount;

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _seatCount = Length + RoomCapacityPadding;

    [Benchmark(Baseline = true)]
    public int LinearScanList() => Drain(ExamRoomSolution.CreateByLinearScanList(_seatCount));

    [Benchmark]
    public int BinarySearchDynamicArray()
        => Drain(ExamRoomSolution.CreateByBinarySearchDynamicArray(_seatCount));

    private int Drain(ExamRoomSolution.IExamRoom room)
    {
        var assigned = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            assigned[i] = room.Seat();
        }

        foreach (var seat in assigned)
        {
            room.Leave(seat);
        }

        return assigned[Length - 1];
    }
}
