using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.CopyListWithRandomPointer;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the one arm is CopyListWithRandomPointerSolution's, the same
// method CopyListWithRandomPointerTests proves correct. Every node's Random is
// seeded to a uniformly random node in the list (itself included) or left null,
// so the HashMap memo has to handle references pointing both forward and
// backward through the list, not only ever recursing ahead of itself.
//
// Returns object, not RandomLinkedListNode<int>? - the node type is internal, so
// a public [Benchmark] method cannot name it as a return type (CS0050).
[MemoryDiagnoser]
public class CopyListWithRandomPointerBenchmarks
{
    private const int Seed = 138;
    private const double NullRandomProbability = 0.2;

    [Params(200, 5_000)]
    public int NodeCount;

    private RandomLinkedListNode<int> _head = null!;

    [GlobalSetup]
    public void Setup() => _head = BuildRandomList(NodeCount, Seed);

    [Benchmark]
    public object? HashMapMemo() => CopyListWithRandomPointerSolution.CopyByHashMapMemo(_head);

    private static RandomLinkedListNode<int> BuildRandomList(int count, int seed)
    {
        var random = new Random(seed);
        var nodes = new RandomLinkedListNode<int>[count];

        for (var i = 0; i < count; i++)
        {
            nodes[i] = new RandomLinkedListNode<int>(i);
        }

        for (var i = 0; i < count - 1; i++)
        {
            nodes[i].Next = nodes[i + 1];
        }

        foreach (var node in nodes)
        {
            node.Random = random.NextDouble() < NullRandomProbability ? null : nodes[random.Next(count)];
        }

        return nodes[0];
    }
}
