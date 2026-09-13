using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.LeetCode.KClosestPointsToOrigin;

// Shape under test: the answer-equality trap. LeetCode says "you may return the
// answer in any order", so the ROWS are a set - but a row is a point, and [3,3]
// is not [3,-3]. Sorting inside the rows as well (the obvious reading of "compare
// nested collections loosely") would accept a wrong answer. This is precisely why
// LeetCodeAnswers has no default comparison and why RowSetEqual and
// SequenceOfSequencesEqual are separate, named choices.
internal sealed class KClosestPointsToOriginRegistration : ILeetCodeProblemRegistration
{
    public LeetCodeProblem Describe()
        => LeetCodeProblem.For<(int[][] Points, int K), int[][]>("k-closest-points-to-origin")
            .Strategy("FullSort", input => KClosestPointsToOriginSolution.KClosestByFullSort(input.Points, input.K))
            .Strategy(
                "SizeKMaxHeap",
                input => KClosestPointsToOriginSolution.KClosestBySizeKMaxHeap(input.Points, input.K))
            .MatchingAnswersWith(LeetCodeAnswers.RowSetEqual<int>)
            .Case("example-1", ([[1, 3], [-2, 2]], 1), [[-2, 2]])
            .Case("example-2", ([[3, 3], [5, -1], [-2, 4]], 2), [[3, 3], [-2, 4]])
            .Case("k-covers-every-point", ([[1, 1], [2, 2]], 2), [[1, 1], [2, 2]])
            .Workload("scattered-20000", BuildScatteredPoints(count: 20_000, k: 100))
            .Build();

    private static (int[][] Points, int K) BuildScatteredPoints(int count, int k)
    {
        var random = new Random(Seed: 20_000);
        var points = new int[count][];

        for (var index = 0; index < count; index++)
        {
            points[index] = [random.Next(-10_000, 10_000), random.Next(-10_000, 10_000)];
        }

        return (points, k);
    }
}
