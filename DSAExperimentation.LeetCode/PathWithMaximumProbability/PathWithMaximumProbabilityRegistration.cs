using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.LeetCode.PathWithMaximumProbability;

// Shape under test: a FLOATING-POINT answer, where exact equality is the wrong
// test and LeetCode says so itself ("answers within 1e-5 of the actual answer
// will be accepted"). Also the §17.4 hoisted-overload shape: the workload binds
// the prepared ProbabilityGraph overload so graph construction is not charged to
// the measured region, while the cases go through the raw LeetCode-shaped
// overload that builds it.
internal sealed class PathWithMaximumProbabilityRegistration : ILeetCodeProblemRegistration
{
    private const int WorkloadNodeCount = 400;

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

            // Only Dijkstra gets a workload: the exhaustive arm is exponential in
            // the vertex count, so measuring it at this size would not finish.
            .Workload("cycle-400", BuildCycle(WorkloadNodeCount), "Dijkstra")
            .Build();

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
