using DSAExperimentation.LeetCode.Solutions.MinimumCostToConvertStringI;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumCostToConvertStringI;

public sealed partial class MinimumCostToConvertStringIPromiseTests
{
    public static TheoryData<string, string, char[], char[], int[], bool> Inputs =>
        new()
        {
            { "abcd", "acbe", ['a', 'b', 'c', 'c', 'e', 'd'], ['b', 'c', 'b', 'e', 'b', 'e'], [2, 5, 5, 1, 2, 20], true },
            { "programming", "debugging", ['p'], ['d'], [1], false },
            { "abc", "xyz", ['a', 'b'], ['x', 'y'], [1], false },
            { "abc", "aBc", ['b'], ['B'], [1], false },
            { "abc", "abc", ['a'], ['a'], [1], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForEqualLengthsWholeRulesAndLowercaseLetters(
        string source, string target, char[] original, char[] changed, int[] cost, bool expected)
        => Assert.Equal(
            expected,
            MinimumCostToConvertStringIPromise.IsKeptBy(
                new MinimumCostToConvertStringISolution.SourceText(source),
                new MinimumCostToConvertStringISolution.TargetText(target),
                (original, changed, cost)));
}
