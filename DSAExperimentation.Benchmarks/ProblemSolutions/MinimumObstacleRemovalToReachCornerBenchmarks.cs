using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Obstacle Removal to Reach Corner (LC 2290): entering an obstacle cell
// costs 1, an empty cell costs 0 - a 0/1-weighted grid shortest path. This compares
// the textbook O(V^2) linear-scan Dijkstra (no priority queue - the classic
// "before you reach for a heap" baseline) against this repo's own
// ShortestPath.Dijkstra (Collections/Heap-backed, O(E log V)) over the same
// WeightedGridNode/WeightedGridTopology grid ShortestPathAlgorithmBenchmarks
// already uses, with each edge's weight set from the *target* cell's obstacle
// flag instead of a uniform 1. Both variants only read the graph (settle/distance
// bookkeeping is local to each method), so it is safely built once in Setup rather
// than rebuilt per run.
[MemoryDiagnoser]
public class MinimumObstacleRemovalToReachCornerBenchmarks
{
    // 1-in-5 chance a cell is an obstacle.
    private const int ObstacleProbability = 5;
    private const int RandomSeed = 2290; // LC problem number

    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    [Params(15, 60)]
    public int Size;

    private List<WeightedGridNode> _nodes = null!;
    private WeightedGridNode _source = null!;
    private WeightedGridNode _target = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var (nodes, source, target) = BuildObstacleGrid(Size, random);
        _nodes = nodes;
        _source = source;
        _target = target;
    }

    [Benchmark(Baseline = true)]
    public int NaiveArrayScanDijkstra()
    {
        var distance = _nodes.ToDictionary(node => node, _ => int.MaxValue);
        var settled = new HashSet<WeightedGridNode>();
        distance[_source] = 0;

        for (var i = 0; i < _nodes.Count; i++)
        {
            var current = PickUnsettledMinimum(distance, settled);

            if (current is null)
            {
                break;
            }

            settled.Add(current);
            RelaxNeighbors(current, distance);
        }

        return distance[_target];
    }

    [Benchmark]
    public int RepoHeapDijkstra()
    {
        var distances = ShortestPath.Dijkstra<
            WeightedGridNode, WeightedGridTopology, ListEdges<WeightedGridNode, int>, int>(_source);

        return distances[_target];
    }

    private static (List<WeightedGridNode> Nodes, WeightedGridNode Source, WeightedGridNode Target) BuildObstacleGrid(int size, Random random)
    {
        var (grid, obstacle) = CreateGridAndObstacles(size, random);

        WireEdges(grid, obstacle, size);

        var nodes = CollectNodes(grid);

        return (nodes, grid[0, 0], grid[size - 1, size - 1]);
    }

    private static (WeightedGridNode[,] Grid, bool[,] Obstacle) CreateGridAndObstacles(int size, Random random)
    {
        var grid = new WeightedGridNode[size, size];
        var obstacle = new bool[size, size];

        for (var row = 0; row < size; row++)
        {
            for (var col = 0; col < size; col++)
            {
                grid[row, col] = new WeightedGridNode(row, col);
                obstacle[row, col] = random.Next(ObstacleProbability) == 0;
            }
        }

        obstacle[0, 0] = false;
        obstacle[size - 1, size - 1] = false;

        return (grid, obstacle);
    }

    private static List<WeightedGridNode> CollectNodes(WeightedGridNode[,] grid)
    {
        var nodes = new List<WeightedGridNode>(grid.Length);

        foreach (var node in grid)
        {
            nodes.Add(node);
        }

        return nodes;
    }

    private readonly record struct ObstacleGridWiring(WeightedGridNode[,] Nodes, bool[,] Obstacle, int Size);

    private static void WireEdges(WeightedGridNode[,] nodes, bool[,] obstacle, int size)
    {
        var wiring = new ObstacleGridWiring(nodes, obstacle, size);

        for (var row = 0; row < size; row++)
        {
            for (var col = 0; col < size; col++)
            {
                foreach (var direction in Directions)
                {
                    TryAddEdge(wiring, row, col, direction);
                }
            }
        }
    }

    private static void TryAddEdge(ObstacleGridWiring wiring, int row, int col, (int DRow, int DCol) direction)
    {
        var r = row + direction.DRow;
        var c = col + direction.DCol;

        if (r < 0 || r >= wiring.Size || c < 0 || c >= wiring.Size)
        {
            return;
        }

        var weight = wiring.Obstacle[r, c] ? 1 : 0;
        wiring.Nodes[row, col].Edges.Add((weight, wiring.Nodes[r, c]));
    }

    private static WeightedGridNode? PickUnsettledMinimum(
        Dictionary<WeightedGridNode, int> distance, HashSet<WeightedGridNode> settled)
    {
        WeightedGridNode? best = null;
        var bestDistance = int.MaxValue;

        foreach (var (node, nodeDistance) in distance)
        {
            if (!settled.Contains(node) && nodeDistance < bestDistance)
            {
                best = node;
                bestDistance = nodeDistance;
            }
        }

        return best;
    }

    private static void RelaxNeighbors(WeightedGridNode current, Dictionary<WeightedGridNode, int> distance)
    {
        foreach (var (weight, neighbor) in current.Edges)
        {
            var candidate = distance[current] + weight;

            if (candidate < distance[neighbor])
            {
                distance[neighbor] = candidate;
            }
        }
    }
}
