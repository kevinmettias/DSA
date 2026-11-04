using DSAExperimentation.LeetCode.Solutions.SimilarStringGroups;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SimilarStringGroups;

public sealed partial class SimilarStringGroupsPromiseTests
{
    public static TheoryData<string[], bool> Inputs =>
        new()
        {
            { ["tars", "rats", "arts", "star"], true },
            { ["omv", "ovm"], true },
            { ["ab", "ba", "abc", "cab", "bca"], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheWordsAreAnagrams(string[] strs, bool expected)
        => Assert.Equal(expected, SimilarStringGroupsPromise.IsKeptBy(strs));
}
