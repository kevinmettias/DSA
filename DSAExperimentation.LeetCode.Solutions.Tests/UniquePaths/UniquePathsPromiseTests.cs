using DSAExperimentation.LeetCode.Solutions.UniquePaths;

namespace DSAExperimentation.LeetCode.Solutions.Tests.UniquePaths;

public sealed partial class UniquePathsPromiseTests
{
    public static TheoryData<int, int, bool> Inputs =>
        new()
        {
            { 3, 7, true },
            { 3, 2, true },
            { 1, 1, true },
            { 20, 30, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheCountFitsAnInt(int rowCount, int columnCount, bool expected)
        => Assert.Equal(expected, UniquePathsPromise.IsKeptBy(rowCount, columnCount));
}
