using DSAExperimentation.LeetCode.Solutions.SpecialBinaryString;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SpecialBinaryString;

// Harness only. The split-and-recurse walk and both sort strategies are
// SpecialBinaryStringSolution's - this file pins them to two sibling pieces that
// arrive out of order. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class SpecialBinaryStringSolutionTests
{
    public static TheoryData<SpecialPieceCase> Cases =>
        new()
        {
            // Two sibling pieces out of order: "10" and "1100", with "1100" >
            // "10" lexicographically, so the maximal arrangement swaps them.
            new SpecialPieceCase("101100", "110010"),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MakeLargestSpecialByArraySort_Cases_SwapsPiecesToMaximize(
        SpecialPieceCase testCase) =>
        Assert.Equal(testCase.Expected, SpecialBinaryStringSolution.MakeLargestSpecialByArraySort(testCase.S));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MakeLargestSpecialByMergeSort_Cases_SwapsPiecesToMaximize(
        SpecialPieceCase testCase) =>
        Assert.Equal(testCase.Expected, SpecialBinaryStringSolution.MakeLargestSpecialByMergeSort(testCase.S));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one case,
    // naming which special string goes in and which one is expected back. Passing the
    // two strings separately would let a caller transpose them silently.
    public readonly record struct SpecialPieceCase(string S, string Expected);
}
