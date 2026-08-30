using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Exam Room (LC 855): both variants share the exact same BestAvailableSeat scan
// - a single pass over the occupied seats that finds the widest min-distance
// gap, which already yields the insertion index for free, so Seat() itself is
// unaffected by which primitive backs the room. Leave(p) is where they differ:
// LinearScanList keeps occupied seats in a plain, sorted List<int> and locates
// p via List<T>.Remove's own linear scan (MyCalendarIBenchmarks' LinearScan
// precedent), O(n) per call. BinarySearchDynamicArray instead composes this
// repo's own DynamicArray<int> + BinarySearch.LowerBound (IntervalSet/
// MyCalendarI precedent), O(log n) to locate p before the same O(n) shift
// every array-backed removal pays either way. Every seated student later
// leaves in the arrival order they sat, so both variants run the same number
// of Seat()/Leave() calls doing comparable total work.
[MemoryDiagnoser]
public class ExamRoomBenchmarks
{
    [Params(200, 2_000)]
    public int Length;

    private int _seatCount;

    [GlobalSetup]
    public void Setup() => _seatCount = Length + 1_000;

    [Benchmark(Baseline = true)]
    public int LinearScanList()
    {
        var occupied = new List<int>();
        var assigned = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            assigned[i] = SeatLinear(occupied);
        }

        foreach (var seat in assigned)
        {
            occupied.Remove(seat);
        }

        return occupied.Count;
    }

    [Benchmark]
    public int BinarySearchDynamicArray()
    {
        var occupied = new DynamicArray<int>();
        var assigned = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            assigned[i] = SeatSorted(occupied);
        }

        foreach (var seat in assigned)
        {
            var index = BinarySearch.LowerBound<int, DynamicArraySequence<int>>(new DynamicArraySequence<int>(occupied), seat);
            occupied.RemoveAt(index);
        }

        return occupied.Count;
    }

    private int SeatLinear(List<int> occupied)
    {
        if (occupied.Count == 0)
        {
            occupied.Insert(0, 0);
            return 0;
        }

        var (bestIndex, bestSeat) = BestAvailableSeat(occupied.Count, i => occupied[i]);
        occupied.Insert(bestIndex, bestSeat);
        return bestSeat;
    }

    private int SeatSorted(DynamicArray<int> occupied)
    {
        if (occupied.Count == 0)
        {
            occupied.Insert(0, 0);
            return 0;
        }

        var (bestIndex, bestSeat) = BestAvailableSeat(occupied.Count, occupied.Get);
        occupied.Insert(bestIndex, bestSeat);
        return bestSeat;
    }

    private (int Index, int Seat) BestAvailableSeat(int count, Func<int, int> get)
    {
        var bestIndex = 0;
        var bestSeat = 0;
        var bestDistance = get(0);

        for (var i = 0; i < count - 1; i++)
        {
            var left = get(i);
            var right = get(i + 1);
            var candidate = left + ((right - left) / 2);
            var distance = candidate - left;

            if (distance > bestDistance)
            {
                bestDistance = distance;
                bestSeat = candidate;
                bestIndex = i + 1;
            }
        }

        var lastSeat = get(count - 1);
        var endDistance = _seatCount - 1 - lastSeat;

        if (endDistance > bestDistance)
        {
            bestSeat = _seatCount - 1;
            bestIndex = count;
        }

        return (bestIndex, bestSeat);
    }
}
