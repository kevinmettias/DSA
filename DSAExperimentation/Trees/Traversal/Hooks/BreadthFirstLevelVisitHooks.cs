namespace DSAExperimentation.Trees;

public readonly struct BreadthFirstLevelVisitHooks<TNode, TAction>
    : IBreadthFirstHooks<TNode>
    where TAction : struct, ILevelVisitAction<TNode>
{
    private static readonly List<TNode> CurrentLevel = [];

    private static int currentDepth;

    public static void Discover(TNode node, int depth)
    {
        if (depth == 0)
        {
            Reset();
        }
    }

    public static void Visit(TNode node, int depth)
    {
        if (CurrentLevel.Count > 0 && depth != currentDepth)
        {
            Flush();
        }

        currentDepth = depth;
        CurrentLevel.Add(node);
    }

    public static void Finish()
        => Flush();

    private static void Flush()
    {
        if (CurrentLevel.Count == 0)
        {
            return;
        }

        TAction.Invoke(CurrentLevel.ToArray(), currentDepth);
        CurrentLevel.Clear();
    }

    private static void Reset()
    {
        CurrentLevel.Clear();
        currentDepth = 0;
    }
}
