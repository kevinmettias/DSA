using DSAExperimentation.LeetCode.DesignAnOrderedStream;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignAnOrderedStream;

// Harness only: both strategies live in DesignAnOrderedStreamSolution. LeetCode's
// own shape here is a stateful object across a sequence of calls, so Examples
// encodes an insertion script - the stream size, the id keys in arrival order, the
// value inserted with each, and the chunk each Insert is expected to return - the
// same call-script shape DesignBrowserHistoryTests already uses for its own
// instance-API problem.
public sealed class DesignAnOrderedStreamTests
{
    public static TheoryData<int, int[], string[], string[][]> Examples =>
        new()
        {
            // LeetCode's published example: nothing is emitted until the gap before
            // an already-arrived value closes.
            {
                5,
                [3, 1, 2, 5, 4],
                ["ccccc", "aaaaa", "bbbbb", "eeeee", "ddddd"],
                [[], ["aaaaa"], ["bbbbb", "ccccc"], [], ["ddddd", "eeeee"]]
            },

            // Values arrive in order, so every call emits exactly its own value and
            // the cursor never has to walk more than one slot.
            { 3, [1, 2, 3], ["a", "b", "c"], [["a"], ["b"], ["c"]] },

            // The worst case for the cursor: everything arrives in reverse, so the
            // last Insert drains the whole stream in one walk.
            { 4, [4, 3, 2, 1], ["d", "c", "b", "a"], [[], [], [], ["a", "b", "c", "d"]] },

            // A single-slot stream: the one Insert closes the stream outright.
            { 1, [1], ["only"], [["only"]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void OrderedStreamByListBacked_LeetCodeExamples_ReturnsChunksAsGapsClose(
        int n, int[] idKeys, string[] values, string[][] expected) =>
        RunScript(new DesignAnOrderedStreamSolution.OrderedStreamByListBacked(n), idKeys, values, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void OrderedStreamByDynamicArrayBacked_LeetCodeExamples_ReturnsChunksAsGapsClose(
        int n, int[] idKeys, string[] values, string[][] expected) =>
        RunScript(new DesignAnOrderedStreamSolution.OrderedStreamByDynamicArrayBacked(n), idKeys, values, expected);

    private static void RunScript(
        DesignAnOrderedStreamSolution.IOrderedStream stream, int[] idKeys, string[] values, string[][] expected)
    {
        for (var i = 0; i < idKeys.Length; i++)
        {
            var chunk = stream.Insert(idKeys[i], values[i]);

            Assert.Equal(expected[i], chunk);
        }
    }
}
