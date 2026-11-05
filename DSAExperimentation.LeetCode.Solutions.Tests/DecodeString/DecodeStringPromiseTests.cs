using DSAExperimentation.LeetCode.Solutions.DecodeString;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DecodeString;

public sealed partial class DecodeStringPromiseTests
{
    public static TheoryData<string, bool> Inputs =>
        new()
        {
            { "3[a2[c]]", true },
            { "abc", true },
            { "10[a]", true },
            { "a[b]", false },
            { "2[a", false },
            { "a2", false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForAValidEncoding(string encoded, bool expected)
        => Assert.Equal(expected, DecodeStringPromise.IsKeptBy(encoded));
}
