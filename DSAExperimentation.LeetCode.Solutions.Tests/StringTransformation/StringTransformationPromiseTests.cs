using DSAExperimentation.LeetCode.Solutions.StringTransformation;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StringTransformation;

public sealed partial class StringTransformationPromiseTests
{
    public static TheoryData<string, string, long, bool> Inputs =>
        new()
        {
            { "abcd", "cdab", 2L, true },
            { "ababab", "ababab", 1L, true },
            { "mississippi", "ississippimis", 6L, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForStringsOfOneLength(string source, string target, long operations, bool expected)
        => Assert.Equal(expected, StringTransformationPromise.IsKeptBy(source, target, operations));
}
