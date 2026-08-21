namespace DSAExperimentation.Trees;

internal readonly record struct TraversalFrame<TNode>(
    TNode Node,
    int Depth)
    where TNode : class;
