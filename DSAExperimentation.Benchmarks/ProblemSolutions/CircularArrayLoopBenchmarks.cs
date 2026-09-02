using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Circular Array Loop (LC 457): a per-start HashSet<int> walk (allocated fresh for
// every starting index, the way most people first write this) vs. building this
// repo's own SinglyLinkedListNode<int> functional graph once - direction-mismatched
// and same-index self-loop edges left null - then reusing SinglyLinkedList's Floyd's
// tortoise-and-hare CycleDetection.HasCycle from every start. Values are random and
// nonzero, so a genuine cycle is astronomically unlikely to appear and both strategies
// run every starting index to completion instead of exiting early on the first try.
[MemoryDiagnoser]
public class CircularArrayLoopBenchmarks
{
    private const int SignChoiceCount = 2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length)
            .Select(_ => random.Next(1, Length) * (random.Next(SignChoiceCount) == 0 ? 1 : -1))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool HashSetPerStartWalk()
    {
        for (var start = 0; start < _values.Length; start++)
        {
            if (HasCycleFrom(start))
            {
                return true;
            }
        }

        return false;
    }

    private bool HasCycleFrom(int start)
    {
        var n = _values.Length;
        var visited = new HashSet<int>();
        var current = start;

        while (visited.Add(current))
        {
            var next = (((current + _values[current]) % n) + n) % n;

            if (next == current || Math.Sign(_values[next]) != Math.Sign(_values[current]))
            {
                return false;
            }

            current = next;
        }

        return true;
    }

    [Benchmark]
    public bool LinkedListFloyd()
    {
        var n = _values.Length;
        var nodes = CreateNodes(n);
        LinkNodes(nodes, _values, n);

        return AnyNodeHasCycle(nodes, n);
    }

    private static SinglyLinkedListNode<int>[] CreateNodes(int n)
    {
        var nodes = new SinglyLinkedListNode<int>[n];

        for (var i = 0; i < n; i++)
        {
            nodes[i] = new SinglyLinkedListNode<int>(i);
        }

        return nodes;
    }

    private static void LinkNodes(SinglyLinkedListNode<int>[] nodes, int[] values, int n)
    {
        for (var i = 0; i < n; i++)
        {
            var nextIndex = (((i + values[i]) % n) + n) % n;

            if (nextIndex == i || Math.Sign(values[nextIndex]) != Math.Sign(values[i]))
            {
                continue;
            }

            nodes[i].Next = nodes[nextIndex];
        }
    }

    private static bool AnyNodeHasCycle(SinglyLinkedListNode<int>[] nodes, int n)
    {
        for (var i = 0; i < n; i++)
        {
            if (CycleDetection.HasCycle(nodes[i]))
            {
                return true;
            }
        }

        return false;
    }
}
