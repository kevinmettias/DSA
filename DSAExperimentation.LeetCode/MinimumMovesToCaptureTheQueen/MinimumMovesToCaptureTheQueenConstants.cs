namespace DSAExperimentation.LeetCode.MinimumMovesToCaptureTheQueen;

// The values LC 3001 fixes, declared once so MinimumMovesToCaptureTheQueenSolution and its
// benchmark read the same copy instead of each restating it.
internal static class MinimumMovesToCaptureTheQueenConstants
{
    // The board is a fixed 8x8 grid of 1-indexed ranks and files, so both coordinates
    // being within it is one bound check rather than two bare eights.
    internal const int BoardSize = 8;
}
