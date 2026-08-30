using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Sliding Puzzle (LC 773): the textbook mutate-the-blank BFS (Queue<(string,int)>
// plus a HashSet<string> visited set, generating each candidate slide on the fly)
// vs. this repo's own BFS - Reduce.Graph + DistanceMapReduceAlgebra over a
// precomputed PuzzleNode graph of all 720 board permutations, the same
// "distance to some specific target" composition OpenTheLockBenchmarks already uses
// for LC 752. StartState varies how many slides separate the board from "123450"
// (LC's own 1-move and 5-move examples), the same search-depth axis
// OpenTheLockBenchmarks' DeadendCount varies for the lock's Cayley graph.
[MemoryDiagnoser]
public class SlidingPuzzleBenchmarks
{
    [Params("123405", "412503")]
    public string StartState = null!;

    private Dictionary<string, PuzzleNode> _nodesByState = null!;
    private PuzzleNode _startNode = null!;

    [GlobalSetup]
    public void Setup() => (_nodesByState, _startNode) = PuzzleGraphs.BuildGraph(StartState);

    [Benchmark(Baseline = true)]
    public int MutationQueueBfs()
    {
        var visited = new HashSet<string> { StartState };
        var queue = new Queue<(string State, int Moves)>();
        queue.Enqueue((StartState, 0));

        while (queue.Count > 0)
        {
            var (state, moves) = queue.Dequeue();

            if (state == PuzzleGraphs.Target)
            {
                return moves;
            }

            foreach (var neighbor in PuzzleGraphs.BlankSlideNeighbors(state))
            {
                if (visited.Add(neighbor))
                {
                    queue.Enqueue((neighbor, moves + 1));
                }
            }
        }

        return -1;
    }

    [Benchmark]
    public int ReduceGraphBfs()
    {
        var distances = Reduce.Graph<
            PuzzleNode, PuzzleTopology, ListChildren<PuzzleNode>,
            NaturalChildOrder<PuzzleNode, ListChildren<PuzzleNode>>, ListChildren<PuzzleNode>,
            BreadthFirstReduceOrder<PuzzleNode>,
            DistanceMapReduceAlgebra<PuzzleNode>, Dictionary<PuzzleNode, int>>(_startNode);

        return _nodesByState.TryGetValue(PuzzleGraphs.Target, out var targetNode) &&
            distances.TryGetValue(targetNode, out var distance)
                ? distance
                : -1;
    }
}
