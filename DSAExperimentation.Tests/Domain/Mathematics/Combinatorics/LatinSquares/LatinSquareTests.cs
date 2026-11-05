using DSAExperimentation.Domain.Mathematics.Combinatorics.LatinSquares;

namespace DSAExperimentation.Tests.Domain.Mathematics.Combinatorics.LatinSquares;

// The squares are written in letters, since a Latin square may use any n symbols. Each square that
// fails breaks one clause of the rule and keeps the others: its rows, its columns, or its symbols.
public sealed partial class LatinSquareTests
{
    // Row r of the cyclic square is the first row shifted r places: the addition table of the
    // integers mod 5, a Latin square of every order. Five is prime, so no box splits this square.
    public static TheoryData<string[]> LatinSquaresOfOrderFive => new()
    {
        { ["abcde", "bcdea", "cdeab", "deabc", "eabcd"] },
        { ["abcde", "cdeab", "eabcd", "bcdea", "deabc"] },
    };

    // Every column holds a, b and c once, but two rows repeat a symbol.
    public static TheoryData<string[]> RowRepeatsOfOrderThree =>
        new() { { ["bca", "aab", "cbc"] } };

    // Every row holds a, b and c once, but two columns repeat a symbol.
    public static TheoryData<string[]> ColumnRepeatsOfOrderThree =>
        new() { { ["bac", "cab", "abc"] } };

    // A square that is not three rows of three cells.
    public static TheoryData<string[]> MisshapenOfOrderThree => new()
    {
        { ["abc", "bca"] },
        { ["abc", "bc", "cab"] },
        { ["abcd", "bcda", "cdab"] },
    };

    // Against a square whose only symbol is an 'a' at the top left: the row and the column hold it,
    // the diagonal does not, and no line holds a 'b'.
    public static TheoryData<int, int, char> PlacementsATopLeftABlocks =>
        new() { { 0, 2, 'a' }, { 2, 0, 'a' } };

    public static TheoryData<int, int, char> PlacementsATopLeftAAllows =>
        new() { { 1, 1, 'a' }, { 0, 2, 'b' } };

    [Theory]
    [MemberData(nameof(LatinSquaresOfOrderFive))]
    public void IsLatinSquare_EachSymbolOncePerRowAndColumn_ReturnsTrue(string[] rows) =>
        Assert.True(LatinSquare<OrderFive>.IsLatinSquare(Square(rows)));

    [Theory]
    [MemberData(nameof(RowRepeatsOfOrderThree))]
    public void IsLatinSquare_RowRepeatsASymbol_ReturnsFalse(string[] rows) =>
        Assert.False(LatinSquare<OrderThree>.IsLatinSquare(Square(rows)));

    [Theory]
    [MemberData(nameof(ColumnRepeatsOfOrderThree))]
    public void IsLatinSquare_ColumnRepeatsASymbol_ReturnsFalse(string[] rows) =>
        Assert.False(LatinSquare<OrderThree>.IsLatinSquare(Square(rows)));

    // Rows a b and b c and their columns repeat nothing, but the square holds three symbols, not
    // two.
    [Fact]
    public void IsLatinSquare_MoreSymbolsThanItsOrder_ReturnsFalse() =>
        Assert.False(LatinSquare<OrderTwo>.IsLatinSquare(Square(["ab", "bc"])));

    [Theory]
    [MemberData(nameof(MisshapenOfOrderThree))]
    public void IsLatinSquare_NotOrderBySquare_ReturnsFalse(string[] rows) =>
        Assert.False(LatinSquare<OrderThree>.IsLatinSquare(Square(rows)));

    [Theory]
    [MemberData(nameof(PlacementsATopLeftABlocks))]
    public void IsInRowOrColumn_SymbolALineHolds_ReturnsTrue(int row, int col, char symbol) =>
        Assert.True(LatinSquare<OrderThree>.IsInRowOrColumn(TopLeftA(), row, col, symbol));

    [Theory]
    [MemberData(nameof(PlacementsATopLeftAAllows))]
    public void IsInRowOrColumn_SymbolNoLineHolds_ReturnsFalse(int row, int col, char symbol) =>
        Assert.False(LatinSquare<OrderThree>.IsInRowOrColumn(TopLeftA(), row, col, symbol));

    private static char[][] Square(string[] rows) => [.. rows.Select(row => row.ToCharArray())];

    private static char[][] TopLeftA() => Square(["a..", "...", "..."]);

    private readonly struct OrderTwo : ILatinSquare
    {
        public static int Order => 2;
    }

    private readonly struct OrderThree : ILatinSquare
    {
        public static int Order => 3;
    }

    private readonly struct OrderFive : ILatinSquare
    {
        public static int Order => 5;
    }
}
