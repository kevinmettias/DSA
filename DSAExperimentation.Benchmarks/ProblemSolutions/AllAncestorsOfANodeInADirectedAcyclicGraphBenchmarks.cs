using DSAExperimentation.LeetCode.AllAncestorsOfANodeInADirectedAcyclicGraph;
using AncestorNode = DSAExperimentation.DataStructures.Graph.Adjacency.AdjacencyNode;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AllAncestorsOfANodeInADirectedAcyclicGraphSolution's,
// the same methods AllAncestorsOfANodeInADirectedAcyclicGraphSolutionTests proves correct,
// each handed the prepared node list its hoisted overload takes so graph
// construction is charged to [GlobalSetup] rather than to the walk being measured.
//
// Edges point from a lower id to a higher one with a capped fan-out, so the
// relation is a guaranteed-acyclic DAG whose transitive closure is dense - the
// same generation shape CourseScheduleIIBenchmarks and LoudAndRichBenchmarks use.
public class AllAncestorsOfANodeInADirectedAcyclicGraphBenchmarks
{
    private const int MaxFanOut = 3;

    private List<AncestorNode> _nodes = new();

    [Params(50, 1_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _nodes = Enumerable.Range(0, NodeCount).Select(id => new AncestorNode(id)).ToList();

        for (var i = 0; i < NodeCount; i++)
        {
            var fanOut = Math.Min(MaxFanOut, NodeCount - 1 - i);
            for (var f = 1; f <= fanOut; f++)
            {
                _nodes[i].Neighbors.Add(_nodes[i + f]);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public List<List<int>> NaivePerNodeForwardWalk() =>
        AllAncestorsOfANodeInADirectedAcyclicGraphSolution.GetAncestorsByPerNodeForwardWalk(_nodes);

    [Benchmark]
    public List<List<int>> TopologicalDpPass() =>
        AllAncestorsOfANodeInADirectedAcyclicGraphSolution.GetAncestorsByTopologicalDpPass(_nodes);
}
