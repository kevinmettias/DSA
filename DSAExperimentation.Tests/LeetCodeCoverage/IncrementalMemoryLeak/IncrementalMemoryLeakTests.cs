using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IncrementalMemoryLeak;

// LeetCode 1860. Incremental Memory Leak: each second i, the stick with more
// remaining memory gives up i megabytes (ties go to stick 1). This repo's own
// Heap<Element,TOrder> as a 2-element max-heap picks that stick every round - popping
// the current max, pushing it back reduced (or leaving it and stopping once it can't
// cover i) - the same "explicit priority-holding primitive per round" shape
// FindMedianFromDataStreamTests.cs already establishes for Heap<T,TOrder>. Ties are
// broken toward stick 1 inside MemoryStick's own IComparable, since a binary heap
// gives no other stability guarantee among equal-priority elements.
public sealed class IncrementalMemoryLeakTests
{
    [Fact]
    public void MemoryLeak_EqualSticks_PrefersStickOneOnTies()
    {
        var result = MemoryLeak(memory1: 2, memory2: 2);

        Assert.Equal([3, 1, 0], result);
    }

    [Fact]
    public void MemoryLeak_UnequalSticks_ReturnsCrashSecondAndRemainingMemory()
    {
        var result = MemoryLeak(memory1: 8, memory2: 11);

        Assert.Equal([6, 0, 4], result);
    }

    private static int[] MemoryLeak(int memory1, int memory2)
    {
        var sticks = new Heap<MemoryStick, MaxHeapOrder<MemoryStick>>();
        sticks.Push(new MemoryStick(memory1, StickIndex: 1));
        sticks.Push(new MemoryStick(memory2, StickIndex: 2));

        var i = 1;
        while (!DrainOneSecond(sticks, i))
        {
            i++;
        }

        var result = new int[3];
        result[0] = i;
        while (sticks.TryPop(out var stick))
        {
            result[stick.StickIndex] = stick.Amount;
        }

        return result;
    }

    // Pops the stick with the most remaining memory and takes i megabytes from
    // it for second i. Returns true once no stick has enough left to give (the
    // crash second), leaving that stick back on the heap unchanged.
    private static bool DrainOneSecond(Heap<MemoryStick, MaxHeapOrder<MemoryStick>> sticks, int i)
    {
        sticks.TryPop(out var largest);

        if (largest.Amount < i)
        {
            sticks.Push(largest);
            return true;
        }

        sticks.Push(largest with { Amount = largest.Amount - i });
        return false;
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
