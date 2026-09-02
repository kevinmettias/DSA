using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Odd Even Linked List (LC 328): a two-List<int>-and-rebuild baseline (materializes
// odd- and even-indexed values into separate buffers, then re-links a fresh chain,
// O(n) extra space) vs. this repo's SinglyLinkedListNode<int> rewired in place with
// O(1) extra space. Both methods clone the shared fixture first so mutating one
// iteration's result never corrupts the next (SetMatrixZeroesBenchmarks precedent).
[MemoryDiagnoser]
public class OddEvenLinkedListBenchmarks
{
    private const int ParityDivisor = 2;

    [Params(200, 5_000)]
    public int Length;

    private SinglyLinkedListNode<int> _head = null!;

    [GlobalSetup]
    public void Setup() => _head = Build(Enumerable.Range(0, Length).ToArray());

    [Benchmark(Baseline = true)]
    public int TwoListRebuild()
    {
        var head = Clone(_head);
        var odds = new List<int>();
        var evens = new List<int>();
        var index = 0;

        for (var node = head; node is not null; node = node.Next)
        {
            (index % ParityDivisor == 0 ? odds : evens).Add(node.Value);
            index++;
        }

        odds.AddRange(evens);
        return odds.Count;
    }

    [Benchmark]
    public int InPlaceRewire()
    {
        var head = Clone(_head);
        var reordered = OddEvenList(head);
        var count = 0;

        for (var node = reordered; node is not null; node = node.Next)
        {
            count++;
        }

        return count;
    }

    private static SinglyLinkedListNode<int>? OddEvenList(SinglyLinkedListNode<int>? head)
    {
        if (head?.Next is null)
        {
            return head;
        }

        var odd = head;
        var even = head.Next;
        var evenHead = even;

        while (even?.Next is not null)
        {
            odd.Next = even.Next;
            odd = odd.Next;
            even.Next = odd.Next;
            even = even.Next;
        }

        odd.Next = evenHead;
        return head;
    }

    private static SinglyLinkedListNode<int> Build(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next!;
    }

    private static SinglyLinkedListNode<int> Clone(SinglyLinkedListNode<int> head)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        for (var node = head; node is not null; node = node.Next)
        {
            tail.Next = new SinglyLinkedListNode<int>(node.Value);
            tail = tail.Next;
        }

        return dummy.Next!;
    }
}
