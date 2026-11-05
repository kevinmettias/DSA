namespace DSAExperimentation.Domain.Mathematics.Combinatorics.LatinSquares;

// A Latin square of order n is an n x n grid filled with n symbols, each appearing exactly once in
// every row and once in every column. The order is the one number: it is the side, and the count of
// symbols. A Sudoku grid is a Latin square whose cells are also split into n regions of n cells,
// each holding every symbol once, which mathematicians call a gerechte design (Behrens, 1956). The
// order is a static abstract member of a struct, so LatinSquare's loops run to a constant in each
// caller.
/// <summary>A Latin square's order: its side, and how many symbols it holds.</summary>
public interface ILatinSquare
{
    /// <summary>The order n: the square has n rows and n columns and holds n symbols.</summary>
    static abstract int Order { get; }
}
