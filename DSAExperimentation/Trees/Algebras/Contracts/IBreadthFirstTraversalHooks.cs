namespace DSAExperimentation.Trees;

public interface IBreadthFirstTraversalHooks<TNode, TSelf>
    : IBreadthFirstFoldAlgebra<TNode, Unit>
    where TSelf : IBreadthFirstTraversalHooks<TNode, TSelf>
{
    static Unit IBreadthFirstFoldAlgebra<TNode, Unit>.Seed
        => default;

    static void IBreadthFirstFoldAlgebra<TNode, Unit>.Discover(TNode node, int depth)
        => TSelf.OnDiscover(node, depth);

    static Unit IBreadthFirstFoldAlgebra<TNode, Unit>.Accumulate(Unit state, TNode node, int depth)
    {
        TSelf.OnVisit(node, depth);
        return default;
    }

    static void IBreadthFirstFoldAlgebra<TNode, Unit>.Finish()
        => TSelf.OnFinish();

    static virtual void OnDiscover(TNode node, int depth)
    {
    }

    static virtual void OnVisit(TNode node, int depth)
    {
    }

    static virtual void OnFinish()
    {
    }
}
