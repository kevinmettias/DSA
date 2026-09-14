using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.IncrementalMemoryLeak;

// LeetCode 1860. Incremental Memory Leak: on second i the stick with more memory
// remaining gives up i megabytes (ties go to stick 1), and the program crashes on
// the first second neither stick can cover. The answer is
// [crashSecond, memory1Remaining, memory2Remaining].
//
// Both strategies allocate exactly the same rounds and differ only in how "the
// stick with the most left" is decided: a direct comparison of two ints, or this
// repo's own Heap<Element, TOrder> holding the two sticks as a max-heap - the same
// "explicit priority-holding primitive per round" shape FindMedianFromDataStream
// establishes for Heap<Element, TOrder>.
internal static class IncrementalMemoryLeakSolution
{
    private const int FirstStickIndex = 1;
    private const int SecondStickIndex = 2;

    // LeetCode's answer shape: [crashSecond, memory1, memory2].
    private const int ResultSize = 3;
    private const int CrashSecondIndex = 0;

    // The textbook answer: two ints and an if/else, no data structure at all.
    // Deliberately BCL-only - it is the arm the heap simulation below has to
    // justify itself against.
    public static int[] MemoryLeakByArithmetic(int memory1, int memory2)
    {
        var second = 1;

        while (memory1 >= second || memory2 >= second)
        {
            if (memory1 >= memory2)
            {
                memory1 -= second;
            }
            else
            {
                memory2 -= second;
            }

            second++;
        }

        return [second, memory1, memory2];
    }

    // The same round-by-round allocation channeled through Heap<Element, TOrder>
    // as a two-element max-heap: pop the stick with the most left, push it back
    // reduced, and stop the first time the popped stick cannot cover the second.
    // Ties resolve toward stick 1 inside MemoryStick's own IComparable, because a
    // binary heap gives no stability guarantee among equal-priority elements.
    public static int[] MemoryLeakByMaxHeap(int memory1, int memory2)
    {
        var sticks = new Heap<MemoryStick, MaxHeapOrder<MemoryStick>>();
        sticks.Push(new MemoryStick(memory1, FirstStickIndex));
        sticks.Push(new MemoryStick(memory2, SecondStickIndex));

        var result = new int[ResultSize];
        result[CrashSecondIndex] = DepleteToCrashSecond(sticks);

        while (sticks.TryPop(out var stick))
        {
            result[stick.StickIndex] = stick.Amount;
        }

        return result;
    }

    // Runs the allocation until the largest remaining stick cannot give away the
    // current second, leaving every stick on the heap at its final amount.
    private static int DepleteToCrashSecond(Heap<MemoryStick, MaxHeapOrder<MemoryStick>> sticks)
    {
        var second = 1;

        while (true)
        {
            sticks.TryPop(out var largest);

            if (largest.Amount < second)
            {
                sticks.Push(largest);
                return second;
            }

            sticks.Push(largest with { Amount = largest.Amount - second });
            second++;
        }
    }

    // One memory stick's remaining megabytes, ordered by how much is left and then
    // toward the lower-numbered stick, which is LC 1860's own tie-break rule. It
    // models this problem alone, so it lives beside the solution (section 17.3).
    private readonly record struct MemoryStick(int Amount, int StickIndex) : IComparable<MemoryStick>
    {
        public int CompareTo(MemoryStick other)
        {
            var byAmount = Amount.CompareTo(other.Amount);
            return byAmount != 0 ? byAmount : other.StickIndex.CompareTo(StickIndex);
        }
    }
}
