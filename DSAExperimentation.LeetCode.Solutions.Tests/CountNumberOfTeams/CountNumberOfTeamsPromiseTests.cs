using DSAExperimentation.LeetCode.Solutions.CountNumberOfTeams;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountNumberOfTeams;

public sealed partial class CountNumberOfTeamsPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [2, 5, 3, 4, 1], true },
            { [1, 2, 2, 3], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryRatingIsUnique(int[] rating, bool expected)
        => Assert.Equal(expected, CountNumberOfTeamsPromise.IsKeptBy(rating));
}
