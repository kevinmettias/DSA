namespace DSAExperimentation.Domain.Mathematics.Surfaces;

// The global shape of a board of rows and columns: where a cell stepped to lands, if anywhere. A grid
// without defects is flat, so by Gauss-Bonnet the closed surfaces it can make are only those of Euler
// characteristic 0, the torus and the Klein bottle; the projective plane and the sphere need cone
// points, as the corners of a cube are. A twisted gluing mirrors a walker that crosses it, so a
// landing reports which of the walker's axes came out reversed.
/// <summary>A board's surface: where a cell's coordinates land after a step, or that they fall off.</summary>
public interface ISurface
{
    /// <summary>Brings a stepped-to cell back onto the board, if the surface lets it.</summary>
    /// <param name="row">The cell's row; on return, the row it lands on.</param>
    /// <param name="col">The cell's column; on return, the column it lands on.</param>
    /// <param name="board">How many rows and columns the board has.</param>
    /// <param name="mirrored">Which of the walker's axes the landing reversed, if it crossed a twisted edge.</param>
    /// <returns>Whether the cell lands on the board.</returns>
    static abstract bool TryLand(ref int row, ref int col, (int Rows, int Cols) board, out MirroredAxes mirrored);
}
