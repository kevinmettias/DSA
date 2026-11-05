using DSAExperimentation.LeetCode.Solutions.TrafficSignalColor;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TrafficSignalColor;

// Harness only. The classification lives in TrafficSignalColorSolution - this
// file pins it to the boundary values its own
// range check has to get right (0, 30 and 90 inclusive; 91 and 1000 excluded).
public sealed partial class TrafficSignalColorSolutionTests
{
    public static TheoryData<int, string> Cases =>
        new()
        {
            { 0, "Green" },
            { 30, "Orange" },
            { 90, "Red" },
            { 91, "Invalid" },
            { 1000, "Invalid" },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void ColorByRangeCheck_Cases_ReturnsSignalState(int timer, string expected) =>
        Assert.Equal(expected, TrafficSignalColorSolution.ColorByRangeCheck(timer));
}
