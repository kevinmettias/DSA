using DSAExperimentation.LeetCode.Solutions.SortingTheSentence;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SortingTheSentence;

public sealed partial class SortingTheSentencePromiseTests
{
    public static TheoryData<string, bool> Inputs =>
        new()
        {
            { "is2 sentence4 This1 a3", true },
            { "Myself2 Me1 I4 and3", true },
            { "nine9 eight8 seven7 six6 five5 four4 three3 two2 one1 zero0", false },
            { "a1 b1", false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheWordsAreNumberedFromOne(string sentence, bool expected)
        => Assert.Equal(expected, SortingTheSentencePromise.IsKeptBy(sentence));
}
