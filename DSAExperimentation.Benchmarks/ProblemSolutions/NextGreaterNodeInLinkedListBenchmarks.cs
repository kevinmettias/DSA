using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Next Greater Node In Linked List (LC 1019): the canonical O(n^2) per-node forward
// scan vs. a single O(n) monotonic-decreasing sweep through this repo's own
// Stack<int> of pending indices (DailyTemperaturesBenchmarks precedent), after one
// initial O(n) walk of SinglyLinkedListNode<int>.Next to materialize node values.
// Values are a random permutation so no node's answer short-circuits the
// brute-force scan early.
[MemoryDiagnoser]
public class NextGreaterNodeInLinkedListBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private SinglyLinkedListNode<int> _head = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1019);
        var values = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
        _head = Build(values);
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceScan()
    {
        var values = ToValues(_head);
        var result = new int[values.Count];

        for (var i = 0; i < values.Count; i++)
        {
            for (var later = i + 1; later < values.Count; later++)
            {
                if (values[later] > values[i])
                {
                    result[i] = values[later];
                    break;
                }
            }
        }

        return result;
    }

    [Benchmark]
    public int[] MonotonicStackSweep()
    {
        var values = ToValues(_head);
        var result = new int[values.Count];
        var decreasingIndices = new RepoIntStack();

        for (var i = 0; i < values.Count; i++)
        {
            while (decreasingIndices.TryPeek(out var previousIndex) && values[previousIndex] < values[i])
            {
                decreasingIndices.TryPop(out _);
                result[previousIndex] = values[i];
            }

            decreasingIndices.Push(i);
        }

        return result;
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
