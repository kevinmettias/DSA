using DSAExperimentation.LeetCode.TrafficSignalColor;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TrafficSignalColor;

// Harness only. The classification lives in TrafficSignalColorSolution - this
// file pins it to LeetCode's published examples plus the boundary values its own
// range check has to get right (0, 30 and 90 inclusive; 91 and 1000 excluded).
public sealed class TrafficSignalColorTests
{
    public static TheoryData<int, string> Examples =>
        new()
        {
            { 60, "Red" },
            { 5, "Invalid" },
            { 0, "Green" },
            { 30, "Orange" },
            { 90, "Red" },
            { 91, "Invalid" },
            { 1000, "Invalid" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ColorByRangeCheck_LeetCodeExamples_ReturnsSignalState(int timer, string expected) =>
        Assert.Equal(expected, TrafficSignalColorSolution.ColorByRangeCheck(timer));
}
