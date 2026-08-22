namespace DSAExperimentation.Trees;

public interface IBreadthFirstTraversalHooks<TNode, TSelf>
    : IBreadthFirstReduceAlgebra<TNode, Unit>
    where TSelf : IBreadthFirstTraversalHooks<TNode, TSelf>
{
    static Unit IBreadthFirstReduceAlgebra<TNode, Unit>.Seed
        => default;

    static void IBreadthFirstReduceAlgebra<TNode, Unit>.Discover(TNode node, int depth)
        => TSelf.NodeDiscovered(node, depth);

    static Unit IBreadthFirstReduceAlgebra<TNode, Unit>.Accumulate(Unit state, TNode node, int depth)
    {
        TSelf.Visit(node, depth);
        return default;
    }

    static void IBreadthFirstReduceAlgebra<TNode, Unit>.Finish()
        => TSelf.TraversalFinished();

    static virtual void NodeDiscovered(TNode node, int depth)
    {
    }

    static virtual void Visit(TNode node, int depth)
    {
    }

    static virtual void TraversalFinished()
    {
    }
}
