using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Delete the Middle Node of a Linked List (LC 2095): the naive way to delete an
// index found by first counting the list is to rebuild a whole new list, copying
// every node except the one at that index - two full traversals (count, then copy)
// and n-1 fresh node allocations. SlowFastPointerInPlaceDelete instead reuses this
// repo's own SinglyLinkedListNode<T>.Next slow/fast walk (the same two-pointer shape
// CycleDetection.cs already uses) to find the middle's predecessor in one traversal,
// then splices it out in place with zero new node allocations beyond the list
// itself. Both methods build a fresh list from the same source array on every
// invocation - deleting is destructive, so a shared mutable list across invocations
// would only ever pay the real cost once, the same "fresh copy per invocation"
// discipline SortAnArrayBenchmarks already uses for its in-place sort.
[MemoryDiagnoser]
public class DeleteTheMiddleNodeOfALinkedListBenchmarks
{
    private const int RandomSeed = 2095; // LC problem number
    private const int ValueRangeExclusive = 1_000;
    private const int MiddleDivisor = 2;

    [Params(500, 20_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, ValueRangeExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int TwoPassCountThenRebuild()
    {
        var head = BuildList(_values);

        var length = 0;
        for (var node = head; node is not null; node = node.Next)
        {
            length++;
        }

        var middleIndex = length / MiddleDivisor;
        var newHead = RebuildWithoutMiddle(head, middleIndex);

        return CountNodes(newHead);
    }

    private static SinglyLinkedListNode<int>? RebuildWithoutMiddle(SinglyLinkedListNode<int> head, int middleIndex)
    {
        var builder = new LinkedListBuilder();
        var index = 0;

        for (var node = head; node is not null; node = node.Next, index++)
        {
            if (index != middleIndex)
            {
                builder.Append(node.Value);
            }
        }

        return builder.Head;
    }

    private sealed class LinkedListBuilder
    {
        private SinglyLinkedListNode<int>? _tail;

        public SinglyLinkedListNode<int>? Head { get; private set; }

        public void Append(int value)
        {
            var copy = new SinglyLinkedListNode<int>(value);

            if (_tail is null)
            {
                Head = copy;
            }
            else
            {
                _tail.Next = copy;
            }

            _tail = copy;
        }
    }

    [Benchmark]
    public int SlowFastPointerInPlaceDelete()
    {
        var head = BuildList(_values);
        var result = DeleteMiddle(head);
        return CountNodes(result);
    }

    private static SinglyLinkedListNode<int>? DeleteMiddle(SinglyLinkedListNode<int>? head)
    {
        if (head?.Next is null)
        {
            return null;
        }

        var prev = head;
        var slow = head;
        var fast = head;

        while (fast is not null && fast.Next is not null)
        {
            prev = slow;
            slow = slow!.Next;
            fast = fast.Next.Next;
        }

        prev!.Next = slow!.Next;
        return head;
    }

    private static SinglyLinkedListNode<int> BuildList(int[] values)
    {
        var head = new SinglyLinkedListNode<int>(values[0]);
        var tail = head;

        for (var i = 1; i < values.Length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(values[i]);
            tail = tail.Next;
        }

        return head;
    }

    private static int CountNodes(SinglyLinkedListNode<int>? head)
    {
        var count = 0;

        for (var node = head; node is not null; node = node.Next)
        {
            count++;
        }

        return count;
    }
}
