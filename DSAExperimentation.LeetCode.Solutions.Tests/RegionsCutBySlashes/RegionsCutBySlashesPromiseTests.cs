using DSAExperimentation.LeetCode.Solutions.RegionsCutBySlashes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RegionsCutBySlashes;

public sealed partial class RegionsCutBySlashesPromiseTests
{
    public static TheoryData<string[], bool> Inputs =>
        new()
        {
            { [" /", "/ "], true },
            { ["/\\", "\\/"], true },
            { ["\\/", "\\/", "\\/"], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForASquareGrid(string[] grid, bool expected)
        => Assert.Equal(expected, RegionsCutBySlashesPromise.IsKeptBy(grid));
}
