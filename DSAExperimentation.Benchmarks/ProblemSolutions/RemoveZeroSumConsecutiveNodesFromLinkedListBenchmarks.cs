using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Remove Zero Sum Consecutive Nodes from Linked List (LC 1171): the textbook O(n^2)
// nested-loop brute force (for every start node, walk forward re-summing until a
// zero-sum run is found and spliced out) vs. this repo's own
// HashMap<int, SinglyLinkedListNode<int>> prefix-sum index, which finds every
// zero-sum run in one O(n) pass.
[MemoryDiagnoser]
public class RemoveZeroSumConsecutiveNodesFromLinkedListBenchmarks
{
    private const int NodeValueMagnitude = 3;
    private const int NodeValueExclusiveUpperBound = 4;

    [Params(300, 3_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(-NodeValueMagnitude, NodeValueExclusiveUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int NestedLoopRescan() => Count(RemoveZeroSumSublistsNaive(BuildList(_values)));

    [Benchmark]
    public int HashMapPrefixSum() => Count(RemoveZeroSumSublists(BuildList(_values)));

    private static SinglyLinkedListNode<int>? RemoveZeroSumSublistsNaive(SinglyLinkedListNode<int>? head)
    {
        var dummy = new SinglyLinkedListNode<int>(0) { Next = head };

        for (var start = dummy; start is not null; start = start.Next)
        {
            var sum = 0;

            for (var end = start; end is not null; end = end.Next)
            {
                sum += end.Value;

                if (sum == 0)
                {
                    start.Next = end.Next;
                }
            }
        }

        return dummy.Next;
    }

    private static SinglyLinkedListNode<int>? RemoveZeroSumSublists(SinglyLinkedListNode<int>? head)
    {
        var dummy = new SinglyLinkedListNode<int>(0) { Next = head };
        var lastNodeAtSum = new HashMap<int, SinglyLinkedListNode<int>>();

        var sum = 0;
        for (var node = dummy; node is not null; node = node.Next)
        {
            sum += node.Value;
            lastNodeAtSum.Set(sum, node);
        }

        sum = 0;
        for (var node = dummy; node is not null; node = node.Next)
        {
            sum += node.Value;
            lastNodeAtSum.TryGetValue(sum, out var lastNodeWithSameSum);
            node.Next = lastNodeWithSameSum!.Next;
        }

        return dummy.Next;
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
