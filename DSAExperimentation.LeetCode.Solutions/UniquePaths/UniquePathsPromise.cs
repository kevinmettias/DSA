using System.Numerics;

namespace DSAExperimentation.LeetCode.Solutions.UniquePaths;

// LC 62's promise about its input: the answer is at most 2 * 10^9, so it fits the int both
// strategies answer in. LeetCode's judge never poses an input that breaks it. A larger count wraps
// around. Generated cases do not always keep the promise, so a harness that runs them asks this
// first.
internal static class UniquePathsPromise
{
    private const long MostPaths = 2_000_000_000;

    public static bool IsKeptBy(int rowCount, int columnCount) => PathCount(rowCount, columnCount) <= MostPaths;

    // The binomial (m + n - 2) choose (m - 1), exact at any size.
    private static BigInteger PathCount(int rowCount, int columnCount)
    {
        BigInteger count = 1;

        for (var step = 1; step < rowCount; step++)
        {
            count = count * (columnCount - 1 + step) / step;
        }

        return count;
    }
}
