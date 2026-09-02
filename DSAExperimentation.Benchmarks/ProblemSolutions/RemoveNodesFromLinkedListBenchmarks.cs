using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using RepoNodeStack = DSAExperimentation.DataStructures.Stack.Stack<DSAExperimentation.DataStructures.SinglyLinkedList.SinglyLinkedListNode<int>>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Remove Nodes From Linked List (LC 2487): the naive O(n^2) approach - for every node,
// scan every node to its right for a strictly greater value (baseline) - vs. a single
// O(n) left-to-right sweep through this repo's own Stack<SinglyLinkedListNode<int>>,
// the same monotonic-stack shape NextGreaterNodeInLinkedListBenchmarks already uses
// over SinglyLinkedListNode<int>.Next, here popping any node made obsolete by a larger
// one instead of just recording it. Values are a random permutation so no node's
// removal decision short-circuits the brute-force scan early.
[MemoryDiagnoser]
public class RemoveNodesFromLinkedListBenchmarks
{
    private const int RandomSeed = 2487;

    [Params(200, 5_000)]
    public int Length;

    private SinglyLinkedListNode<int> _head = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var values = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
        _head = Build(values);
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceScan()
    {
        var values = ToValues(_head);
        var kept = new List<int>();

        for (var i = 0; i < values.Count; i++)
        {
            var hasGreaterToTheRight = false;

            for (var later = i + 1; later < values.Count; later++)
            {
                if (values[later] > values[i])
                {
                    hasGreaterToTheRight = true;
                    break;
                }
            }

            if (!hasGreaterToTheRight)
            {
                kept.Add(values[i]);
            }
        }

        return kept.ToArray();
    }

    [Benchmark]
    public int[] MonotonicStackSweep()
    {
        var keep = new RepoNodeStack();

        for (var node = _head; node is not null; node = node.Next)
        {
            while (keep.TryPeek(out var top) && top.Value < node.Value)
            {
                keep.TryPop(out _);
            }

            keep.Push(node);
        }

        var survivors = new int[keep.Count];
        for (var i = survivors.Length - 1; i >= 0; i--)
        {
            keep.TryPop(out var node);
            survivors[i] = node.Value;
        }

        return survivors;
    }

    private static List<int> ToValues(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values;
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
}
