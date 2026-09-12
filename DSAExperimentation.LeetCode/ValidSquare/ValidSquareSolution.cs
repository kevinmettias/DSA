using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.ValidSquare;

// LeetCode 593. Valid Square: four points form a square exactly when their 6
// pairwise squared distances resolve into four equal "side" distances and two
// equal "diagonal" distances worth exactly double a side. Squared distances (not
// the square root) avoid floating-point comparisons entirely.
//
// The two strategies differ only in how they read that shape off the 6 distances -
// sort them and check fixed indices, or scan for the min/max directly.
internal static class ValidSquareSolution
{
    private const int ExpectedSideCount = 4;
    private const int ExpectedDiagonalCount = 2;
    private const int DiagonalToSideSquaredRatio = 2;
    private const int ThirdSideIndex = 2;
    private const int FourthSideIndex = 3;
    private const int FirstDiagonalIndex = 4;
    private const int SecondDiagonalIndex = 5;
    private const int PairwiseDistanceCount = 6;

    // This repo's own MergeSort over ArrayIndexedSequence (the same "sort, then
    // scan the sorted shape" idiom HIndex uses): sort the 6 squared distances, then
    // check the fixed-index shape a square's sorted distances must have.
    public static bool IsValidSquareByMergeSort(int[] p1, int[] p2, int[] p3, int[] p4) =>
        IsValidSquareByMergeSort([p1, p2, p3, p4]);

    public static bool IsValidSquareByMergeSort(int[][] points)
    {
        var distances = SquaredDistances(points);
        MergeSort.Sort<long, ArrayIndexedSequence<long>>(new ArrayIndexedSequence<long>(distances));

        var side = distances[0];
        return side > 0
            && distances[1] == side && distances[ThirdSideIndex] == side && distances[FourthSideIndex] == side
            && distances[FirstDiagonalIndex] == distances[SecondDiagonalIndex] && distances[FirstDiagonalIndex] == DiagonalToSideSquaredRatio * side;
    }

    // The textbook answer: a manual two-pass min/max scan over the 6 squared
    // distances instead of sorting them. Deliberately written without this repo's
    // primitives - it is the arm the composed solution above has to justify itself
    // against.
    public static bool IsValidSquareByMinMaxScan(int[] p1, int[] p2, int[] p3, int[] p4) =>
        IsValidSquareByMinMaxScan([p1, p2, p3, p4]);

    public static bool IsValidSquareByMinMaxScan(int[][] points)
    {
        var distances = SquaredDistances(points);
        var (min, max) = FindMinAndMax(distances);

        if (min <= 0)
        {
            return false;
        }

        return HasValidSideAndDiagonalCounts(distances, min, max);
    }

    private static (long Min, long Max) FindMinAndMax(long[] distances)
    {
        var min = long.MaxValue;
        var max = long.MinValue;

        foreach (var distance in distances)
        {
            min = Math.Min(min, distance);
            max = Math.Max(max, distance);
        }

        return (min, max);
    }

    private static bool HasValidSideAndDiagonalCounts(long[] distances, long min, long max)
    {
        var sideCount = 0;
        var diagonalCount = 0;

        foreach (var distance in distances)
        {
            if (distance == min)
            {
                sideCount++;
            }
            else if (distance == max)
            {
                diagonalCount++;
            }
            else
            {
                return false;
            }
        }

        return sideCount == ExpectedSideCount && diagonalCount == ExpectedDiagonalCount && max == DiagonalToSideSquaredRatio * min;
    }

    private static long[] SquaredDistances(int[][] points)
    {
        var distances = new long[PairwiseDistanceCount];
        var next = 0;

        for (var i = 0; i < points.Length; i++)
        {
            for (var j = i + 1; j < points.Length; j++)
            {
                var dx = points[i][0] - points[j][0];
                var dy = points[i][1] - points[j][1];
                distances[next++] = ((long)dx * dx) + ((long)dy * dy);
            }
        }

        return distances;
    }
}
