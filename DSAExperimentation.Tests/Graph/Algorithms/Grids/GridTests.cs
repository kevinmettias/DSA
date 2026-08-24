using DSAExperimentation.Graph.Algorithms.Grids;

namespace DSAExperimentation.Tests.Graph.Algorithms.Grids;

public sealed partial class GridTests
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
}
