using DSAExperimentation.LeetCode.Solutions.KokoEatingBananas;

namespace DSAExperimentation.LeetCode.Solutions.Tests.KokoEatingBananas;

public sealed partial class KokoEatingBananasPromiseTests
{
    public static TheoryData<int[], int, bool> Inputs =>
        new()
        {
            { [3, 6, 7, 11], 8, true },
            { [30, 11, 23, 4, 20], 5, true },
            { [1, 1, 1, 1, 1, 1], 5, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenThereAreHoursForEveryPile(int[] piles, int hourBudget, bool expected)
        => Assert.Equal(expected, KokoEatingBananasPromise.IsKeptBy(piles, hourBudget));
}
