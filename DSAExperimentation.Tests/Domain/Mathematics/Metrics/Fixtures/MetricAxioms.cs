using DSAExperimentation.Domain.Mathematics.Metrics;

namespace DSAExperimentation.Tests.Domain.Mathematics.Metrics.Fixtures;

// The three axioms a distance must satisfy to be a metric, read through the norm contract alone so
// every preset and the general form are held to the same facts, on every sampled point, pair and
// triple: the distance is zero exactly from a point to itself, it is the same both ways, and it is
// never longer than a detour through a third point. A root rounds in its last bits, so a detour may
// come out shorter by a rounding error and no more.
internal static class MetricAxioms
{
    private const double RoundingAllowance = 1e-9;

    public static void AssertHoldFor<TNorm>()
        where TNorm : struct, ILpNorm
    {
        foreach (var (_, points) in SampledPoints.ByDimension)
        {
            foreach (var first in points)
            {
                AssertPairs<TNorm>(first, points);
            }
        }
    }

    // Every axiom that begins at one point: its distances to each point, and through each to each.
    private static void AssertPairs<TNorm>(double[] first, double[][] points)
        where TNorm : struct, ILpNorm
    {
        Assert.Equal(0.0, TNorm.Distance(first, first));

        foreach (var second in points)
        {
            var forward = TNorm.Distance(first, second);
            var isSamePoint = first.SequenceEqual(second);

            Assert.Equal(forward, TNorm.Distance(second, first));
            Assert.Equal(isSamePoint, forward == 0.0);
            AssertNoShorterDetour<TNorm>(first, second, forward, points);
        }
    }

    // No detour from the first point to the second through any sampled point is shorter.
    private static void AssertNoShorterDetour<TNorm>(
        double[] first, double[] second, double direct, double[][] points)
        where TNorm : struct, ILpNorm
    {
        foreach (var through in points)
        {
            var detour = TNorm.Distance(first, through) + TNorm.Distance(through, second);

            Assert.True(direct <= detour + RoundingAllowance, $"{direct} > {detour}");
        }
    }
}
