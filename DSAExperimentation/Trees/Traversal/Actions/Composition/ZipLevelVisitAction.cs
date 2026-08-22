namespace DSAExperimentation.Trees;

public readonly struct ZipLevelVisitAction<TNode, TFirst, TSecond> : ILevelVisitAction<TNode>
    where TFirst : struct, ILevelVisitAction<TNode>
    where TSecond : struct, ILevelVisitAction<TNode>
{
    public static void Invoke(IReadOnlyList<TNode> level, int depth)
    {
        TFirst.Invoke(level, depth);
        TSecond.Invoke(level, depth);
    }
}
