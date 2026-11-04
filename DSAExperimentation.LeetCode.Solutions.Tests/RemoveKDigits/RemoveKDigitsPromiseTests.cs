using DSAExperimentation.LeetCode.Solutions.RemoveKDigits;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RemoveKDigits;

public sealed partial class RemoveKDigitsPromiseTests
{
    public static TheoryData<string, int, bool> Inputs =>
        new()
        {
            { "1432219", 3, true },
            { "10", 2, true },
            { "100000", 7, false },
            { "0123", 1, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenKFitsTheDigits(string num, int removalCount, bool expected)
        => Assert.Equal(expected, RemoveKDigitsPromise.IsKeptBy(num, removalCount));
}
