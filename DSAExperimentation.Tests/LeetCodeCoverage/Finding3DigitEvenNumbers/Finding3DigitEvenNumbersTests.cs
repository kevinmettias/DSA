using DSAExperimentation.LeetCode.Finding3DigitEvenNumbers;

namespace DSAExperimentation.Tests.LeetCodeCoverage.Finding3DigitEvenNumbers;

// Harness only. Both strategies live in Finding3DigitEvenNumbersSolution - the
// List.Contains dedupe baseline that used to exist only as an unasserted benchmark
// arm, and the Set<int> + MergeSort composition - and this file pins both to
// LeetCode's published examples plus the degenerate digit sets.
public sealed class Finding3DigitEvenNumbersTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            // LC example 1.
            { [2, 1, 3, 0], [102, 120, 130, 132, 210, 230, 302, 310, 312, 320] },

            // LC example 2: the same value at different positions may both be
            // used, but equal resulting numbers collapse to one.
            { [2, 2, 8, 8, 2], [222, 228, 282, 288, 822, 828, 882] },

            // LC example 3: no even digit at all, so no number can end evenly.
            { [3, 7, 5], [] },

            // Every digit zero: a leading zero is never allowed, so nothing forms.
            { [0, 0, 0], [] },

            // Exactly one even digit, which must therefore take the ones place.
            { [1, 2, 3], [132, 312] },

            // All three positions carry the same even digit.
            { [6, 6, 6], [666] },

            // Fewer than three digits: no triple of distinct positions exists.
            { [2, 4], [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindEvenNumbersByListScanDedupe_LeetCodeExamples_ReturnsSortedDistinctEvenNumbers(
        int[] digits, int[] expected) =>
        Assert.Equal(expected, Finding3DigitEvenNumbersSolution.FindEvenNumbersByListScanDedupe(digits));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindEvenNumbersBySetDedupe_LeetCodeExamples_ReturnsSortedDistinctEvenNumbers(
        int[] digits, int[] expected) =>
        Assert.Equal(expected, Finding3DigitEvenNumbersSolution.FindEvenNumbersBySetDedupe(digits));
}
