using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Merge In Between Linked Lists (LC 1669): the array-rebuild baseline (materialize
// list1 as a List<int>, RemoveRange the [a,b] window, InsertRange list2's values -
// O(n) per call since everything after the window shifts) vs. this repo's own
// SinglyLinkedListNode<T> chain, which splices list2 in with two pointer rewrites
// regardless of how many nodes sit on either side of the removed range.
[MemoryDiagnoser]
public class MergeInBetweenLinkedListsBenchmarks
{
    private const int SecondListLength = 5;
    private const int SecondListValueOffset = 1_000_000;
    private const int SpliceStartDivisor = 3;
    private const int WindowBoundaryOffset = 2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _list1Values = null!;
    private int[] _list2Values = null!;
    private int _a;
    private int _b;

    [GlobalSetup]
    public void Setup()
    {
        _list1Values = Enumerable.Range(0, Length).ToArray();
        _list2Values = Enumerable.Range(SecondListValueOffset, SecondListLength).ToArray();
        _a = Length / SpliceStartDivisor;
        _b = _a + SecondListLength - 1;
    }

    [Benchmark(Baseline = true)]
    public int ArraySpliceRebuild()
    {
        var merged = new List<int>(_list1Values);
        merged.RemoveRange(_a, _b - _a + 1);
        merged.InsertRange(_a, _list2Values);
        return merged.Count;
    }

    [Benchmark]
    public int LinkedListSplice()
    {
        var list1 = BuildList(_list1Values);
        var list2 = BuildList(_list2Values);

        var merged = MergeInBetween(list1, _a, _b, list2);

        return Count(merged);
    }

    private static SinglyLinkedListNode<int> MergeInBetween(SinglyLinkedListNode<int> list1, int a, int b, SinglyLinkedListNode<int> list2)
    {
        var before = list1;
        for (var i = 0; i < a - 1; i++) before = before.Next!;

        var after = before;
        for (var i = 0; i < b - a + WindowBoundaryOffset; i++) after = after.Next!;

        before.Next = list2;

        var list2Tail = list2;
        while (list2Tail.Next is not null) list2Tail = list2Tail.Next;
        list2Tail.Next = after;

        return list1;
    }

    private static SinglyLinkedListNode<int> BuildList(int[] values)
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

    private static int Count(SinglyLinkedListNode<int>? head)
    {
        var count = 0;
        for (var node = head; node is not null; node = node.Next) count++;
        return count;
    }
}
