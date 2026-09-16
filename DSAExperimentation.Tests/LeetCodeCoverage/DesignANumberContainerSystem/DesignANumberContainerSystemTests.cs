using DSAExperimentation.LeetCode.DesignANumberContainerSystem;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignANumberContainerSystem;

// Harness only. Both strategies are DesignANumberContainerSystemSolution's - this
// file replays LeetCode's published call sequence against each
// INumberContainerStrategy implementation via a small operation script, so a
// failure still names the strategy that broke even though the "input" here is a
// sequence of mutating calls rather than a single argument tuple.
// NumberContainerOp.Apply is pure dispatch (which method to call with which
// arguments) - no index/ordering logic of its own.
public sealed partial class DesignANumberContainerSystemTests
{
    public static TheoryData<NumberContainerOp[], int?[]> Examples =>
        new()
        {
            // LeetCode's published example: find before anything is assigned, then
            // the smallest index for 10, then the same query after index 1 is
            // reassigned to a different number.
            {
                [
                    NumberContainerOp.Find(10),
                    NumberContainerOp.Change(2, 10),
                    NumberContainerOp.Change(1, 10),
                    NumberContainerOp.Change(3, 10),
                    NumberContainerOp.Change(5, 10),
                    NumberContainerOp.Find(10),
                    NumberContainerOp.Change(1, 20),
                    NumberContainerOp.Find(10),
                ],
                [-1, null, null, null, null, 1, null, 2]
            },

            // A number nothing was ever assigned to.
            {
                [
                    NumberContainerOp.Change(0, 5),
                    NumberContainerOp.Find(99),
                ],
                [null, -1]
            },

            // One index reassigned repeatedly, ending back on its first number:
            // only the current assignment counts, and the abandoned one is empty.
            {
                [
                    NumberContainerOp.Change(4, 7),
                    NumberContainerOp.Change(4, 8),
                    NumberContainerOp.Change(4, 7),
                    NumberContainerOp.Find(7),
                    NumberContainerOp.Find(8),
                ],
                [null, null, null, 4, -1]
            },

            // An index reclaimed by a number a previous Find already discarded it
            // from - the lazy-deletion strategy has to answer 1 again rather than
            // stay on the entry it popped.
            {
                [
                    NumberContainerOp.Change(1, 10),
                    NumberContainerOp.Change(2, 10),
                    NumberContainerOp.Find(10),
                    NumberContainerOp.Change(1, 20),
                    NumberContainerOp.Find(10),
                    NumberContainerOp.Change(1, 10),
                    NumberContainerOp.Find(10),
                    NumberContainerOp.Find(20),
                ],
                [null, null, 1, null, 2, null, 1, -1]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberContainersByLinearScan_LeetCodeExamples_FindsSmallestCurrentlyAssignedIndex(
        NumberContainerOp[] operations, int?[] expected) =>
        Assert.Equal(expected, RunScript(
            new DesignANumberContainerSystemSolution.NumberContainersByLinearScan(), operations));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberContainersByLazyDeletionHeap_LeetCodeExamples_FindsSmallestCurrentlyAssignedIndex(
        NumberContainerOp[] operations, int?[] expected) =>
        Assert.Equal(expected, RunScript(
            new DesignANumberContainerSystemSolution.NumberContainersByLazyDeletionHeap(), operations));

    private static int?[] RunScript(
        DesignANumberContainerSystemSolution.INumberContainerStrategy strategy,
        NumberContainerOp[] operations) =>
        [.. operations.Select(operation => operation.Apply(strategy))];
}
