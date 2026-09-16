using DSAExperimentation.LeetCode.DesignMovieRentalSystem;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DesignMovieRentalSystem;

// One call in a movie rental script: which method to invoke and with what
// arguments. Pure dispatch, built through the named factories below so a script
// reads like the LeetCode call sequence it replays.
public readonly record struct MovieRentalOp(MovieRentalOp.OpKind kind, int shop, int movie)
{
    public static MovieRentalOp Search(int movie) => new(OpKind.Search, 0, movie);

    public static MovieRentalOp Rent(int shop, int movie) => new(OpKind.Rent, shop, movie);

    public static MovieRentalOp Drop(int shop, int movie) => new(OpKind.Drop, shop, movie);

    public static MovieRentalOp Report() => new(OpKind.Report, 0, 0);

    // No rows for the two void calls, one row of shop ids for search(), one row per
    // rented pair for report() - so a script runner can assert one expected value
    // per operation uniformly. Internal, not public: IMovieRentingSystem is
    // internal to DesignMovieRentalSystemSolution, and only this assembly's
    // RunScript ever calls Apply.
    internal int[][] Apply(DesignMovieRentalSystemSolution.IMovieRentingSystem system)
    {
        switch (kind)
        {
            case OpKind.Search:
                return [[.. system.Search(movie)]];
            case OpKind.Rent:
                system.Rent(shop, movie);
                return [];
            case OpKind.Drop:
                system.Drop(shop, movie);
                return [];
            default:
                return [.. system.Report().Select(pair => pair.ToArray())];
        }
    }

    public enum OpKind
    {
        Search,
        Rent,
        Drop,
        Report,
    }
}
