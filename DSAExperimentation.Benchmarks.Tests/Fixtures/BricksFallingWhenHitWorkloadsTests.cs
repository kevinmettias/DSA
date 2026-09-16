using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for BricksFallingWhenHitWorkloads (ARCHITECTURE 17.7). The reading depends on
// the wall being square, the top row staying fully bricked, and every hit landing on a real,
// distinct brick position - which is what makes each hit do genuine connectivity work instead of
// a mostly no-op one.
public sealed partial class BricksFallingWhenHitWorkloadsTests
{
    private const int Size = 16;
    private const int Seed = 803; // LC problem number
    private const int BricksPerHit = 2;
    private const int Brick = 1;

    [Fact]
    public void BuildWall_Size_ReturnsASquareGrid()
    {
        var wall = BricksFallingWhenHitWorkloads.BuildWall(Size, Seed);

        Assert.Equal(Size, wall.Grid.Length);
        Assert.All(wall.Grid, row => Assert.Equal(Size, row.Length));
    }

    [Fact]
    public void BuildWall_TopRow_StaysFullyBricked()
    {
        var wall = BricksFallingWhenHitWorkloads.BuildWall(Size, Seed);

        Assert.All(wall.Grid[0], cell => Assert.Equal(Brick, cell));
    }

    [Fact]
    public void BuildWall_EveryHit_TargetsABrickInsideTheGrid()
    {
        var wall = BricksFallingWhenHitWorkloads.BuildWall(Size, Seed);

        foreach (var hit in wall.Hits)
        {
            Assert.Equal(BricksPerHit, hit.Length);
            Assert.InRange(hit[0], 0, Size - 1);
            Assert.InRange(hit[1], 0, Size - 1);
            Assert.Equal(Brick, wall.Grid[hit[0]][hit[1]]);
        }
    }

    [Fact]
    public void BuildWall_EveryHitPosition_IsDistinct()
    {
        var wall = BricksFallingWhenHitWorkloads.BuildWall(Size, Seed);

        Assert.Equal(wall.Hits.Length, wall.Hits.Select(hit => (hit[0], hit[1])).Distinct().Count());
    }

    [Fact]
    public void BuildWall_SameSeed_ReturnsTheSameWall()
    {
        var wall = BricksFallingWhenHitWorkloads.BuildWall(Size, Seed);
        var repeat = BricksFallingWhenHitWorkloads.BuildWall(Size, Seed);

        Assert.Equal(AnswerText.Of(wall.Grid), AnswerText.Of(repeat.Grid));
        Assert.Equal(AnswerText.Of(wall.Hits), AnswerText.Of(repeat.Hits));
    }
}
