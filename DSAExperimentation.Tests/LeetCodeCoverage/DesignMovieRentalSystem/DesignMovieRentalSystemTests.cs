using DSAExperimentation.LeetCode.DesignMovieRentalSystem;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignMovieRentalSystem;

// Harness only. Both strategies are DesignMovieRentalSystemSolution's - this file
// replays LeetCode's published call sequence, plus the shapes the pre-migration
// test carried (a movie nothing stocks, more unrented shops than the five-result
// cap) and two the sort-on-query arm had never been asked at all before it was
// promoted from a benchmark-only search(): a report() capped at five, and a price
// tie broken by shop id. MovieRentalOp.Apply is pure dispatch - which method to
// call with which arguments - with no ordering logic of its own.
//
// Every answer is normalised to int[][] so one expected value per operation covers
// all four calls: search() answers as a single row of shop ids, report() as one row
// per rented (shop, movie) pair, and the two void calls as no rows at all.
public sealed class DesignMovieRentalSystemTests
{
    private static readonly int[][] SixShopsOneMovie =
    [
        [0, 1, 6], [1, 1, 5], [2, 1, 4], [3, 1, 3], [4, 1, 2], [5, 1, 1],
    ];

    public static TheoryData<int, int[][], MovieRentalOp[], int[][][]> Examples =>
        new()
        {
            {
                3,
                [[0, 1, 5], [0, 2, 6], [0, 3, 7], [1, 1, 4], [1, 2, 7], [2, 1, 5]],
                [
                    MovieRentalOp.Search(1),
                    MovieRentalOp.Rent(0, 1),
                    MovieRentalOp.Rent(1, 2),
                    MovieRentalOp.Report(),
                    MovieRentalOp.Drop(1, 2),
                    MovieRentalOp.Search(2),
                ],
                [
                    [[1, 0, 2]],
                    [],
                    [],
                    [[0, 1], [1, 2]],
                    [],
                    [[0, 1]],
                ]
            },
            {
                1,
                [[0, 1, 5]],
                [MovieRentalOp.Search(99), MovieRentalOp.Report()],
                [[[]], []]
            },
            {
                6,
                SixShopsOneMovie,
                [MovieRentalOp.Search(1)],
                [[[5, 4, 3, 2, 1]]]
            },
            {
                6,
                SixShopsOneMovie,
                [
                    MovieRentalOp.Rent(5, 1),
                    MovieRentalOp.Rent(4, 1),
                    MovieRentalOp.Rent(3, 1),
                    MovieRentalOp.Rent(2, 1),
                    MovieRentalOp.Rent(1, 1),
                    MovieRentalOp.Report(),
                    MovieRentalOp.Search(1),
                ],
                [
                    [],
                    [],
                    [],
                    [],
                    [],
                    [[5, 1], [4, 1], [3, 1], [2, 1], [1, 1]],
                    [[0]],
                ]
            },
            {
                2,
                [[0, 1, 5], [1, 1, 5]],
                [
                    MovieRentalOp.Search(1),
                    MovieRentalOp.Rent(1, 1),
                    MovieRentalOp.Rent(0, 1),
                    MovieRentalOp.Report(),
                ],
                [
                    [[0, 1]],
                    [],
                    [],
                    [[0, 1], [1, 1]],
                ]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MovieRentingSystemBySortOnQuery_LeetCodeExamples_AnswersEveryCallInTheScript(
        int shopCount, int[][] entries, MovieRentalOp[] operations, int[][][] expected) =>
        RunScript(
            new DesignMovieRentalSystemSolution.MovieRentingSystemBySortOnQuery(shopCount, entries),
            operations,
            expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void MovieRentingSystemByBinarySearchTree_LeetCodeExamples_AnswersEveryCallInTheScript(
        int shopCount, int[][] entries, MovieRentalOp[] operations, int[][][] expected) =>
        RunScript(
            new DesignMovieRentalSystemSolution.MovieRentingSystemByBinarySearchTree(shopCount, entries),
            operations,
            expected);

    private static void RunScript(
        DesignMovieRentalSystemSolution.IMovieRentingSystem system,
        MovieRentalOp[] operations,
        int[][][] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(system));
        }
    }
}
