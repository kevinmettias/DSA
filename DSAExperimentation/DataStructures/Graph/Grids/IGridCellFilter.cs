namespace DSAExperimentation.DataStructures.Graph.Grids;

// Which on-board cells a GridNeighbors scan may step into - the one axis that differs between one
// flood fill and the next (land, water, unlabelled land, a height no lower than the cell left).
// Instance members on a struct type parameter, like IVisitGuard (§12.4): the grid, the cell being
// left and any cell the search skips all arrive from outside the signature, so a static-abstract
// witness would have no channel to them. Each filter is its own JIT instantiation and CanEnter a
// direct, inlinable call - no delegate, no interface dispatch.
//
// CanEnter is only ever asked about a cell GridSize.HasCell has already accepted, so a filter may
// index its grid unguarded. The scan copies the filter by value, so a filter reads; one that
// recorded what it was asked would record into its own copy.
/// <summary>Decides which on-board cells a <see cref="GridNeighbors"/> scan may step into.</summary>
public interface IGridCellFilter
{
    /// <summary>Reports whether a search may step into a cell, which is always one on the board.</summary>
    /// <param name="row">The cell's row, already on the board, so the filter may index its grid unguarded.</param>
    /// <param name="col">The cell's column, already on the board, so the filter may index its grid unguarded.</param>
    /// <returns>True to let the search enter the cell; false to keep it out.</returns>
    bool CanEnter(int row, int col);
}
