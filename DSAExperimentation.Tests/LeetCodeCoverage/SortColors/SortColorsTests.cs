using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SortColors;

public sealed partial class SortColorsTests
{
    [Theory]
    [InlineData(new[] { 2, 0, 2, 1, 1, 0 }, new[] { 0, 0, 1, 1, 2, 2 })]
    [InlineData(new[] { 2, 0, 1 }, new[] { 0, 1, 2 })]
    public void SortColors_LeetCodeExamples_SortsInPlace(int[] nums, int[] expected) { Sort(nums); Assert.Equal(expected, nums); }
    private static void Sort(int[] nums) { var seq = new ArrayIndexedSequence<int>(nums); var low = 0; var mid = 0; var high = seq.Length - 1; while (mid <= high) { if (seq.Get(mid) == 0) Swap(seq, low++, mid++); else if (seq.Get(mid) == 2) Swap(seq, mid, high--); else mid++; } }
    private static void Swap(ArrayIndexedSequence<int> seq, int first, int second) { var temp = seq.Get(first); seq.Set(first, seq.Get(second)); seq.Set(second, temp); }
}
