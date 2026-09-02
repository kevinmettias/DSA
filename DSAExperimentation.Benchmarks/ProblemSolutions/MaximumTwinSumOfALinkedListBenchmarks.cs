using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Deque;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Twin Sum of a Linked List (LC 2130): the common approach of materializing
// the list into a List<int> and index-pairing list[i] with list[^(i+1)] vs. this
// repo's own Deque<int> (CircularBuffer-backed, ARCHITECTURE.md Sec.4.1) draining
// matched front/back pairs in one pass with no index arithmetic at all. _values
// backs a fresh SinglyLinkedListNode<int> chain per invocation since both
// benchmarks consume the list by walking Next once.
[MemoryDiagnoser]
public class MaximumTwinSumOfALinkedListBenchmarks
{
    private const int MaxValueExclusive = 100_000;
    private const int TwinPairStride = 2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ArrayIndexTwoPointer()
    {
        var list = new List<int>(_values.Length);

        for (var node = Build(_values); node is not null; node = node.Next)
        {
            list.Add(node.Value);
        }

        var best = 0;
        for (var i = 0; i < list.Count / TwinPairStride; i++)
        {
            best = Math.Max(best, list[i] + list[^(i + 1)]);
        }

        return best;
    }

    [Benchmark]
    public int DequeFrontBackDrain()
    {
        var values = new Deque<int>();

        for (var node = Build(_values); node is not null; node = node.Next)
        {
            values.PushBack(node.Value);
        }

        var best = 0;
        while (values.TryPopFront(out var front) && values.TryPopBack(out var back))
        {
            best = Math.Max(best, front + back);
        }

        return best;
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
