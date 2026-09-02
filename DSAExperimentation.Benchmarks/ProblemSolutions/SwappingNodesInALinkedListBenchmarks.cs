using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Swapping Nodes in a Linked List (LC 1721): materializing the list into an array first
// (the naive approach many linked-list solutions fall back to, paying O(n) extra space to
// get random access) vs. this repo's own SinglyLinkedListNode<T> walked with a fast/slow
// pair of references so the kth-from-front and kth-from-end nodes are both found in one
// O(n), O(1)-extra-space pass - the same "array baseline vs. linked-list primitive" shape
// SwapNodesInPairsBenchmarks already uses for its own problem.
[MemoryDiagnoser]
public class SwappingNodesInALinkedListBenchmarks
{
    private const int TargetIndexDivisor = 3; // k picks a node one third of the way into the list

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;
    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        _values = Enumerable.Range(1, Length).ToArray();
        _k = Length / TargetIndexDivisor;
    }

    [Benchmark(Baseline = true)]
    public int ArrayMaterializeSwap()
    {
        var copy = _values.ToArray();
        (copy[_k - 1], copy[copy.Length - _k]) = (copy[copy.Length - _k], copy[_k - 1]);
        return copy[0];
    }

    [Benchmark]
    public int LinkedListTwoPointerSwap()
        => SwapNodes(BuildList(_values), _k)!.Value;

    private static SinglyLinkedListNode<int>? SwapNodes(SinglyLinkedListNode<int>? head, int k)
    {
        var front = head;
        for (var i = 1; i < k; i++)
        {
            front = front!.Next;
        }

        var end = head;
        var runner = front;
        while (runner!.Next is not null)
        {
            runner = runner.Next;
            end = end!.Next;
        }

        (front!.Value, end!.Value) = (end.Value, front.Value);
        return head;
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
}
