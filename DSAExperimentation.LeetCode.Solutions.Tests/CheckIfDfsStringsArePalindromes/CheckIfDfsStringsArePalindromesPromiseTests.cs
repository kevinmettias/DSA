using DSAExperimentation.LeetCode.Solutions.CheckIfDfsStringsArePalindromes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CheckIfDfsStringsArePalindromes;

public sealed partial class CheckIfDfsStringsArePalindromesPromiseTests
{
    public static TheoryData<int[], string, bool> Inputs =>
        new()
        {
            { [-1, 0, 0, 1, 1, 2], "aababa", true },
            { [-1, 0, 0, 1, 1, 2, 2], "aabbcc", false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenEveryNodeHasACharacter(int[] parent, string nodeCharacters, bool expected)
        => Assert.Equal(expected, CheckIfDfsStringsArePalindromesPromise.IsKeptBy(parent, nodeCharacters));
}
