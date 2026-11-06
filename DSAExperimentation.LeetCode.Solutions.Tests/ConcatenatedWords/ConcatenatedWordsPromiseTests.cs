using DSAExperimentation.LeetCode.Solutions.ConcatenatedWords;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ConcatenatedWords;

public sealed partial class ConcatenatedWordsPromiseTests
{
    public static TheoryData<string[]> KeptInputs => [["cat", "cats", "catsdog"], ["a"]];

    // A word given twice, and a word with a digit.
    public static TheoryData<string[]> BrokenInputs => [["sun", "star", "star"], ["a1", "a"]];

    [Theory]
    [MemberData(nameof(KeptInputs))]
    public void IsKeptBy_UniqueLowercaseWords_IsTrue(string[] words) => Assert.True(ConcatenatedWordsPromise.IsKeptBy(words));

    [Theory]
    [MemberData(nameof(BrokenInputs))]
    public void IsKeptBy_RepeatedOrNonLetterWord_IsFalse(string[] words) => Assert.False(ConcatenatedWordsPromise.IsKeptBy(words));
}
