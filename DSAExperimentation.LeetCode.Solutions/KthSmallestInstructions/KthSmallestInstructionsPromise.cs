namespace DSAExperimentation.LeetCode.Solutions.KthSmallestInstructions;

// LC 1643's promise about its input: k is the rank of one of the instructions that reach destination, of
// which there are C(row + column, row), one per arrangement of the row's 'V's among the column's 'H's.
// LeetCode's judge never poses an input that breaks it. Past the last of them the enumerate-and-sort arm
// indexes beyond the list it built and throws, while the memoized greedy answers with the last instruction,
// every 'V' first. Generated cases do not always keep the promise, so a harness that runs them asks this
// first.
internal static class KthSmallestInstructionsPromise
{
    public static bool IsKeptBy(int[] destination, long rank) => rank >= 1 && rank <= PathCountTo(destination);

    // How many instructions reach destination: C(row + column, row), built one factor at a time, so that each
    // partial product is itself a binomial coefficient and every division is exact.
    public static long PathCountTo(int[] destination)
    {
        var (rows, columns) = (destination[0], destination[1]);
        var count = 1L;

        for (var step = 1; step <= rows; step++)
        {
            count = count * (columns + step) / step;
        }

        return count;
    }
}
