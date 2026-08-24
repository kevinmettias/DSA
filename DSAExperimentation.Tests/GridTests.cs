using DSAExperimentation.Graph;

namespace DSAExperimentation.Tests;

public sealed class GridTests
{
    // . . .
    // . # .
    // . . .
    // (#) is a wall; every other cell is open.
    private static Grid GridWithCenterWall() => new(new[,]
    {
        { true, true, true },
        { true, false, true },
        { true, true, true },
    });

    [Fact]
    public void Distance_RoutesAroundAWall()
    {
        var grid = GridWithCenterWall();
        var start = new GridNode(1, 0, grid);
        var target = new GridNode(1, 2, grid);

        // Manhattan distance is 2 (straight across row 1), but (1,1) is walled off
        // and every shortest unobstructed path happens to cross it, so the open
        // route has to detour via row 0 or row 2 - proving GridChildren is actually
        // filtering on Grid.IsPassable, not just computing raw neighbors.
        var distance = GridShortestPath.Distance(start, target);

        Assert.Equal(4, distance);
    }

    [Fact]
    public void Distance_UnreachableTarget_ReturnsNull()
    {
        var passable = new[,] { { true, true }, { false, true } };
        var grid = new Grid(passable);
        var start = new GridNode(0, 0, grid);
        var unreachable = new GridNode(5, 5, grid);

        var distance = GridShortestPath.Distance(start, unreachable);

        Assert.Null(distance);
    }

    [Fact]
    public void Distance_StartEqualsTarget_ReturnsZero()
    {
        var grid = GridWithCenterWall();
        var start = new GridNode(1, 0, grid);

        var distance = GridShortestPath.Distance(start, start);

        Assert.Equal(0, distance);
    }

    [Fact]
    public void ConnectedComponents_CountsIslands()
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

        var islands = ConnectedComponents.Count<
            GridNode, GridTopology, GridChildren,
            NaturalChildOrder<GridNode, GridChildren>, GridChildren,
            DepthFirstReduceOrder<GridNode>>(land);

        Assert.Equal(2, islands);
    }
}
