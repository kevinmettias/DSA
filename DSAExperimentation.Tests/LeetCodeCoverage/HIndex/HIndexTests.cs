using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HIndex;

// LeetCode 274. H-Index: sort citations ascending with this repo's own MergeSort over
// ArrayIndexedSequence, then scan from the most-cited paper down for the largest h
// with at least h papers cited h times or more - O(n log n) instead of the textbook
// O(n^2) "recount for every candidate h" scan.
public sealed partial class HIndexTests
{
    [Theory]
    [InlineData(new[] { 3, 0, 6, 1, 5 }, 3)]
    [InlineData(new[] { 1, 3, 1 }, 1)]
    [InlineData(new[] { 0, 0 }, 0)]
    public void HIndex_LeetCodeExamples_ReturnsLargestQualifyingH(int[] citations, int expected)
        => Assert.Equal(expected, HIndex(citations));

    private static int HIndex(int[] citations)
    {
        var sorted = citations.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var h = 0;
        for (var i = sorted.Length - 1; i >= 0; i--)
        {
            var papersAtLeastThisCited = sorted.Length - i;
            if (sorted[i] < papersAtLeastThisCited)
            {
                break;
            }

            h = papersAtLeastThisCited;
        }

        return h;
    }
}
