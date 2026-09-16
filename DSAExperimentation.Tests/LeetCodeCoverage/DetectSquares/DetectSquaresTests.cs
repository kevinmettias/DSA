using DSAExperimentation.LeetCode.DetectSquares;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DetectSquares;

// Harness only. Both strategies are DetectSquaresSolution's - this file replays
// LeetCode's published call sequence against each IDetectSquares instance, so a
// failure still names the strategy that broke even though the "input" here is a
// sequence of Add/Count calls rather than a single argument tuple, the same shape
// AllOneDataStructureTests uses for its own instance-API problem. The point-list
// baseline (previously untested scaffolding inlined in the benchmark) gets the same
// coverage as the grouped-HashMap strategy here for the first time.
public sealed class DetectSquaresTests
{
    public static TheoryData<DetectSquaresOp[], int?[]> Examples =>
        new()
        {
            {
                // LeetCode's published example, including the repeated Add that turns
                // the same query from 1 into 2.
                [
                    DetectSquaresOp.Add(3, 10), DetectSquaresOp.Add(11, 2), DetectSquaresOp.Add(3, 2),
                    DetectSquaresOp.Count(11, 10), DetectSquaresOp.Count(14, 8),
                    DetectSquaresOp.Add(11, 2), DetectSquaresOp.Count(11, 10),
                ],
                [null, null, null, 1, 0, null, 2]
            },
            {
                // No stored point shares the query's x-coordinate.
                [DetectSquaresOp.Add(0, 0), DetectSquaresOp.Count(5, 5)],
                [null, 0]
            },
            {
                // Nothing added yet.
                [DetectSquaresOp.Count(0, 0)],
                [0]
            },
            {
                // The square lies to the LEFT of the query, so only the x - side
                // candidate corner exists.
                [
                    DetectSquaresOp.Add(0, 0), DetectSquaresOp.Add(0, 2), DetectSquaresOp.Add(2, 0),
                    DetectSquaresOp.Count(2, 2),
                ],
                [null, null, null, 1]
            },
            {
                // A duplicated corner multiplies the answer: (0,0) was added twice, so
                // the one square shape is counted twice.
                [
                    DetectSquaresOp.Add(0, 0), DetectSquaresOp.Add(0, 0), DetectSquaresOp.Add(0, 2),
                    DetectSquaresOp.Add(2, 2), DetectSquaresOp.Count(2, 0),
                ],
                [null, null, null, null, 2]
            },
            {
                // A duplicate of the query point itself is a zero-area "square" and
                // must not be counted.
                [DetectSquaresOp.Add(1, 1), DetectSquaresOp.Add(1, 1), DetectSquaresOp.Count(1, 1)],
                [null, null, 0]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByHashMapGroupedByX_LeetCodeExamples_CountsAxisAlignedSquares(
        DetectSquaresOp[] operations, int?[] expected) =>
        RunScript(DetectSquaresSolution.CreateByHashMapGroupedByX(), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByPointListScan_LeetCodeExamples_CountsAxisAlignedSquares(
        DetectSquaresOp[] operations, int?[] expected) =>
        RunScript(DetectSquaresSolution.CreateByPointListScan(), operations, expected);

    private static void RunScript(
        DetectSquaresSolution.IDetectSquares detector, DetectSquaresOp[] operations, int?[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(detector));
        }
    }

    // One call in a DetectSquares script: which operation to invoke and at which point.
    // Pure dispatch, built via the named factories below so a script (like Examples above)
    // reads like the LeetCode call sequence it replays. Add returns null (no answer);
    // Count returns the actual answer - the same null-means-"no return value" convention
    // AllOneOp.Apply uses for its own mutator/query split. Nested here rather than left
    // at file scope so the file declares exactly one type.
    public readonly record struct DetectSquaresOp(bool isCount, int pointX, int pointY)
    {
        public static DetectSquaresOp Add(int pointX, int pointY) => new(isCount: false, pointX, pointY);

        public static DetectSquaresOp Count(int pointX, int pointY) => new(isCount: true, pointX, pointY);

        internal int? Apply(DetectSquaresSolution.IDetectSquares detector)
        {
            if (isCount)
            {
                return detector.Count(pointX, pointY);
            }

            detector.Add(pointX, pointY);

            return null;
        }
    }
}
