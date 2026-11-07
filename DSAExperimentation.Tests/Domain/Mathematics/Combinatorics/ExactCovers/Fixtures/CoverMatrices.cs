namespace DSAExperimentation.Tests.Domain.Mathematics.Combinatorics.ExactCovers.Fixtures;

// Small matrices whose covers are known by hand.
internal static class CoverMatrices
{
    // Knuth's example from "Dancing Links" (2000): columns A to G, and six rows, of which only
    // rows 0, 3 and 4 (his 1, 4 and 5: {C, E, F}, {A, D} and {B, G}) cover every column once.
    public static MatrixProblem Knuth { get; } = new(7, 0,
    [
        [2, 4, 5],
        [0, 3, 6],
        [1, 2, 5],
        [0, 3],
        [1, 6],
        [3, 4, 6],
    ]);

    // Knuth's cover, in the order a search taking the column with the fewest rows chooses it:
    // column A first (two rows meet it, and no column fewer), whose first row fails and whose
    // second is row 3; then column E, which only row 0 still meets; then column B, met only by
    // row 4. A search taking the first column left would choose B before E: [3, 4, 0].
    public static int[] KnuthCover { get; } = [3, 0, 4];

    // Two primary columns and a secondary one. Rows 0 and 1 cover the primaries but both meet the
    // secondary column, so they clash; each goes with the other primary's plain row, and rows 2
    // and 3 cover the primaries alone: three covers, {0, 3}, {1, 2} and {2, 3}. Were the secondary
    // column primary there would be two; were it free, four.
    public static MatrixProblem WithSecondary { get; } = new(2, 1,
    [
        [0, 2],
        [1, 2],
        [0],
        [1],
    ]);

    // Column 2 is met by no row, so there is no cover at all.
    public static MatrixProblem Uncoverable { get; } = new(3, 0, [[0], [1], [0, 1]]);

    // Every pair of rows clashes on column 0 or column 1, and no row covers both alone.
    public static MatrixProblem Clashing { get; } = new(3, 0, [[0, 1], [0, 2], [1, 2]]);
}
