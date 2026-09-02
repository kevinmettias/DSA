using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Reverse Nodes in Even Length Groups (LC 2074): a plain int[] baseline that computes
// the same increasing-then-truncated group boundaries and reverses each even-actual
// -length group in place via Array.Reverse, vs. this repo's own SinglyLinkedListNode<T>
// pointer splicing directly over the list - no array materialization at all.
[MemoryDiagnoser]
public class ReverseNodesInEvenLengthGroupsBenchmarks
{
    private const int InitialGroupSize = 2;
    private const int EvenLengthDivisor = 2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(1, Length).ToArray();

    [Benchmark(Baseline = true)]
    public int ArrayEvenLengthGroupReverse()
    {
        var array = _values.ToArray();
        var start = Math.Min(1, array.Length);
        var groupSize = InitialGroupSize;

        while (start < array.Length)
        {
            var actualLength = Math.Min(groupSize, array.Length - start);

            if (actualLength % EvenLengthDivisor == 0)
            {
                Array.Reverse(array, start, actualLength);
            }

            start += actualLength;
            groupSize++;
        }

        return array[0];
    }

    [Benchmark]
    public int LinkedListEvenLengthGroupReverse()
    {
        var reversed = ReverseEvenLengthGroups(BuildList(_values));
        return Count(reversed);
    }

    private static SinglyLinkedListNode<int>? ReverseEvenLengthGroups(SinglyLinkedListNode<int>? head)
    {
        var groupPrevious = head;
        var groupSize = InitialGroupSize;

        while (groupPrevious?.Next is not null)
        {
            var (tail, length) = MeasureGroup(groupPrevious, groupSize);

            groupPrevious = length % EvenLengthDivisor == 0 ? ReverseGroup(groupPrevious, tail) : tail;
            groupSize++;
        }

        return head;
    }

    private static (SinglyLinkedListNode<int> Tail, int Length) MeasureGroup(
        SinglyLinkedListNode<int> groupPrevious, int groupSize)
    {
        var node = groupPrevious;
        var length = 0;

        while (length < groupSize && node.Next is not null)
        {
            node = node.Next;
            length++;
        }

        return (node, length);
    }

    private static SinglyLinkedListNode<int> ReverseGroup(
        SinglyLinkedListNode<int> groupPrevious, SinglyLinkedListNode<int> tail)
    {
        var groupNext = tail.Next;
        var oldGroupHead = groupPrevious.Next!;
        SinglyLinkedListNode<int>? previous = groupNext;
        var current = oldGroupHead;

        while (current != groupNext)
        {
            var next = current!.Next;
            current.Next = previous;
            previous = current;
            current = next;
        }

        groupPrevious.Next = tail;
        return oldGroupHead;
    }

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next;
    }

    private static int Count(SinglyLinkedListNode<int>? head)
    {
        var count = 0;

        for (var node = head; node is not null; node = node.Next)
        {
            count++;
        }

        return count;
    }
}
