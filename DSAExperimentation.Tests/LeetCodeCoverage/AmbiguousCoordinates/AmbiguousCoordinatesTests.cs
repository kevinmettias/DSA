using DSAExperimentation.LeetCode.AmbiguousCoordinates;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AmbiguousCoordinates;

// Harness only: both strategies live in AmbiguousCoordinatesSolution and are
// asserted against the same examples - all four of LeetCode's published ones,
// which between them cover a clean digit run, a leading zero that only "0" itself
// can carry, a run where both zero rules bite at once, and a trailing zero that
// leaves exactly one answer. Order is not part of LeetCode's answer, so examples
// compare as sets.
public sealed class AmbiguousCoordinatesTests
{
    public static TheoryData<string, string[]> Examples =>
        new()
        {
            { "(123)", ["(1, 23)", "(12, 3)", "(1.2, 3)", "(1, 2.3)"] },
            {
                "(0123)",
                ["(0, 123)", "(0, 12.3)", "(0, 1.23)", "(0.1, 23)", "(0.1, 2.3)", "(0.12, 3)"]
            },
            { "(00011)", ["(0, 0.011)", "(0.001, 1)"] },
            { "(100)", ["(10, 0)"] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindCoordinatesByRebuildAndRescan_LeetCodeExamples_ReturnsEveryValidPlacement(
        string wrappedDigits, string[] expected) =>
        Assert.Equal(
            new HashSet<string>(expected),
            new HashSet<string>(AmbiguousCoordinatesSolution.FindCoordinatesByRebuildAndRescan(wrappedDigits)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindCoordinatesBySliceAndCheck_LeetCodeExamples_ReturnsEveryValidPlacement(
        string wrappedDigits, string[] expected) =>
        Assert.Equal(
            new HashSet<string>(expected),
            new HashSet<string>(AmbiguousCoordinatesSolution.FindCoordinatesBySliceAndCheck(wrappedDigits)));
}
