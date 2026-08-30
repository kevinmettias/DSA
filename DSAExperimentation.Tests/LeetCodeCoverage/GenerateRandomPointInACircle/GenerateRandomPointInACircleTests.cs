namespace DSAExperimentation.Tests.LeetCodeCoverage.GenerateRandomPointInACircle;

// LeetCode 478. Generate Random Point in a Circle: a closed-form polar draw
// (r = radius * sqrt(u), theta = u * 2*PI - the sqrt is what keeps the sample
// uniform by AREA instead of clustering near the center) over a seeded
// System.Random, the same randomness primitive ShuffleAnArray/RandomPickIndex/
// LinkedListRandomNode already use. No other DataStructures/Algorithms type
// applies: the problem has no storage/topology to add on top of that single draw,
// the same "Operations + open runtime object, no Representation axis" shape
// ARCHITECTURE.md documents for Algorithms/Traversal/DepthFirst/DepthFirstSearch.cs.
public sealed partial class GenerateRandomPointInACircleTests
{
    [Fact]
    public void RandPoint_ManyDraws_AlwaysLandsWithinTheCircle()
    {
        var solution = new Solution(radius: 10.0, xCenter: 5.0, yCenter: -3.0, seed: 1);

        for (var i = 0; i < 500; i++)
        {
            var point = solution.RandPoint();
            var dx = point[0] - 5.0;
            var dy = point[1] - -3.0;

            Assert.True((dx * dx) + (dy * dy) <= (10.0 * 10.0) + 1e-9);
        }
    }

    [Fact]
    public void RandPoint_ManyDraws_ProducesVariedPoints()
    {
        var solution = new Solution(radius: 1.0, xCenter: 0.0, yCenter: 0.0, seed: 2);

        var distinct = new HashSet<(double X, double Y)>();
        for (var i = 0; i < 200; i++)
        {
            var point = solution.RandPoint();
            distinct.Add((point[0], point[1]));
        }

        Assert.True(distinct.Count > 190);
    }

    [Fact]
    public void RandPoint_ZeroRadius_AlwaysReturnsTheCenter()
    {
        var solution = new Solution(radius: 0.0, xCenter: 4.0, yCenter: 7.0, seed: 3);

        var point = solution.RandPoint();

        Assert.Equal(4.0, point[0]);
        Assert.Equal(7.0, point[1]);
    }

    private sealed class Solution(double radius, double xCenter, double yCenter, int seed)
    {
        private readonly Random _random = new(seed);

        public double[] RandPoint()
        {
            var r = radius * Math.Sqrt(_random.NextDouble());
            var angle = _random.NextDouble() * 2 * Math.PI;
            return [xCenter + (r * Math.Cos(angle)), yCenter + (r * Math.Sin(angle))];
        }
    }
}
