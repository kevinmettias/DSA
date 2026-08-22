namespace DSAExperimentation.Trees;

public readonly struct LevelGroupedBreadthFirstHooks<TNode, TAction>
    : IBreadthFirstTraversalHooks<TNode, LevelGroupedBreadthFirstHooks<TNode, TAction>>
    where TAction : struct, ILevelVisitAction<TNode>
{
    private static readonly ContiguousGroupBuffer<TNode, int> Buffer = new();

    public static void NodeDiscovered(TNode node, int depth)
    {
        if (depth == 0)
        {
            Buffer.Reset();
        }
    }

    public static void Visit(TNode node, int depth)
        => Buffer.Add(node, depth, TAction.Invoke);

    public static void TraversalFinished()
        => Buffer.Flush(TAction.Invoke);
}
