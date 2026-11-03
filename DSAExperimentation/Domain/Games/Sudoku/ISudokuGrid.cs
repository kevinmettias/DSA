namespace DSAExperimentation.Domain.Games.Sudoku;

// A Sudoku grid is fixed by its box size n: the board is n x n boxes of n x n cells, n^2 a side, and
// holds n^2 symbols, so the box size is the one number and every other follows (Sudoku). Classic
// Sudoku is n = 3; the 4x4 children's puzzle is n = 2 and the 16x16 one n = 4. A board also writes an
// empty cell with some mark, which is part of how the grid is written down.
/// <summary>A Sudoku grid: its box size, and how an empty cell is written.</summary>
public interface ISudokuGrid
{
    /// <summary>The side of a box, n: the board is n x n boxes of n x n cells.</summary>
    static abstract int BoxSize { get; }

    /// <summary>The mark an empty cell is written with.</summary>
    static abstract char EmptyCell { get; }
}
