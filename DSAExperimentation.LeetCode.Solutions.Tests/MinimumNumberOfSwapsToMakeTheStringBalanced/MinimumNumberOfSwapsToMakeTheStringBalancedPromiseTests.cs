using DSAExperimentation.LeetCode.Solutions.MinimumNumberOfSwapsToMakeTheStringBalanced;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumNumberOfSwapsToMakeTheStringBalanced;

public sealed partial class MinimumNumberOfSwapsToMakeTheStringBalancedPromiseTests
{
    public static TheoryData<string, bool> Inputs =>
        new()
        {
            { "][][", true },
            { "]]][[[", true },
            { "]][[[]]]", false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenOpenersMatchClosers(string text, bool expected)
        => Assert.Equal(expected, MinimumNumberOfSwapsToMakeTheStringBalancedPromise.IsKeptBy(text));
}
