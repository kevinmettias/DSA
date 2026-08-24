namespace DSAExperimentation.Graph;

// A canonical proof for ITopDownHooks: every root-to-leaf path is naturally
// top-down (a path is exactly "the state a node inherited from its parent, plus
// itself") and awkward bottom-up - a Fold's Combine only ever sees a node together
// with its children's *finished* results, with no way to hand each child a copy of
// "the path so far" before that child even starts.
//
// The output list travels as part of TState itself (never copied, same reference at
// every node) rather than through a static mutable field - the same "state carries a
// reference to shared context" pattern GridNode uses for its Grid.
public static class AllRootToLeafPaths
{
    public static List<TNode[]> Find<TNode, TTopology, TChildren, TOrder, TOrderedChildren>(TNode? root)
        where TNode : class
        where TTopology : struct, ITreeTopology<TNode, TChildren>
        where TChildren : struct, IChildren<TNode>
        where TOrder : struct, IChildOrder<TNode, TChildren, TOrderedChildren>
        where TOrderedChildren : struct, IChildren<TNode>
    {
        var output = new List<TNode[]>();

        TopDownTraversal.Walk<
            TNode, TTopology, TChildren, TOrder, TOrderedChildren,
            CollectPathsHooks<TNode>, (TNode[] Path, List<TNode[]> Output)>(
            root, (Path: root is null ? [] : [root], Output: output));

        return output;
    }

    private readonly struct CollectPathsHooks<TNode> : ITopDownHooks<TNode, (TNode[] Path, List<TNode[]> Output)>
        where TNode : class
    {
        public static void Visit(TNode node, (TNode[] Path, List<TNode[]> Output) state, int depth, bool isLeaf)
        {
            if (isLeaf)
            {
                state.Output.Add(state.Path);
            }
        }

        public static (TNode[] Path, List<TNode[]> Output) Descend(
            TNode parent, (TNode[] Path, List<TNode[]> Output) parentState, TNode child)
        {
            var path = new TNode[parentState.Path.Length + 1];
            Array.Copy(parentState.Path, path, parentState.Path.Length);
            path[^1] = child;

            return (path, parentState.Output);
        }
    }
}
