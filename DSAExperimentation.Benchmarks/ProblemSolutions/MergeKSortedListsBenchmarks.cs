using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Heap;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class MergeKSortedListsBenchmarks
{
    [Params(8, 64)] public int ListCount;
    private int[][] _values = null!;

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, ListCount).Select(offset => Enumerable.Range(0, 64).Select(i => (i * ListCount) + offset).ToArray()).ToArray();

    [Benchmark(Baseline = true)]
    public int FlattenSort()
    {
        var values = _values.SelectMany(x => x).ToArray();
        Array.Sort(values);
        return values.Length;
    }

    [Benchmark]
    public int HeapMerge() => Count(MergeKLists(_values.Select(BuildList).ToArray()));

    private static SinglyLinkedListNode<int>? MergeKLists(SinglyLinkedListNode<int>?[] lists)
    {
        var heap = new Heap<SinglyLinkedListNode<int>, NodeOrder>();
        foreach (var list in lists) if (list is not null) heap.Push(list);
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;
        while (heap.TryPop(out var node))
        {
            if (node.Next is not null) heap.Push(node.Next);
            tail.Next = node; tail = node;
        }
        tail.Next = null;
        return dummy.Next;
    }

    private readonly struct NodeOrder : IHeapOrder<SinglyLinkedListNode<int>>
    {
        public static bool HasPriority(SinglyLinkedListNode<int> candidate, SinglyLinkedListNode<int> incumbent) => candidate.Value < incumbent.Value;
    }

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0); var tail = dummy;
        foreach (var value in values) { tail.Next = new SinglyLinkedListNode<int>(value); tail = tail.Next; }
        return dummy.Next;
    }

    private static int Count(SinglyLinkedListNode<int>? head)
    {
        var count = 0; for (var node = head; node is not null; node = node.Next) count++; return count;
    }
}
