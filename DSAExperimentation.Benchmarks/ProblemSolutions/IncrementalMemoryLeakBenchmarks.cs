using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Incremental Memory Leak (LC 1860): Arithmetic compares the two sticks directly with
// a plain if/else each second. HeapSimulation channels the identical round-by-round
// allocation through this repo's own Heap<Element,TOrder> as a 2-element max-heap
// (ties broken toward stick 1 via MemoryStick's own IComparable) - the same "same
// algorithm, channeled through a repo primitive" comparison AddDigitsBenchmarks.cs
// already makes for LC 258's Stack<T>.
[MemoryDiagnoser]
public class IncrementalMemoryLeakBenchmarks
{
    private const int SecondStickIndex = 2;
    private const int ResultSize = 3;

    [Params(1_000_000, 100_000_000)]
    public int Capacity;

    [Benchmark(Baseline = true)]
    public int[] Arithmetic()
    {
        var memory1 = Capacity;
        var memory2 = Capacity;
        var i = 1;

        while (memory1 >= i || memory2 >= i)
        {
            if (memory1 >= memory2)
            {
                memory1 -= i;
            }
            else
            {
                memory2 -= i;
            }

            i++;
        }

        return [i, memory1, memory2];
    }

    [Benchmark]
    public int[] HeapSimulation()
    {
        var sticks = new Heap<MemoryStick, MaxHeapOrder<MemoryStick>>();
        sticks.Push(new MemoryStick(Capacity, StickIndex: 1));
        sticks.Push(new MemoryStick(Capacity, StickIndex: SecondStickIndex));

        var i = DepleteToStableRound(sticks);

        var result = new int[ResultSize];
        result[0] = i;
        while (sticks.TryPop(out var stick))
        {
            result[stick.StickIndex] = stick.Amount;
        }

        return result;
    }

    private static int DepleteToStableRound(Heap<MemoryStick, MaxHeapOrder<MemoryStick>> sticks)
    {
        var i = 1;
        while (true)
        {
            sticks.TryPop(out var largest);

            if (largest.Amount < i)
            {
                sticks.Push(largest);
                break;
            }

            sticks.Push(largest with { Amount = largest.Amount - i });
            i++;
        }

        return i;
    }

    private readonly record struct MemoryStick(int Amount, int StickIndex) : IComparable<MemoryStick>
    {
        public int CompareTo(MemoryStick other)
        {
            var byAmount = Amount.CompareTo(other.Amount);
            return byAmount != 0 ? byAmount : other.StickIndex.CompareTo(StickIndex);
        }
    }
}
