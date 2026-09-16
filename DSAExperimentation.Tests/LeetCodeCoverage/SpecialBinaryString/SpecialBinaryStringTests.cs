using DSAExperimentation.LeetCode.SpecialBinaryString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SpecialBinaryString;

// Harness only. The split-and-recurse walk and both sort strategies are
// SpecialBinaryStringSolution's - this file just pins them to LeetCode's published
// examples.
public sealed partial class SpecialBinaryStringTests
{
    public static TheoryData<SpecialPieceExample> Examples =>
        new()
        {
            // LeetCode's own worked example: top-level pieces of the interior are "10"
            // and "1100"; swapping them to "1100" + "10" (then re-wrapping in the outer
            // "1"..."0") is LeetCode's published answer for this input.
            new SpecialPieceExample("11011000", "11100100"),
            // A single special pair has nothing to swap.
            new SpecialPieceExample("10", "10"),
            // Two sibling pieces out of order: "10" and "1100", with "1100" >
            // "10" lexicographically, so the maximal arrangement swaps them.
            new SpecialPieceExample("101100", "110010"),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MakeLargestSpecialByArraySort_LeetCodeExamples_SwapsPiecesToMaximize(
        SpecialPieceExample example) =>
        Assert.Equal(example.Expected, SpecialBinaryStringSolution.MakeLargestSpecialByArraySort(example.S));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MakeLargestSpecialByMergeSort_LeetCodeExamples_SwapsPiecesToMaximize(
        SpecialPieceExample example) =>
        Assert.Equal(example.Expected, SpecialBinaryStringSolution.MakeLargestSpecialByMergeSort(example.S));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one LeetCode example,
    // naming which special string goes in and which one is expected back. Passing the
    // two strings separately would let a caller transpose them silently.
    public readonly record struct SpecialPieceExample(string S, string Expected);
}
