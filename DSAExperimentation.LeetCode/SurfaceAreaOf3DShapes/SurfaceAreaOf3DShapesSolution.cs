namespace DSAExperimentation.LeetCode.SurfaceAreaOf3DShapes;

// LeetCode 892. Surface Area of 3D Shapes: for each grid cell's unit-cube stack, sum
// top+bottom (2, once any cubes are stacked there) plus the exposed area on each of
// the 4 orthogonal sides - the height difference against that neighbor's stack, or
// the full stack height against the implicit height-0 ground outside the grid.
//
// Both strategies are inherently O(n^2) - there is no better asymptotic algorithm
// available, the same "no complexity split to compare" shape TransposeMatrix already
// documents for fixed-shape grid arithmetic - so the pair compares two real
// constant-factor strategies:
//
// - BoundsCheckedNeighbors re-derives each neighbor's bounds check inline per
//   direction. It is the baseline, and what you would write without this repo, so
//   its internals stay BCL.
// - PaddedBorder rings the grid with height-0 cells once up front so the inner loop
//   can index all 4 neighbors directly with zero per-cell branches.
//
// No repo Representation/Operations primitive applies to either shape: Grid and
// GridChildren model unordered orthogonal adjacency for graph walks, not a per-cell
// height-difference sum, so routing this through Grid/** would not be a genuine fit -
// the same reasoning TransposeMatrix and SpiralMatrixII already state for their index
// arithmetic.
internal static class SurfaceAreaOf3DShapesSolution
{
    // Top face + bottom face contributed by every non-zero-height cell.
    private const int TopAndBottomFaceArea = 2;

    // One extra height-0 cell on each side of the grid.
    private const int BorderPadding = 2;

    // Baseline: walk the grid as given, bounds-checking each of the 4 neighbor
    // lookups and treating anything off the edge as height 0.
    public static int SurfaceAreaByBoundsCheckedNeighbors(int[][] grid)
    {
        var view = new GridView(grid, grid.Length);
        var area = 0;

        for (var row = 0; row < view.Size; row++)
        {
            for (var col = 0; col < view.Size; col++)
            {
                area += CellExposedArea(view, row, col);
            }
        }

        return area;
    }

    // One cell's whole contribution: top and bottom faces once it carries any cubes,
    // plus each of the 4 orthogonal sides as the height difference against that
    // neighbour's stack.
    private static int CellExposedArea(GridView view, int row, int col)
    {
        var height = view.Cells[row][col];

        if (height == 0)
        {
            return 0;
        }

        var area = TopAndBottomFaceArea;
        area += ExposedSide(view, height, row - 1, col);
        area += ExposedSide(view, height, row + 1, col);
        area += ExposedSide(view, height, row, col - 1);
        area += ExposedSide(view, height, row, col + 1);
        return area;
    }

    private readonly record struct GridView(int[][] Cells, int Size);

    private static int ExposedSide(GridView view, int height, int row, int col)
    {
        var neighborHeight = IsInside(view, row, col) ? HeightAt(view, row, col) : 0;

        return Math.Max(0, height - neighborHeight);
    }

    // Every side probe steps one cell outside the grid, so both coordinates have to
    // be in range before the neighbour's height may be read at all.
    private static bool IsInside(GridView view, int row, int col) =>
        row >= 0 && row < view.Size && col >= 0 && col < view.Size;

    // The stack height at a cell - read only once IsInside has vouched for the
    // coordinates.
    private static int HeightAt(GridView view, int row, int col) => view.Cells[row][col];

    // Pay for one height-0 border up front, then index all 4 neighbors
    // unconditionally - the per-cell branches the baseline spends become a single
    // copy pass.
    public static int SurfaceAreaByPaddedBorder(int[][] grid)
    {
        var padded = BuildPaddedGrid(grid);

        return SumPaddedArea(padded, grid.Length);
    }

    private static int[][] BuildPaddedGrid(int[][] grid)
    {
        var n = grid.Length;
        var padded = new int[n + BorderPadding][];

        for (var r = 0; r < n + BorderPadding; r++)
        {
            padded[r] = new int[n + BorderPadding];
        }

        for (var r = 0; r < n; r++)
        {
            for (var c = 0; c < n; c++)
            {
                padded[r + 1][c + 1] = grid[r][c];
            }
        }

        return padded;
    }

    private static int SumPaddedArea(int[][] padded, int n)
    {
        var area = 0;

        for (var r = 1; r <= n; r++)
        {
            for (var c = 1; c <= n; c++)
            {
                area += PaddedCellExposedArea(padded, r, c);
            }
        }

        return area;
    }

    private static int PaddedCellExposedArea(int[][] padded, int r, int c)
    {
        var height = padded[r][c];

        if (height == 0)
        {
            return 0;
        }

        var area = TopAndBottomFaceArea;
        area += Math.Max(0, height - padded[r - 1][c]);
        area += Math.Max(0, height - padded[r + 1][c]);
        area += Math.Max(0, height - padded[r][c - 1]);
        area += Math.Max(0, height - padded[r][c + 1]);
        return area;
    }
}
