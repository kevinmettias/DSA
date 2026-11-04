using DSAExperimentation.LeetCode.Solutions.FindKthBitInNthBinaryString;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindKthBitInNthBinaryString;

public sealed partial class FindKthBitInNthBinaryStringPromiseTests
{
    public static TheoryData<int, int, bool> Inputs =>
        new()
        {
            { 3, 1, true },
            { 4, 15, true },
            { 12, 4096, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheBitIsInTheString(int stringOrder, int bitPosition, bool expected)
        => Assert.Equal(expected, FindKthBitInNthBinaryStringPromise.IsKeptBy(stringOrder, bitPosition));
}
