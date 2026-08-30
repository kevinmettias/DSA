using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Race Car (LC 818): the textbook mutate-the-tuple BFS (Queue<(int,int)> plus a
// HashSet<(int,int)> visited set, generating each of the two candidate commands on
// the fly) vs. this repo's own BFS - Reduce.Graph + DistanceMapReduceAlgebra over a
// precomputed RaceCarNode graph, the same "distance to some specific target"
// composition OpenTheLockBenchmarks already uses for LC 752. Both explore the same
// pre-bounded (position, speed) state space built by RaceCarGraphs.
[MemoryDiagnoser]
public class RaceCarBenchmarks
{
    [Params(6, 25)]
    public int Target;

    private Dictionary<(int Position, int Speed), RaceCarNode> _nodesByState = null!;
    private List<int> _speeds = null!;
    private RaceCarNode _start = null!;

    [GlobalSetup]
    public void Setup() => (_nodesByState, _speeds, _start) = RaceCarGraphs.BuildGraph(Target);

    [Benchmark(Baseline = true)]
    public int MutationQueueBfs()
    {
        var visited = new HashSet<(int Position, int Speed)> { (0, 1) };
        var queue = new Queue<(int Position, int Speed, int Commands)>();
        queue.Enqueue((0, 1, 0));

        while (queue.Count > 0)
        {
            var (position, speed, commands) = queue.Dequeue();

            if (position == Target)
            {
                return commands;
            }

            var acceleratePosition = position + speed;
            var accelerateSpeed = speed * 2;

            if (_nodesByState.ContainsKey((acceleratePosition, accelerateSpeed)) &&
                visited.Add((acceleratePosition, accelerateSpeed)))
            {
                queue.Enqueue((acceleratePosition, accelerateSpeed, commands + 1));
            }

            var reverseSpeed = speed > 0 ? -1 : 1;

            if (visited.Add((position, reverseSpeed)))
            {
                queue.Enqueue((position, reverseSpeed, commands + 1));
            }
        }

        return -1;
    }

    [Benchmark]
    public int ReduceGraphBfs()
    {
        var distances = Reduce.Graph<
            RaceCarNode, RaceCarTopology, ListChildren<RaceCarNode>,
            NaturalChildOrder<RaceCarNode, ListChildren<RaceCarNode>>, ListChildren<RaceCarNode>,
            BreadthFirstReduceOrder<RaceCarNode>,
            DistanceMapReduceAlgebra<RaceCarNode>, Dictionary<RaceCarNode, int>>(_start);

        var best = int.MaxValue;

        foreach (var speed in _speeds)
        {
            if (_nodesByState.TryGetValue((Target, speed), out var node) &&
                distances.TryGetValue(node, out var distance) && distance < best)
            {
                best = distance;
            }
        }

        return best;
    }
}
