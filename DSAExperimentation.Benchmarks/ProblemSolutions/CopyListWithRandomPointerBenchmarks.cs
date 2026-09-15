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

    private RandomLinkedListNode<int> _head = null!;

    [Params(200, 5_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _head = BuildRandomList(NodeCount, Seed);

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
            var staysNull = random.NextDouble() < NullRandomProbability;

            node.Random = staysNull ? null : RandomNode(nodes, random, count);
        }

        return nodes[0];
    }

    private static RandomLinkedListNode<int> RandomNode(
        RandomLinkedListNode<int>[] nodes,
        Random random,
        int count) => nodes[random.Next(count)];

    [Benchmark]
    public object? HashMapMemo() => CopyListWithRandomPointerSolution.CopyByHashMapMemo(_head);
}
