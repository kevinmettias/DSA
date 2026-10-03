using DSAExperimentation.LeetCode.CloneGraph;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: the pre-migration version carried two [Benchmark] arms
// (Baseline, PrimitiveComposed) that were both compile-smoke placeholders
// returning a literal 1 - neither ever called the test's private recursive
// clone. Both arms are now CloneGraphSolution's, over a ring graph (each node
// connected to its next and previous neighbor) sized by NodeCount. Each arm
// returns the cloned root itself as object? - Node is internal to
// DSAExperimentation.LeetCode, so a public [Benchmark] method cannot name it
// (CS0050).
public class CloneGraphBenchmarks
{
    private Node _graph = null!;

    [Params(10, 1_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _graph = BuildRing(NodeCount);

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

    [Benchmark(Baseline = true)]
    public object? DictionaryDfs() => CloneGraphSolution.CloneByDictionaryDfs(_graph);

    [Benchmark]
    public object? HashMapDfs() => CloneGraphSolution.CloneByHashMapDfs(_graph);
}
