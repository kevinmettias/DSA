using static DSAExperimentation.LeetCode.DesignMovieRentalSystem.DesignMovieRentalSystemSolution;

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
        RunScript(new MovieRentingSystemBySortOnQuery(shopCount, entries), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void MovieRentingSystemByBinarySearchTree_LeetCodeExamples_AnswersEveryCallInTheScript(
        int shopCount, int[][] entries, MovieRentalOp[] operations, int[][][] expected) =>
        RunScript(new MovieRentingSystemByBinarySearchTree(shopCount, entries), operations, expected);

    private static void RunScript(IMovieRentingSystem system, MovieRentalOp[] operations, int[][][] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(system));
        }
    }
}

// One call in a movie rental script: which method to invoke and with what
// arguments. Pure dispatch, built through the named factories below so a script
// reads like the LeetCode call sequence it replays.
public readonly record struct MovieRentalOp
{
    private readonly Kind _kind;
    private readonly int _shop;
    private readonly int _movie;

    private MovieRentalOp(Kind kind, int shop, int movie)
    {
        _kind = kind;
        _shop = shop;
        _movie = movie;
    }

    public static MovieRentalOp Search(int movie) => new(Kind.Search, 0, movie);

    public static MovieRentalOp Rent(int shop, int movie) => new(Kind.Rent, shop, movie);

    public static MovieRentalOp Drop(int shop, int movie) => new(Kind.Drop, shop, movie);

    public static MovieRentalOp Report() => new(Kind.Report, 0, 0);

    // No rows for the two void calls, one row of shop ids for search(), one row per
    // rented pair for report() - so a script runner can assert one expected value
    // per operation uniformly. Internal, not public: IMovieRentingSystem is
    // internal to DesignMovieRentalSystemSolution, and only this assembly's
    // RunScript ever calls Apply.
    internal int[][] Apply(IMovieRentingSystem system)
    {
        switch (_kind)
        {
            case Kind.Search:
                return [[.. system.Search(_movie)]];
            case Kind.Rent:
                system.Rent(_shop, _movie);
                return [];
            case Kind.Drop:
                system.Drop(_shop, _movie);
                return [];
            default:
                return [.. system.Report().Select(pair => pair.ToArray())];
        }
    }

    private enum Kind
    {
        Search,
        Rent,
        Drop,
        Report,
    }
}
