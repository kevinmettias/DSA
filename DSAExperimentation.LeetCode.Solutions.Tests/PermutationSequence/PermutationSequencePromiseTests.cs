using DSAExperimentation.LeetCode.Solutions.PermutationSequence;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PermutationSequence;

public sealed partial class PermutationSequencePromiseTests
{
    public static TheoryData<int, int, bool> Inputs =>
        new()
        {
            { 3, 3, true },
            { 4, 9, true },
            { 8, 40_320, true },
            { 8, 40_321, false },
            { 3, 0, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheRankNamesOneOfTheNFactorialPermutations(int digitCount, int rank, bool expected)
        => Assert.Equal(expected, PermutationSequencePromise.IsKeptBy(digitCount, rank));
}
