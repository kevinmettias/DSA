using DSAExperimentation.LeetCode.Solutions.SpecialBinaryString;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SpecialBinaryString;

public sealed partial class SpecialBinaryStringPromiseTests
{
    public static TheoryData<string, bool> Inputs =>
        new()
        {
            { "11011000", true },
            { "10", true },
            { "1111000010001100", false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForASpecialString(string specialString, bool expected)
        => Assert.Equal(expected, SpecialBinaryStringPromise.IsKeptBy(specialString));
}
