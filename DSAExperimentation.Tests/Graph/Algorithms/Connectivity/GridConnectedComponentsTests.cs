using DSAExperimentation.Graph.Algorithms.Connectivity;
using DSAExperimentation.Graph.Algorithms.Grids;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Graph.Engines.Reducing;

namespace DSAExperimentation.Tests.Graph.Algorithms.Connectivity;

public sealed partial class GridConnectedComponentsTests
{
    [Fact]
    public void Count_GridIslands_CountsDiagonallySeparatedComponents()
    {
        // 1 1 0
        // 0 1 0
        // 0 0 1
        // "Land" (passable) cells (0,0)-(0,1)-(1,1) are 4-directionally connected
        // into one island; (2,2) is diagonal-only from (1,1), so it's a second,
        // separate island - ties GridTopology together with the multi-root guard
        // ConnectedComponents relies on.
        var grid = new Grid(new[,]
        {
            { true, true, false },
            { false, true, false },
            { false, false, true },
        });

        var land = PassableCells(grid);

        var islands = ConnectedComponents.Count<
            GridNode, GridTopology, GridChildren,
            NaturalChildOrder<GridNode, GridChildren>, GridChildren,
            DepthFirstReduceOrder<GridNode>>(land);

        Assert.Equal(2, islands);
    }

    private static List<GridNode> PassableCells(Grid grid)
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
}
