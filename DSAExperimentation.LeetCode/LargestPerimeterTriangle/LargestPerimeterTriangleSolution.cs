using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.LargestPerimeterTriangle;

// LeetCode 976. Largest Perimeter Triangle: pick three of nums' side lengths that
// can form a triangle with non-zero area, maximizing their sum, or report 0 when no
// such triple exists.
//
// The two strategies differ only in how many triples they have to look at: the
// baseline checks the triangle inequality against every unordered triple, while the
// composed strategy sorts first and then only ever inspects consecutive triples off
// the top of the sorted run.
internal static class LargestPerimeterTriangleSolution
{
    // sorted[i - TripleWindowOffset .. i] is the candidate triple at index i.
    private const int TripleWindowOffset = 2;

    // No triple forms a triangle; LC 976 reports 0 rather than -1.
    private const int NoTriangle = 0;

    // The textbook answer: every unordered triple, each checked against all three
    // triangle inequalities on the raw (unsorted) values, keeping the largest valid
    // perimeter. Deliberately written without this repo's primitives - it is the arm
    // LargestPerimeterBySortedScan has to justify itself against.
    public static int LargestPerimeterByBruteForceTriples(int[] nums)
    {
        var best = NoTriangle;

        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                best = BestPerimeterWithFixedPair(nums, i, j, best);
            }
        }

        return best;
    }

    private static int BestPerimeterWithFixedPair(int[] nums, int firstIndex, int secondIndex, int best)
    {
        for (var thirdIndex = secondIndex + 1; thirdIndex < nums.Length; thirdIndex++)
        {
            int firstSide = nums[firstIndex], secondSide = nums[secondIndex], thirdSide = nums[thirdIndex];

            if (CanFormTriangle(firstSide, secondSide, thirdSide))
            {
                best = Math.Max(best, firstSide + secondSide + thirdSide);
            }
        }

        return best;
    }

    // All three triangle inequalities hold, so the sides enclose a non-zero area.
    private static bool CanFormTriangle(int firstSide, int secondSide, int thirdSide) =>
        firstSide + secondSide > thirdSide
        && firstSide + thirdSide > secondSide
        && secondSide + thirdSide > firstSide;

    // This repo's own MergeSort over an ArrayIndexedSequence<int> - the same
    // composition SortAnArray proves out for LC 912 and
    // FindPolygonWithTheLargestPerimeter reuses for LC 2971 - sorts the sides
    // ascending, then one backward scan checks consecutive triples only: for a fixed
    // largest side the two next-largest remaining sides beat any smaller pair, so the
    // first triple (from the top) satisfying a[i-2] + a[i-1] > a[i] is provably the
    // maximum-perimeter answer. Sorting is the only work that scales.
    public static int LargestPerimeterBySortedScan(int[] nums)
    {
        var sorted = (int[])nums.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        for (var i = sorted.Length - 1; i >= TripleWindowOffset; i--)
        {
            if (sorted[i - TripleWindowOffset] + sorted[i - 1] > sorted[i])
            {
                return sorted[i - TripleWindowOffset] + sorted[i - 1] + sorted[i];
            }
        }

        return NoTriangle;
    }
}
