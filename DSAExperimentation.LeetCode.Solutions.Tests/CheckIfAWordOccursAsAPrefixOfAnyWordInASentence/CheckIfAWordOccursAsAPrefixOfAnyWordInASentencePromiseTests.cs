using DSAExperimentation.LeetCode.Solutions.CheckIfAWordOccursAsAPrefixOfAnyWordInASentence;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CheckIfAWordOccursAsAPrefixOfAnyWordInASentence;

public sealed partial class CheckIfAWordOccursAsAPrefixOfAnyWordInASentencePromiseTests
{
    public static TheoryData<string, string, bool> Inputs =>
        new()
        {
            { "i love eating burger", "burg", true },
            { "python java c++ c#", "c", false },
            { "hello world", "Wor", false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForLowercaseWordsAndSpaces(string sentence, string searchWord, bool expected)
        => Assert.Equal(
            expected,
            CheckIfAWordOccursAsAPrefixOfAnyWordInASentencePromise.IsKeptBy(
                new CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.SentenceText(sentence),
                new CheckIfAWordOccursAsAPrefixOfAnyWordInASentenceSolution.SearchedPrefix(searchWord)));
}
