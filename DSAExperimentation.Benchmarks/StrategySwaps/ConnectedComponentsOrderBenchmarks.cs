using DSAExperimentation.Algorithms.Connectivity;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.Benchmarks.StrategySwaps;

// Many small components rather than one large one: ConnectedComponents enters Reduce.Graph once per
// component through its multi-root overload, so whatever a graph walk costs to start - the visit
// guard, the first frame or queue - is paid once per island here, thousands of times per run. The
// grid is 2x2 land islands separated by one-cell water lanes, deterministic so the workload rebuilds
// identically; both orders count the same islands, because which component a cell belongs to does
// not depend on the order its neighbours are visited in.
public class ConnectedComponentsOrderBenchmarks
{
    // Two land cells, then one water cell, along both axes.
    private const int LanePeriod = 3;
    private const int WaterOffset = LanePeriod - 1;

    private List<GridNode> _land = null!;

    [Params(100, 400)]
    public int Side { get; set; }

    [GlobalSetup]
    public void Setup() => _land = IslandCells(Side);

    [Benchmark(Baseline = true)]
    public int DepthFirst()
        => ConnectedComponents.Count<
            GridNode, GridTopology, GridChildren,
            DepthFirstReduceOrder<GridNode>>(_land);

    [Benchmark]
    public int BreadthFirst()
        => ConnectedComponents.Count<
            GridNode, GridTopology, GridChildren,
            BreadthFirstReduceOrder<GridNode>>(_land);

    private static List<GridNode> IslandCells(int side) => LandCells(new Grid(IslandMap(side)));

    private static bool[,] IslandMap(int side)
    {
        var passable = new bool[side, side];

        for (var row = 0; row < side; row++)
        {
            for (var col = 0; col < side; col++)
            {
                passable[row, col] = IsLand(row) && IsLand(col);
            }
        }

        return passable;
    }

    private static List<GridNode> LandCells(Grid grid)
    {
        var land = new List<GridNode>();

        for (var row = 0; row < grid.Rows; row++)
        {
            for (var col = 0; col < grid.Cols; col++)
            {
                if (grid.IsPassable(row, col))
                {
                    land.Add(new GridNode(row, col, grid));
                }
            }
        }

        return land;
    }

    private static bool IsLand(int coordinate) => coordinate % LanePeriod != WaterOffset;
}
