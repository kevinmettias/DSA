using DSAExperimentation.LeetCode.Solutions.ConstructStringWithMinimumCost;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ConstructStringWithMinimumCost;

public sealed partial class ConstructStringWithMinimumCostPromiseTests
{
    public static TheoryData<string, string[], int[], bool> Inputs =>
        new()
        {
            { "abcdef", ["abdef", "abc", "d", "def", "ef"], [100, 1, 1, 10, 5], true },
            { "ab", ["a", "b"], [1], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryWordHasOneCost(string target, string[] words, int[] costs, bool expected)
        => Assert.Equal(expected, ConstructStringWithMinimumCostPromise.IsKeptBy(target, words, costs));
}
