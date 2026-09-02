using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CloneGraph;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the pre-migration version carried two [Benchmark] arms
// (Baseline, PrimitiveComposed) that were both compile-smoke placeholders
// returning a literal 1 - neither ever called the test's private recursive
// clone. Both arms are now CloneGraphSolution's, over a ring graph (each node
// connected to its next and previous neighbor) sized by NodeCount. Each arm
// returns the cloned root's Value rather than the clone itself - Node is
// internal to DSAExperimentation.LeetCode, so a public [Benchmark] method
// cannot expose it directly (WordLadderII's arms reduce to .Count for the
// same reason).
[MemoryDiagnoser]
public class CloneGraphBenchmarks
{
    [Params(10, 1_000)]
    public int NodeCount;

    private Node _graph = null!;

    [GlobalSetup]
    public void Setup() => _graph = BuildRing(NodeCount);

    [Benchmark(Baseline = true)]
    public int DictionaryDfs() => CloneGraphSolution.CloneByDictionaryDfs(_graph)!.Value;

    [Benchmark]
    public int HashMapDfs() => CloneGraphSolution.CloneByHashMapDfs(_graph)!.Value;

    private static Node BuildRing(int nodeCount)
    {
        var nodes = new Node[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            nodes[i] = new Node(i + 1);
        }

        for (var i = 0; i < nodeCount; i++)
        {
            var next = nodes[(i + 1) % nodeCount];
            nodes[i].Neighbors.Add(next);
            next.Neighbors.Add(nodes[i]);
        }

        return nodes[0];
    }
}
