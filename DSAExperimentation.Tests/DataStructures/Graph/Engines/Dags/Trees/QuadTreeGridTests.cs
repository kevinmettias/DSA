using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees;

public sealed partial class QuadTreeGridTests
{
    private const int TwoByTwo = 2;
    private const int FourByFour = 4;

    // The tree carries Val/isLeaf only - it never carries the grid's size, which is why every
    // Materialize call passes one. A leaf over a 3x3 grid is therefore legal, not a mismatch.
    private const int OddSize = 3;

    [Fact]
    public void Materialize_TrueLeaf_PaintsOnesAcrossTheWholeGrid()
    {
        var grid = QuadTreeGrid.Materialize(One(), TwoByTwo);

        Assert.Equal([[1, 1], [1, 1]], grid);
    }

    [Fact]
    public void Materialize_FalseLeaf_PaintsZeroesAcrossTheWholeGrid()
    {
        var grid = QuadTreeGrid.Materialize(Zero(), TwoByTwo);

        Assert.Equal([[0, 0], [0, 0]], grid);
    }

    [Fact]
    public void Materialize_LeafOfAnyGridSize_PaintsThatWholeGrid()
    {
        var grid = QuadTreeGrid.Materialize(One(), OddSize);

        Assert.Equal([[1, 1, 1], [1, 1, 1], [1, 1, 1]], grid);
    }

    [Fact]
    public void Materialize_MixedQuadrants_PlacesEachInItsOwnCorner()
    {
        var root = new QuadTreeNode(Val: true, IsLeaf: false)
        {
            TopLeft = One(),
            TopRight = One(),
            BottomLeft = Zero(),
            BottomRight = Zero(),
        };

        var grid = QuadTreeGrid.Materialize(root, TwoByTwo);

        Assert.Equal([[1, 1], [0, 0]], grid);
    }

    // Each leaf paints the whole region it covers, not one cell: a quadrant of a 4x4 grid is a
    // 2x2 block, which is the part a single-cell-per-leaf walk would get wrong.
    [Fact]
    public void Materialize_QuadrantsOfAFourByFourGrid_PaintTheirWholeBlocks()
    {
        var root = new QuadTreeNode(Val: true, IsLeaf: false)
        {
            TopLeft = One(),
            TopRight = Zero(),
            BottomLeft = Zero(),
            BottomRight = One(),
        };

        var grid = QuadTreeGrid.Materialize(root, FourByFour);

        Assert.Equal([[1, 1, 0, 0], [1, 1, 0, 0], [0, 0, 1, 1], [0, 0, 1, 1]], grid);
    }

    [Fact]
    public void Allocate_RequestedSize_ReturnsThatManyZeroRows()
    {
        var grid = QuadTreeGrid.Allocate(FourByFour);

        Assert.Equal(FourByFour, grid.Length);
        Assert.All(grid, row => Assert.Equal(FourByFour, row.Length));
        Assert.Equal([[0, 0, 0, 0], [0, 0, 0, 0], [0, 0, 0, 0], [0, 0, 0, 0]], grid);
    }

    [Fact]
    public void Allocate_Rows_AreIndependentArrays()
    {
        var grid = QuadTreeGrid.Allocate(TwoByTwo);

        grid[0][0] = 1;

        Assert.Equal(0, grid[1][0]);
    }

    private static QuadTreeNode One() => new(Val: true, IsLeaf: true);

    private static QuadTreeNode Zero() => new(Val: false, IsLeaf: true);
}
