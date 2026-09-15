namespace DSAExperimentation.LeetCode.EscapeALargeMaze;

// LC 1036's own board: 10^6 x 10^6, so coordinates run 0 .. Size - 1.
//
// Owned here rather than in EscapeALargeMazeSolution because the tests name it too,
// to derive the far corner they place their examples against - the same setting read
// from two places, which is what a type of its own is for.
internal static class EscapeALargeMazeBoard
{
    public const int Size = 1_000_000;
}
