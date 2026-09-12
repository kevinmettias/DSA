using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.AssignCookies;

// LeetCode 455. Assign Cookies: maximize how many children receive a cookie at least
// as large as their greed factor.
//
// FindContentChildrenByBruteForceScan is the textbook O(n*m) baseline: for every
// child, linearly rescan every remaining cookie for the smallest one that still
// satisfies it. FindContentChildrenBySortThenTwoPointer sorts both greed factors and
// cookie sizes ascending with this repo's own MergeSort.Sort<Element,TSequence> over
// an ArrayIndexedSequence (the same shape HIndexTests already exercises for a single
// array, applied here twice), then makes a single O(n+m) greedy two-pointer pass -
// giving a child the smallest cookie that still satisfies them is never worse than
// giving a larger one, so sorting once is enough.
internal static class AssignCookiesSolution
{
    // Deliberately written without this repo's primitives - the baseline the sorted
    // two-pointer scan below has to justify itself against.
    public static int FindContentChildrenByBruteForceScan(int[] greed, int[] sizes)
    {
        var remainingSizes = (int[])sizes.Clone();
        var used = new bool[remainingSizes.Length];
        var content = 0;

        foreach (var childGreed in greed)
        {
            if (TryAssignSmallestSufficientCookie(remainingSizes, used, childGreed))
            {
                content++;
            }
        }

        return content;
    }

    private static bool TryAssignSmallestSufficientCookie(int[] sizes, bool[] used, int greed)
    {
        var bestIndex = FindSmallestSufficientCookieIndex(sizes, used, greed);

        if (bestIndex < 0)
        {
            return false;
        }

        used[bestIndex] = true;
        return true;
    }

    private static int FindSmallestSufficientCookieIndex(int[] sizes, bool[] used, int greed)
    {
        var bestIndex = -1;

        for (var j = 0; j < sizes.Length; j++)
        {
            if (!used[j] && sizes[j] >= greed && (bestIndex < 0 || sizes[j] < sizes[bestIndex]))
            {
                bestIndex = j;
            }
        }

        return bestIndex;
    }

    public static int FindContentChildrenBySortThenTwoPointer(int[] greed, int[] sizes)
    {
        var sortedGreed = (int[])greed.Clone();
        var sortedSizes = (int[])sizes.Clone();

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
