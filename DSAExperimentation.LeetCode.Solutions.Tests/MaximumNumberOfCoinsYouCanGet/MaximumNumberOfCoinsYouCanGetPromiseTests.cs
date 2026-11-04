using DSAExperimentation.LeetCode.Solutions.MaximumNumberOfCoinsYouCanGet;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumNumberOfCoinsYouCanGet;

public sealed partial class MaximumNumberOfCoinsYouCanGetPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [2, 4, 1, 2, 7, 8], true },
            { [9, 8, 7, 6, 5, 1, 2, 3, 4], true },
            { [1, 2, 3, 4], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForWholeRoundsOfThree(int[] piles, bool expected)
        => Assert.Equal(expected, MaximumNumberOfCoinsYouCanGetPromise.IsKeptBy(piles));
}
