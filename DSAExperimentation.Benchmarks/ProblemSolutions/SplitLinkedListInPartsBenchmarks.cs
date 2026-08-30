using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Split Linked List in Parts (LC 725): ArrayRebuild is the naive approach -
// materialize every value into a List<int>, then allocate k brand-new
// SinglyLinkedListNode<int> chains from slices of it, O(n) extra node
// allocation on top of the input list. InPlaceSplit instead walks this repo's
// SinglyLinkedListNode<int> chain once and cuts existing Next pointers to carve
// out each part, reusing every original node - only the k-length result array
// is new allocation. Both methods clone the shared fixture first (OddEvenLinkedListBenchmarks
// precedent) since InPlaceSplit mutates the chain it walks and BenchmarkDotNet
// invokes each [Benchmark] method many times against the one GlobalSetup fixture.
[MemoryDiagnoser]
public class SplitLinkedListInPartsBenchmarks
{
    private const int Parts = 7;

    [Params(200, 5_000)]
    public int Length;

    private SinglyLinkedListNode<int> _head = null!;

    [GlobalSetup]
    public void Setup() => _head = Build(Enumerable.Range(0, Length).ToArray());

    [Benchmark(Baseline = true)]
    public int ArrayRebuild()
    {
        var head = Clone(_head);
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        var size = values.Count / Parts;
        var extra = values.Count % Parts;
        var parts = new SinglyLinkedListNode<int>?[Parts];
        var index = 0;

        for (var i = 0; i < Parts; i++)
        {
            var currentSize = size + (i < extra ? 1 : 0);
            if (currentSize == 0)
            {
                continue;
            }

            var dummy = new SinglyLinkedListNode<int>(0);
            var tail = dummy;

            for (var j = 0; j < currentSize; j++)
            {
                tail.Next = new SinglyLinkedListNode<int>(values[index++]);
                tail = tail.Next;
            }

            parts[i] = dummy.Next;
        }

        return parts.Count(p => p is not null);
    }

    [Benchmark]
    public int InPlaceSplit()
    {
        var head = Clone(_head);
        var length = 0;

        for (var node = head; node is not null; node = node.Next)
        {
            length++;
        }

        var size = length / Parts;
        var extra = length % Parts;
        var parts = new SinglyLinkedListNode<int>?[Parts];
        var current = head;

        for (var i = 0; i < Parts && current is not null; i++)
        {
            parts[i] = current;
            var currentSize = size + (i < extra ? 1 : 0);

            for (var j = 1; j < currentSize; j++)
            {
                current = current!.Next;
            }

            var next = current!.Next;
            current.Next = null;
            current = next;
        }

        return parts.Count(p => p is not null);
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
