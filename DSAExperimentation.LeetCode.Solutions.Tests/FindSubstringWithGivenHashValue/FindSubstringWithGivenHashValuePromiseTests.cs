using DSAExperimentation.DataStructures.RollingHash;
using DSAExperimentation.LeetCode.Solutions.FindSubstringWithGivenHashValue;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindSubstringWithGivenHashValue;

public sealed partial class FindSubstringWithGivenHashValuePromiseTests
{
    public static TheoryData<string, long, long, int, long, bool> Inputs =>
        new()
        {
            { "leetcode", 7, 20, 2, 0, true },
            { "fbxzaad", 31, 100, 3, 32, true },
            { "abcdefghijklmnopqrstuvwxyz", 26, 101, 5, 53, false },
            { "leet code", 7, 20, 2, 0, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForLettersWithAWindowOfTheHash(string text, long power, long modulus, int windowLength, long hashValue, bool expected)
        => Assert.Equal(expected, FindSubstringWithGivenHashValuePromise.IsKeptBy(text, new RollingHashLane(power, modulus), (windowLength, hashValue)));
}
