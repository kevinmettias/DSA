using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.LeetCode.PathWithMaximumProbability;

// Shape under test: a FLOATING-POINT answer, where exact equality is the wrong
// test and LeetCode says so itself ("answers within 1e-5 of the actual answer
// will be accepted"). Also the shape where the two strategies cannot share a
// single workload size, which is why Workload's strategy filter exists.
//
// The branching-graph generator below came from the retired
// PathWithMaximumProbabilityBenchmarks' own Fixtures helper. Workload inputs are
// part of a registration now, so a generator with exactly one caller belongs
// beside that caller rather than in a separate benchmark-only fixtures folder
// that a registration cannot see.
internal sealed class PathWithMaximumProbabilityRegistration : ILeetCodeProblemRegistration
{
    private const int WorkloadNodeCount = 400;
    private const int BranchingSeed = 1514; // LC problem number
    private const int ExtraEdgesPerNode = 2;
    private const double MinEdgeProbability = 0.5;
    private const double EdgeProbabilityRange = 0.49;

    public LeetCodeProblem Describe()
        => LeetCodeProblem
            .For<(int NodeCount, int[][] Edges, double[] SuccessProbabilities, int Start, int End), double>(
                "path-with-maximum-probability")
            .Strategy(
                "ExhaustiveDfs",
                input => PathWithMaximumProbabilitySolution.MaxProbabilityByExhaustiveDfs(
                    input.NodeCount, input.Edges, input.SuccessProbabilities, input.Start, input.End))
            .Strategy(
                "Dijkstra",
                input => PathWithMaximumProbabilitySolution.MaxProbabilityByDijkstra(
                    input.NodeCount, input.Edges, input.SuccessProbabilities, input.Start, input.End))
            .MatchingAnswersWith((actual, expected) => LeetCodeAnswers.WithinTolerance(actual, expected))
            .Case("example-1", (3, [[0, 1], [1, 2], [0, 2]], [0.5, 0.5, 0.2], 0, 2), 0.25)
            .Case("example-2", (3, [[0, 1], [1, 2], [0, 2]], [0.5, 0.5, 0.3], 0, 2), 0.3)
            .Case("example-3-no-path-exists", (3, [[0, 1]], [0.5], 0, 2), 0.0)
            .Case("start-equals-end", (2, [[0, 1]], [0.5], 0, 0), 1.0)

            // Edges stated low-to-high but walked high-to-low: 2 -> 1 -> 0. Only an
            // honestly undirected graph finds this one; orienting the edge list the
            // way it happens to be written passes every example above and fails here.
            .Case("edge-traversed-backwards", (3, [[0, 1], [1, 2]], [0.5, 0.4], 2, 0), 0.2)

            // A chain of certainties stays certain - the product of 1.0s must not
            // drift, and a -log transform must send 1.0 to exactly zero weight.
            .Case("chain-of-certainties", (4, [[0, 1], [1, 2], [2, 3]], [1.0, 1.0, 1.0], 0, 3), 1.0)

            // The two sizes the retired per-problem benchmark swept, and the only
            // workloads BOTH arms run: the exhaustive walk is exponential in the
            // vertex count, so this is where the comparison the benchmark exists to
            // make can actually be made.
            .Workload("branching-10", BuildBranching(nodeCount: 10))
            .Workload("branching-14", BuildBranching(nodeCount: 14))

            // Only Dijkstra gets this one: at 400 vertices the exhaustive arm would
            // not finish, but dropping the workload would leave Dijkstra unmeasured
            // at the size it exists for.
            .Workload("cycle-400", BuildCycle(WorkloadNodeCount), "Dijkstra")
            .Build();

    // Every node i > 0 gets an edge to some earlier node j < i, so node n-1 is
    // always reachable from node 0; the extra edges per node supply the branching
    // density the exhaustive baseline needs to explore exponentially many distinct
    // paths. Probabilities stay in (0.5, 0.99] so long paths remain competitive
    // with short ones and the search cannot be won by one dominant edge.
    private static (int NodeCount, int[][] Edges, double[] SuccessProbabilities, int Start, int End) BuildBranching(
        int nodeCount)
    {
        var random = new Random(BranchingSeed);
        var edges = new List<int[]>();
        var probabilities = new List<double>();

        void AddEdge(int from, int to)
        {
            edges.Add([from, to]);
            probabilities.Add(MinEdgeProbability + (random.NextDouble() * EdgeProbabilityRange));
        }

        for (var node = 1; node < nodeCount; node++)
        {
            AddEdge(random.Next(node), node);
        }

        for (var node = 0; node < nodeCount - 1; node++)
        {
            for (var extra = 0; extra < ExtraEdgesPerNode; extra++)
            {
                AddEdge(node, random.Next(node + 1, nodeCount));
            }
        }

        return (nodeCount, [.. edges], [.. probabilities], 0, nodeCount - 1);
    }

    private static (int NodeCount, int[][] Edges, double[] SuccessProbabilities, int Start, int End) BuildCycle(
        int nodeCount)
    {
        var edges = new int[nodeCount][];
        var probabilities = new double[nodeCount];

        for (var node = 0; node < nodeCount; node++)
        {
            edges[node] = [node, (node + 1) % nodeCount];
            probabilities[node] = 0.99;
        }

        return (nodeCount, edges, probabilities, 0, nodeCount / 2);
    }
}
