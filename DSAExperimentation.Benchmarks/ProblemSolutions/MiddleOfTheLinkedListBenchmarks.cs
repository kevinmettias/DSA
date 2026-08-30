using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Middle of the Linked List (LC 876): a naive two-pass approach (count the list,
// then walk length/2 steps from the head) vs. Floyd's slow/fast two-pointer walk
// over this repo's own SinglyLinkedListNode<int>.Next (MiddleOfTheLinkedListTests
// precedent) that lands on the same node in a single pass. Both are O(n), but
// CountThenWalk touches every node twice while SlowFastTwoPointer touches the
// list once.
[MemoryDiagnoser]
public class MiddleOfTheLinkedListBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private SinglyLinkedListNode<int> _head = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(876);
        _head = BuildRandomList(random, Length);
    }

    [Benchmark(Baseline = true)]
    public int CountThenWalk()
    {
        var count = 0;
        for (var node = _head; node is not null; node = node.Next)
        {
            count++;
        }

        var target = count / 2;
        var current = _head;
        for (var i = 0; i < target; i++)
        {
            current = current!.Next;
        }

        return current!.Value;
    }

    [Benchmark]
    public int SlowFastTwoPointer()
    {
        var slow = _head;
        var fast = _head;

        while (fast?.Next is not null)
        {
            slow = slow!.Next;
            fast = fast.Next.Next;
        }

        return slow!.Value;
    }

    private static SinglyLinkedListNode<int> BuildRandomList(Random random, int length)
    {
        var head = new SinglyLinkedListNode<int>(random.Next(0, 1_000));
        var tail = head;

        for (var i = 1; i < length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(random.Next(0, 1_000));
            tail = tail.Next;
        }

        return head;
    }
}
