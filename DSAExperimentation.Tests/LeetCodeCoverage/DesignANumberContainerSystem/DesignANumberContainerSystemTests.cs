using static DSAExperimentation.LeetCode.DesignANumberContainerSystem.DesignANumberContainerSystemSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignANumberContainerSystem;

// Harness only. Both strategies are DesignANumberContainerSystemSolution's - this
// file replays LeetCode's published call sequence against each
// INumberContainerStrategy implementation via a small operation script, so a
// failure still names the strategy that broke even though the "input" here is a
// sequence of mutating calls rather than a single argument tuple.
// NumberContainerOp.Apply is pure dispatch (which method to call with which
// arguments) - no index/ordering logic of its own.
public sealed class DesignANumberContainerSystemTests
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
        RunScript(new NumberContainersByLinearScan(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberContainersByLazyDeletionHeap_LeetCodeExamples_FindsSmallestCurrentlyAssignedIndex(
        NumberContainerOp[] operations, int?[] expected) =>
        RunScript(new NumberContainersByLazyDeletionHeap(), operations, expected);

    private static void RunScript(INumberContainerStrategy strategy, NumberContainerOp[] operations, int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(strategy));
        }
    }
}

// One call in a NumberContainers script: which method to invoke and with what
// arguments. Pure dispatch, built via the named factories below so a script (like
// Examples above) reads like the LeetCode call sequence it replays.
public readonly record struct NumberContainerOp
{
    private readonly bool _isFind;
    private readonly int _index;
    private readonly int _number;

    private NumberContainerOp(bool isFind, int index, int number)
    {
        _isFind = isFind;
        _index = index;
        _number = number;
    }

    public static NumberContainerOp Change(int index, int number) => new(isFind: false, index, number);

    public static NumberContainerOp Find(int number) => new(isFind: true, index: 0, number);

    // null for the void Change call, the reported index for Find - so a script
    // runner can assert against one expected value per operation uniformly.
    // Internal, not public: INumberContainerStrategy is internal to
    // DesignANumberContainerSystemSolution, and only this same assembly's
    // RunScript ever calls Apply.
    internal int? Apply(INumberContainerStrategy strategy)
    {
        if (_isFind)
        {
            return strategy.Find(_number);
        }

        strategy.Change(_index, _number);
        return null;
    }
}
