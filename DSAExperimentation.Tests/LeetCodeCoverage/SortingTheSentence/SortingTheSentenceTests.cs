using DSAExperimentation.LeetCode.SortingTheSentence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SortingTheSentence;

// Harness only. Both strategies are SortingTheSentenceSolution's - this file pins
// them to LeetCode's published examples in LeetCode's own single-string input
// shape, plus the one-word case, a two-word swap, and a fully reversed nine-word
// sentence at the problem's upper bound.
public sealed class SortingTheSentenceTests
{
    public static TheoryData<string, string> Examples =>
        new()
        {
            { "is2 sentence4 This1 a3", "This is a sentence" },
            { "Myself2 Me1 I4 and3", "Me Myself and I" },
            { "Hello1", "Hello" },
            { "world2 hello1", "hello world" },
            { "i9 h8 g7 f6 e5 d4 c3 b2 a1", "a b c d e f g h i" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SortSentenceByMergeSort_LeetCodeExamples_ReconstructsOriginalOrder(
        string shuffled, string expected) =>
        Assert.Equal(expected, SortingTheSentenceSolution.SortSentenceByMergeSort(shuffled));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SortSentenceByPositionScan_LeetCodeExamples_ReconstructsOriginalOrder(
        string shuffled, string expected) =>
        Assert.Equal(expected, SortingTheSentenceSolution.SortSentenceByPositionScan(shuffled));
}
