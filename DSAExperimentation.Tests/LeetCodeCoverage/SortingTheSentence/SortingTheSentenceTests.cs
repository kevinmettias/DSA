using DSAExperimentation.LeetCode.SortingTheSentence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SortingTheSentence;

// Harness only. Both strategies are SortingTheSentenceSolution's - this file pins
// them to LeetCode's published examples in LeetCode's own single-string input
// shape, plus the one-word case, a two-word swap, and a fully reversed nine-word
// sentence at the problem's upper bound.
public sealed class SortingTheSentenceTests
{
    public static TheoryData<SentenceExample> Examples =>
        new()
        {
            new SentenceExample("is2 sentence4 This1 a3", "This is a sentence"),
            new SentenceExample("Myself2 Me1 I4 and3", "Me Myself and I"),
            new SentenceExample("Hello1", "Hello"),
            new SentenceExample("world2 hello1", "hello world"),
            new SentenceExample("i9 h8 g7 f6 e5 d4 c3 b2 a1", "a b c d e f g h i"),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SortSentenceByMergeSort_LeetCodeExamples_ReconstructsOriginalOrder(SentenceExample example) =>
        Assert.Equal(example.Expected, SortingTheSentenceSolution.SortSentenceByMergeSort(example.Shuffled));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SortSentenceByPositionScan_LeetCodeExamples_ReconstructsOriginalOrder(SentenceExample example) =>
        Assert.Equal(example.Expected, SortingTheSentenceSolution.SortSentenceByPositionScan(example.Shuffled));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one LeetCode example,
    // naming which sentence goes in and which one is expected back. Passing the two
    // strings separately would let a caller transpose them silently.
    public readonly record struct SentenceExample(string Shuffled, string Expected);
}
