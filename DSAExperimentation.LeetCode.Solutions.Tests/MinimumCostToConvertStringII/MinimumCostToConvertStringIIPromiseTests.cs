using DSAExperimentation.LeetCode.Solutions.MinimumCostToConvertStringII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumCostToConvertStringII;

public sealed partial class MinimumCostToConvertStringIIPromiseTests
{
    public static TheoryData<string, string, string[], string[], int[], bool> Inputs =>
        new()
        {
            { "abcd", "acbe", ["a", "b", "c", "c", "e", "d"], ["b", "c", "b", "e", "b", "e"], [2, 5, 5, 1, 2, 20], true },
            { "abcdefgh", "acdeeghh", ["bcd", "fgh", "thh"], ["cde", "thh", "ghh"], [1, 3, 5], true },
            { "mississippi", "missezzzzzzzzzz", ["i"], ["e"], [1], false },
            { "abc", "xyz", ["ab", "c"], ["xy", "z"], [1], false },
            { "abc", "xyz", ["ab"], ["x"], [1], false },
            { "abc", "abc", ["ab"], ["ab"], [1], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForEqualLengthsWholeRulesAndLowercaseLetters(
        string source, string target, string[] original, string[] changed, int[] cost, bool expected)
        => Assert.Equal(expected, MinimumCostToConvertStringIIPromise.IsKeptBy(new SourceText(source), new TargetText(target), (original, changed, cost)));
}
