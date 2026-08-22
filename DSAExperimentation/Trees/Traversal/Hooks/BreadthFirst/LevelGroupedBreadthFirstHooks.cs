namespace DSAExperimentation.Trees;

public readonly struct LevelGroupedBreadthFirstHooks<TNode, TAction>
    : IBreadthFirstTraversalHooks<TNode, LevelGroupedBreadthFirstHooks<TNode, TAction>>
    where TAction : struct, ILevelVisitAction<TNode>
{
    private static readonly ContiguousGroupBuffer<TNode, int> Buffer = new();

    public static void OnDiscover(TNode node, int depth)
    {
        if (depth == 0)
        {
            Buffer.Reset();
        }
    }

    public static void OnVisit(TNode node, int depth)
        => Buffer.Add(node, depth, TAction.Invoke);

    public static void OnFinish()
        => Buffer.Flush(TAction.Invoke);
}
