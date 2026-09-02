using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Reverse Operations (LC 2612): BruteForceScan tests every one of the n
// candidate destinations for each position popped off the BFS frontier (O(n) per
// pop), while ReduceGraph composes this repo's own Reduce.Graph over
// ReversalTopology, whose ReversalChildren computes only the O(K) positions actually
// reachable in one reversal directly from the window arithmetic - so the gap between
// the two arms widens as K shrinks relative to n.
[MemoryDiagnoser]
public class MinimumReverseOperationsBenchmarks
{
    // Small relative to NodeCount, so ReduceGraph's O(K)-per-pop children are a real
    // fraction of BruteForceScan's O(n)-per-pop full scan.
    private const int WindowSize = 8;
    private const int StartPosition = 0;

    [Params(200, 4_000)]
    public int NodeCount;

    [Benchmark(Baseline = true)]
    public int[] BruteForceScan()
    {
        var distances = new int[NodeCount];
        Array.Fill(distances, -1);
        distances[StartPosition] = 0;

        var queue = new Queue<int>();
        queue.Enqueue(StartPosition);

        while (queue.Count > 0)
        {
            var from = queue.Dequeue();

            for (var to = 0; to < NodeCount; to++)
            {
                if (distances[to] != -1 || !IsReachableInOneReversal(from, to))
                {
                    continue;
                }

                distances[to] = distances[from] + 1;
                queue.Enqueue(to);
            }
        }

        return distances;
    }

    private bool IsReachableInOneReversal(int from, int to)
    {
        var numerator = from + to - WindowSize + 1;

        if ((numerator & 1) != 0)
        {
            return false;
        }

        var windowStart = numerator / 2;
        var earliestStart = Math.Max(0, from - WindowSize + 1);
        var latestStart = Math.Min(from, NodeCount - WindowSize);

        return windowStart >= earliestStart && windowStart <= latestStart;
    }

    [Benchmark]
    public int[] ReduceGraph()
    {
        var board = new ReversalBoard(NodeCount, WindowSize, new Set<int>());
        var source = new PositionNode(StartPosition, board);

        var distanceByNode = Reduce.Graph<
            PositionNode, ReversalTopology, ReversalChildren,
            NaturalChildOrder<PositionNode, ReversalChildren>, ReversalChildren,
            BreadthFirstReduceOrder<PositionNode>,
            DistanceMapReduceAlgebra<PositionNode>, Dictionary<PositionNode, int>>(source);

        var answer = new int[NodeCount];

        for (var position = 0; position < NodeCount; position++)
        {
            answer[position] = distanceByNode.TryGetValue(new PositionNode(position, board), out var distance)
                ? distance
                : -1;
        }

        return answer;
    }
}
