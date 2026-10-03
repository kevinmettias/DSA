using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for MinimumCostOfAPathWithSpecialRoadsWorkloads (ARCHITECTURE 17.7). The reading
// depends on the workload being an input LC 2662 can pose: start no further along either axis than
// target, every special road's endpoints inside the square between them, and every cost inside the
// problem's [1, 10^5].
public sealed partial class MinimumCostOfAPathWithSpecialRoadsWorkloadsTests
{
    private const int RoadCount = 40;
    private const int CoordinateUpperBound = 1_000;
    private const int CostUpperBound = 500;
    private const int Seed = 2662; // LC problem number

    // A road's fields are X1, Y1, X2, Y2, Cost.
    private const int SecondEndpointXIndex = 2;
    private const int SecondEndpointYIndex = 3;
    private const int CostFieldIndex = 4;

    // LC 2662 bounds every coordinate and every cost to [1, 10^5].
    private const int ProblemUpperBound = 100_000;

    [Fact]
    public void Build_StartAndTarget_SitInsideTheProblemRangeWithStartFirstOnBothAxes()
    {
        var (start, target, _) = Build();

        Assert.All(start, coordinate => Assert.InRange(coordinate, 1, ProblemUpperBound));
        Assert.All(target, coordinate => Assert.InRange(coordinate, 1, ProblemUpperBound));
        Assert.True(start[0] <= target[0]);
        Assert.True(start[1] <= target[1]);
    }

    [Fact]
    public void Build_EveryRoadEndpoint_LiesInsideTheStartToTargetSquare()
    {
        var (start, target, roads) = Build();

        Assert.Equal(RoadCount, roads.Length);
        Assert.All(roads, road => Assert.Equal(CostFieldIndex + 1, road.Length));
        Assert.All(roads, road => Assert.InRange(road[0], start[0], target[0]));
        Assert.All(roads, road => Assert.InRange(road[1], start[1], target[1]));
        Assert.All(roads, road => Assert.InRange(road[SecondEndpointXIndex], start[0], target[0]));
        Assert.All(roads, road => Assert.InRange(road[SecondEndpointYIndex], start[1], target[1]));
    }

    [Fact]
    public void Build_EveryRoadCost_FallsInsideTheProblemsCostRange()
    {
        var (_, _, roads) = Build();

        Assert.All(roads, road => Assert.InRange(road[CostFieldIndex], 1, ProblemUpperBound));
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload()
    {
        var (start, target, roads) = Build();
        var (repeatStart, repeatTarget, repeatRoads) = Build();

        Assert.Equal(start, repeatStart);
        Assert.Equal(target, repeatTarget);
        Assert.Equal(AnswerGraphText.Of(roads), AnswerGraphText.Of(repeatRoads));
    }

    private static (int[] Start, int[] Target, int[][] SpecialRoads) Build() =>
        MinimumCostOfAPathWithSpecialRoadsWorkloads.Build(RoadCount, CoordinateUpperBound, CostUpperBound, Seed);
}
