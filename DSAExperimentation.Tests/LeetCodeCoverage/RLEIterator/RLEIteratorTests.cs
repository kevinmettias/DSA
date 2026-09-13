using DSAExperimentation.LeetCode.RLEIterator;
using static DSAExperimentation.LeetCode.RLEIterator.RLEIteratorSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RLEIterator;

// Harness only. Both strategies are RLEIteratorSolution's - this file replays
// LeetCode's published next(n) call sequences against each IRleIterator
// implementation, one theory per strategy, so a failure names the strategy that
// broke even though the "input" here is a sequence of calls rather than a single
// argument tuple.
public sealed class RLEIteratorTests
{
    public static TheoryData<int[], int[], int[]> Examples =>
        new()
        {
            // LeetCode's own example: encoding [3,8,0,9,2,5] decodes to
            // 8,8,8,5,5; next(2), next(1), next(1), next(2) exhausts it and then
            // asks for more than remains.
            { [3, 8, 0, 9, 2, 5], [2, 1, 1, 2], [8, 8, 5, -1] },
            // One element short of what the encoding holds.
            { [1, 4], [5], [-1] },
            // Single-element draws across the whole decoded sequence, including
            // the zero-length (0, 9) run, and one draw past the end.
            { [3, 8, 0, 9, 2, 5], [1, 1, 1, 1, 1, 1], [8, 8, 8, 5, 5, -1] },
            // Exact exhaustion in one call: no -1 anywhere.
            { [2, 7], [2], [7] },
            // Exact exhaustion one element at a time, then past the end twice -
            // a spent iterator keeps reporting -1.
            { [2, 7], [1, 1, 1, 1], [7, 7, -1, -1] },
            // A draw that spans a run boundary returns the last element consumed.
            { [2, 3, 2, 6], [3], [6] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByDecompressedArray_LeetCodeExamples_ReturnsExpectedSequence(
        int[] encoding, int[] queries, int[] expected) =>
        AssertReplay(CreateByDecompressedArray(encoding), queries, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByRunLengthQueue_LeetCodeExamples_ReturnsExpectedSequence(
        int[] encoding, int[] queries, int[] expected) =>
        AssertReplay(CreateByRunLengthQueue(encoding), queries, expected);

    private static void AssertReplay(IRleIterator iterator, int[] queries, int[] expected)
    {
        for (var i = 0; i < queries.Length; i++)
        {
            Assert.Equal(expected[i], iterator.Next(queries[i]));
        }
    }
}
