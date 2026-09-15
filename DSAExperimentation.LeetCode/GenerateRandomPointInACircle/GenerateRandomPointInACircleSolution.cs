namespace DSAExperimentation.LeetCode.GenerateRandomPointInACircle;

// LeetCode 478. Generate Random Point in a Circle: Solution(radius, x_center,
// y_center) constructs once; RandPoint() is called repeatedly, each time
// returning a point uniformly distributed inside the circle.
//
// A Design problem's whole point is a sequence of calls against one instance
// (RandPoint alone, this time), so "every strategy for the problem" (§17.3)
// takes the form of two classes implementing a shared IRandomPointGenerator
// surface, the same shape RandomPickIndexSolution already uses for its own
// Design-category problem. IRandomPointGenerator is bespoke to this problem
// alone, so it stays here rather than in DataStructures/.
//
// Neither strategy has a Representation/Topology axis to add a repo data
// structure on top of - each RandPoint() call is one self-contained draw over
// System.Random, the same "Operations + open runtime object, no Representation
// axis" shape ARCHITECTURE.md documents for Traversal/DepthFirst/DepthFirstSearch.
internal static class GenerateRandomPointInACircleSolution
{
    internal interface IRandomPointGenerator
    {
        double[] RandPoint();
    }

    // The textbook baseline: draw uniformly over the bounding square and reject
    // (redraw) any point outside the circle - roughly 1 - pi/4 (~21%) of draws
    // are discarded. Deliberately written without this repo's primitives, the
    // arm the closed-form single-draw strategy below has to justify itself
    // against.
    internal sealed class GenerateRandomPointInACircleByRejectionSampling(double radius, double xCenter, double yCenter) : IRandomPointGenerator
    {
        private readonly Random _random = new();

        public double[] RandPoint()
        {
            double x, y;

            do
            {
                x = (_random.NextDouble() * 2.0 * radius) - radius;
                y = (_random.NextDouble() * 2.0 * radius) - radius;
            }
            while ((x * x) + (y * y) > radius * radius);

            return [xCenter + x, yCenter + y];
        }
    }

    // The closed-form answer: a single polar draw always lands inside the
    // circle on the first try - r = radius * sqrt(u) is what keeps the sample
    // uniform by AREA instead of clustering near the center, theta = u * 2*PI
    // picks the direction. No rejection loop, no wasted draws.
    internal sealed class GenerateRandomPointInACircleByClosedFormPolar(double radius, double xCenter, double yCenter) : IRandomPointGenerator
    {
        private readonly Random _random = new();

        public double[] RandPoint()
        {
            var r = radius * Math.Sqrt(_random.NextDouble());
            var angle = _random.NextDouble() * 2.0 * Math.PI;

            return [xCenter + (r * Math.Cos(angle)), yCenter + (r * Math.Sin(angle))];
        }
    }
}
