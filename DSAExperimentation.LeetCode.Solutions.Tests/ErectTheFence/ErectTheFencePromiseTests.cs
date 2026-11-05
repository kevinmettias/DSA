using DSAExperimentation.LeetCode.Solutions.ErectTheFence;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ErectTheFence;

public sealed partial class ErectTheFencePromiseTests
{
    public static TheoryData<(int X, int Y)[], bool> Inputs =>
        new()
        {
            { [(1, 1), (2, 2), (2, 0), (2, 4), (3, 3), (4, 2)], true },
            { [(1, 2), (2, 2), (1, 2)], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenNoPositionRepeats((int X, int Y)[] points, bool expected)
        => Assert.Equal(expected, ErectTheFencePromise.IsKeptBy(points));
}
