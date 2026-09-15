namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// LeetCode 113's own example tree, node by node. Two of its four root-to-leaf paths
// sum to TargetSum: Root -> Left -> LeftLeft -> LeftLeftRight (5 -> 4 -> 11 -> 2) and
// Root -> Right -> RightRight -> RightRightLeft (5 -> 8 -> 4 -> 5); LeftLeftLeft and
// RightRightRight are the two remaining leaves those have to be told apart from, and
// both benchmark arms have to return the paths themselves rather than a count.
//
// Separate from PathSumIIBenchmarks because these are the values the benchmark
// drives, not part of how it drives them - eleven of them beside that class's own
// members is a second subject, and Tree() below is the only thing the two have to
// agree on.
internal static class PathSumIIExampleTree
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
    public const int RightRightLeft = 5;
    public const int RightRightRight = 1;
}
