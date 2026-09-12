using DSAExperimentation.LeetCode.SpecialBinaryString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SpecialBinaryString;

// Harness only. The split-and-recurse walk and both sort strategies are
// SpecialBinaryStringSolution's - this file just pins them to LeetCode's published
// examples.
public sealed class SpecialBinaryStringTests
{
    public static TheoryData<string, string> Examples =>
        new()
        {
            // LeetCode's own worked example: top-level pieces of the interior are "10"
            // and "1100"; swapping them to "1100" + "10" (then re-wrapping in the outer
            // "1"..."0") is LeetCode's published answer for this input.
            { "11011000", "11100100" },
            // A single special pair has nothing to swap.
            { "10", "10" },
            // Two sibling pieces out of order: "10" and "1100", with "1100" >
            // "10" lexicographically, so the maximal arrangement swaps them.
            { "101100", "110010" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MakeLargestSpecialByArraySort_LeetCodeExamples_SwapsPiecesToMaximize(
        string s, string expected) =>
        Assert.Equal(expected, SpecialBinaryStringSolution.MakeLargestSpecialByArraySort(s));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MakeLargestSpecialByMergeSort_LeetCodeExamples_SwapsPiecesToMaximize(
        string s, string expected) =>
        Assert.Equal(expected, SpecialBinaryStringSolution.MakeLargestSpecialByMergeSort(s));
}
