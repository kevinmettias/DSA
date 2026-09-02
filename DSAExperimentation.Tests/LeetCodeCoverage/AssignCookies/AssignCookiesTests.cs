using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AssignCookies;

// LeetCode 455. Assign Cookies: sort both greed factors and cookie sizes ascending
// with this repo's own MergeSort.Sort<Element,TSequence> over an ArrayIndexedSequence
// (the same shape HIndexTests already exercises for a single array, applied here
// twice), then a single greedy two-pointer pass - giving a child the smallest
// cookie that still satisfies them is never worse than giving a larger one, so
// sorting both once is enough.
public sealed partial class AssignCookiesTests
{
    [Fact]
    public void FindContentChildren_LeetCodeExampleOne_ReturnsOne()
    {
        int[] greed = [1, 2, 3];
        int[] sizes = [1, 1];

        var actual = FindContentChildren(greed, sizes);

        Assert.Equal(1, actual);
    }

    [Fact]
    public void FindContentChildren_LeetCodeExampleTwo_ReturnsTwo()
    {
        int[] greed = [1, 2];
        int[] sizes = [1, 2, 3];

        var actual = FindContentChildren(greed, sizes);

        Assert.Equal(2, actual);
    }

    [Fact]
    public void FindContentChildren_NoCookieLargeEnough_ReturnsZero()
    {
        int[] greed = [5, 9];
        int[] sizes = [1, 2, 3];

        var actual = FindContentChildren(greed, sizes);

        Assert.Equal(0, actual);
    }

    private static int FindContentChildren(int[] greed, int[] sizes)
    {
        var sortedGreed = greed.ToArray();
        var sortedSizes = sizes.ToArray();

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sortedGreed));
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sortedSizes));

        var child = 0;
        var cookie = 0;

        while (child < sortedGreed.Length && cookie < sortedSizes.Length)
        {
            if (sortedSizes[cookie] >= sortedGreed[child])
            {
                child++;
            }

            cookie++;
        }

        return child;
    }
}
