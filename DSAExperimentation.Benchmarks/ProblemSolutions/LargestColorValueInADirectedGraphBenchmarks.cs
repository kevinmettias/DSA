using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LargestColorValueInADirectedGraph;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LargestColorValueInADirectedGraphSolution's, the
// same methods LargestColorValueInADirectedGraphTests proves correct. Each arm is
// handed the prepared List<ColorGraphNode> its hoisted overload takes, so graph
// construction is charged to [GlobalSetup] rather than to the DP being measured.
// Nodes form a guaranteed-acyclic DAG (every edge points from a lower id to a
// higher one, capped fan-out) so both strategies do their full real workload
// instead of an early cycle bailout.
[MemoryDiagnoser]
public class LargestColorValueInADirectedGraphBenchmarks
{
    private const int AlphabetSize = 26;
    private const int MaxFanOut = 3;

    // LC problem number, reused as the deterministic node-color seed.
    private const int RandomSeed = 1857;

    private List<ColorGraphNode> _nodes = new();

    [Params(50, 1_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nodes = Enumerable.Range(0, NodeCount)
            .Select(id => new ColorGraphNode(id, random.Next(AlphabetSize)))
            .ToList();

        for (var i = 0; i < NodeCount; i++)
        {
            var fanOut = Math.Min(MaxFanOut, NodeCount - 1 - i);
            for (var f = 1; f <= fanOut; f++)
            {
                _nodes[i].Successors.Add(_nodes[i + f]);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int RepeatedRelaxation() =>
        LargestColorValueInADirectedGraphSolution.LargestPathValueByRepeatedRelaxation(_nodes);

    [Benchmark]
    public int KahnsTopologicalSortDp() =>
        LargestColorValueInADirectedGraphSolution.LargestPathValueByKahnsTopologicalSort(_nodes);
}
