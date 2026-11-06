using DSAExperimentation.LeetCode.Solutions.PalindromePairs;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PalindromePairs;

public sealed partial class PalindromePairsPromiseTests
{
    public static TheoryData<string[]> KeptInputs => [["abcd", "dcba", "lls", "s", "sssll"], [""]];

    // The empty word given twice, and a word with a digit.
    public static TheoryData<string[]> BrokenInputs => [["race", "", ""], ["a1b", "b1a"]];

    [Theory]
    [MemberData(nameof(KeptInputs))]
    public void IsKeptBy_UniqueLowercaseWords_IsTrue(string[] words) => Assert.True(PalindromePairsPromise.IsKeptBy(words));

    [Theory]
    [MemberData(nameof(BrokenInputs))]
    public void IsKeptBy_RepeatedOrNonLetterWord_IsFalse(string[] words) => Assert.False(PalindromePairsPromise.IsKeptBy(words));
}
