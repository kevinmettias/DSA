using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.LeetCode.SpiralMatrix;

// Shape under test: an INTERFACE return type (IList<int>) rather than a concrete
// one, and a non-square input. The interface matters because TOutput is inferred
// from the For<> call, not from the strategy - so two strategies returning
// different concrete IList implementations still register side by side.
internal sealed class SpiralMatrixRegistration : ILeetCodeProblemRegistration
{
    public LeetCodeProblem Describe()
    {
        var square20 = BuildRectangle(rows: 20, columns: 20);
        var square100 = BuildRectangle(rows: 100, columns: 100);
        var rectangle300x200 = BuildRectangle(rows: 300, columns: 200);

        return LeetCodeProblem.For<int[][], IList<int>>("spiral-matrix")
            .Strategy("VisitedGridWalk", SpiralMatrixSolution.SpiralOrderByVisitedGridWalk)
            .Strategy("BoundaryPointerShrink", SpiralMatrixSolution.SpiralOrderByBoundaryPointerShrink)
            .MatchingAnswersWith(LeetCodeAnswers.SequenceEqual)
            .Case("example-1", [[1, 2, 3], [4, 5, 6], [7, 8, 9]], [1, 2, 3, 6, 9, 8, 7, 4, 5])
            .Case(
                "example-2",
                [[1, 2, 3, 4], [5, 6, 7, 8], [9, 10, 11, 12]],
                [1, 2, 3, 4, 8, 12, 11, 10, 9, 5, 6, 7])
            .Case("single-cell", [[1]], [1])
            .Case("single-row", [[1, 2, 3]], [1, 2, 3])
            .Case("single-column", [[1], [2], [3]], [1, 2, 3])

            // The two square sizes the retired per-problem benchmark swept with
            // [Params], kept as separate workloads rather than averaged into one:
            // the visited-grid arm's extra allocation is what separates the two
            // strategies, and whether it shows up at all is size-dependent.
            .Workload("square-20", square20)
            .Workload("square-100", square100)
            .Workload("rectangle-300x200", rectangle300x200)
            .Build();
    }

    private static int[][] BuildRectangle(int rows, int columns)
        => [.. Enumerable.Range(0, rows).Select(row => Enumerable.Range(row * columns, columns).ToArray())];
}
