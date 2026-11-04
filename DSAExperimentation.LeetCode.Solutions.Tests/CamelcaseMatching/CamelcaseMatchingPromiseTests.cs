using DSAExperimentation.LeetCode.Solutions.CamelcaseMatching;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CamelcaseMatching;

public sealed partial class CamelcaseMatchingPromiseTests
{
    public static TheoryData<string[], string, bool> Inputs =>
        new()
        {
            { ["FooBar", "FooBarTest"], "FB", true },
            { ["testCase1"], "testCase", false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryWordIsLetters(string[] queries, string pattern, bool expected)
        => Assert.Equal(expected, CamelcaseMatchingPromise.IsKeptBy(queries, pattern));
}
