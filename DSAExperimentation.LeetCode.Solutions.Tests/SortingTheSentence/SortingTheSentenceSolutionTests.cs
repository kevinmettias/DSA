using DSAExperimentation.LeetCode.Solutions.SortingTheSentence;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SortingTheSentence;

// Harness only. Both strategies are SortingTheSentenceSolution's - this file pins
// them, in LeetCode's own single-string input shape, to the one-word case, a
// two-word swap, and a fully reversed nine-word sentence at the problem's upper
// bound. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class SortingTheSentenceSolutionTests
{
    public static TheoryData<SentenceCase> Cases =>
        new()
        {
            new SentenceCase("Hello1", "Hello"),
            new SentenceCase("world2 hello1", "hello world"),
            new SentenceCase("i9 h8 g7 f6 e5 d4 c3 b2 a1", "a b c d e f g h i"),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SortSentenceByMergeSort_Cases_ReconstructsOriginalOrder(SentenceCase testCase) =>
        Assert.Equal(testCase.Expected, SortingTheSentenceSolution.SortSentenceByMergeSort(testCase.Shuffled));

    [Theory]
    [MemberData(nameof(Cases))]
    public void SortSentenceByPositionScan_Cases_ReconstructsOriginalOrder(SentenceCase testCase) =>
        Assert.Equal(testCase.Expected, SortingTheSentenceSolution.SortSentenceByPositionScan(testCase.Shuffled));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one case,
    // naming which sentence goes in and which one is expected back. Passing the two
    // strings separately would let a caller transpose them silently.
    public readonly record struct SentenceCase(string Shuffled, string Expected);
}
