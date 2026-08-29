using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

[MemoryDiagnoser]
public class RemoveNthNodeFromEndOfListBenchmarks
{
    [Params(200, 5_000)] public int Length;
    private int[] _values = null!;

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(1, Length).ToArray();

    [Benchmark(Baseline = true)]
    public int ArrayCopyRemove()
    {
        var index = _values.Length - (_values.Length / 2);
        var copy = new int[_values.Length - 1];
        Array.Copy(_values, 0, copy, 0, index);
        Array.Copy(_values, index + 1, copy, index, _values.Length - index - 1);
        return copy.Length;
    }

    [Benchmark]
    public int LinkedListTwoRunner() => Count(RemoveNthFromEnd(BuildList(_values), Length / 2));

    private static SinglyLinkedListNode<int>? RemoveNthFromEnd(SinglyLinkedListNode<int>? head, int n)
    {
        var dummy = new SinglyLinkedListNode<int>(0) { Next = head };
        var fast = dummy;
        var slow = dummy;
        for (var i = 0; i < n; i++) fast = fast.Next!;
        while (fast.Next is not null) { fast = fast.Next; slow = slow.Next!; }
        slow.Next = slow.Next?.Next;
        return dummy.Next;
    }

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;
        foreach (var value in values) { tail.Next = new SinglyLinkedListNode<int>(value); tail = tail.Next; }
        return dummy.Next;
    }

    private static int Count(SinglyLinkedListNode<int>? head)
    {
        var count = 0;
        for (var node = head; node is not null; node = node.Next) count++;
        return count;
    }
}
