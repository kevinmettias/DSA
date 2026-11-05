using DSAExperimentation.Domain.Games.Sudoku.Regions;

namespace DSAExperimentation.Domain.Games.Sudoku;

// A Sudoku grid is a Latin square of order n with its cells split into n regions of n cells
// (IRegionPartition): boxes for classic Sudoku and its 6x6 and 16x16 cousins (IBoxPartition), any
// drawn shapes for jigsaw Sudoku (IRegionMap). A board also writes an empty cell with some mark,
// which is part of how the grid is written down rather than of the square.
/// <summary>A Sudoku grid: its order and regions, and how an empty cell is written.</summary>
public interface ISudokuGrid : IRegionPartition
{
    /// <summary>The mark an empty cell is written with.</summary>
    static abstract char EmptyCell { get; }
}
