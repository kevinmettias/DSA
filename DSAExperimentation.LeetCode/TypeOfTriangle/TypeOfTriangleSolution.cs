using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.TypeOfTriangle;

// LeetCode 3024. Type of Triangle: classify the triangle three side lengths form -
// "none" if they can't form one at all, else "equilateral"/"isosceles"/"scalene".
//
// Both strategies check the same two facts - the triangle inequality, and how many
// of the three sides coincide - and differ only in whether the sides are compared
// pairwise as given, or sorted first so the inequality collapses to a single
// comparison against the largest side.
internal static class TypeOfTriangleSolution
{
    private const string None = "none";
    private const string Equilateral = "equilateral";
    private const string Isosceles = "isosceles";
    private const string Scalene = "scalene";

    // Checks the triangle inequality across all three pairs directly, and counts
    // coincidences the same way - no sorting, so nothing else in this repo is
    // composed here at all. The arm the sorting-based strategy below is measured
    // against.
    public static string ClassifyByDirectComparison(int[] sides)
    {
        var (first, second, third) = (sides[0], sides[1], sides[2]);

        if (BreaksTriangleInequality(first, second, third))
        {
            return None;
        }

        if (first == second && second == third)
        {
            return Equilateral;
        }

        return HasTwoEqualSides(first, second, third) ? Isosceles : Scalene;
    }

    // Any one of the three triangle inequalities failing leaves no triangle at all.
    private static bool BreaksTriangleInequality(int a, int b, int c)
        => a + b <= c || a + c <= b || b + c <= a;

    // Two of the three sides coincide, which is all "isosceles" needs once
    // "equilateral" has been ruled out.
    private static bool HasTwoEqualSides(int a, int b, int c)
        => a == b || b == c || a == c;

    // This repo's own MergeSort, run over a copy so the caller's array is never
    // mutated: once sorted ascending, the triangle inequality is a single
    // comparison of the two smaller sides against the largest, and coincidence
    // checks only ever need to look at adjacent pairs.
    public static string ClassifyByMergeSort(int[] sides)
    {
        var sorted = new[] { sides[0], sides[1], sides[2] };
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var (smallest, middle, largest) = (sorted[0], sorted[1], sorted[2]);

        if (smallest + middle <= largest)
        {
            return None;
        }

        if (smallest == largest)
        {
            return Equilateral;
        }

        return smallest == middle || middle == largest ? Isosceles : Scalene;
    }
}
