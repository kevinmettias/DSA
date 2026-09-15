namespace DSAExperimentation.Benchmarks.Fixtures;

// The rectangle the LC 3235 scenario RectangleCornerWorkloads scatters its circles across.
// Separate from RectangleCornerWorkloads because these are the scenario's values, which a
// benchmark has to name to drive the same case, not part of how the circles are generated.
internal static class RectangleCornerScenario
{
    public const int XCorner = 2_000;
    public const int YCorner = 2_000;
}
