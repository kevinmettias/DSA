using DSAExperimentation.LeetCode.Solutions.IsomorphicStrings;

namespace DSAExperimentation.LeetCode.Solutions.Tests.IsomorphicStrings;

public sealed partial class IsomorphicStringsPromiseTests
{
    public static TheoryData<string, string, bool> Inputs =>
        new()
        {
            { "egg", "add", true },
            { "ab", "abc", false },
            { "\u00e9", "a", false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForTwoAsciiStringsOfOneLength(string source, string target, bool expected)
        => Assert.Equal(expected, IsomorphicStringsPromise.IsKeptBy(source, target));
}
