namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// LeetCode 112's own example tree, node by node. The accepting root-to-leaf path is
// Root -> Left -> LeftLeft -> LeftLeftRight (5 -> 4 -> 11 -> 2), summing to TargetSum,
// the only one of the tree's four root-to-leaf paths that does; LeftLeftLeft and
// RightRightRight are the two remaining leaves it has to be told apart from.
//
// Separate from PathSumBenchmarks because these are the values the benchmark drives,
// not part of how it drives them - ten of them beside that class's own members is a
// second subject, and Tree() below is the only thing the two have to agree on.
internal static class PathSumExampleTree
{
    public const int TargetSum = 22;
    public const int Root = 5;
    public const int Left = 4;
    public const int LeftLeft = 11;
    public const int LeftLeftLeft = 7;
    public const int LeftLeftRight = 2;
    public const int Right = 8;
    public const int RightLeft = 13;
    public const int RightRight = 4;
    public const int RightRightRight = 1;
}
