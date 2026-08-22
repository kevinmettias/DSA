namespace DSAExperimentation.Trees;

public interface IBreadthFirstNodeVisitHooks<TNode, TSelf>
    : IBreadthFirstFoldAlgebra<TNode, Unit>
    where TSelf : IBreadthFirstNodeVisitHooks<TNode, TSelf>
{
    static Unit IBreadthFirstFoldAlgebra<TNode, Unit>.Seed
        => default;

    static Unit IBreadthFirstFoldAlgebra<TNode, Unit>.Accumulate(Unit state, TNode node, int depth)
    {
        TSelf.OnVisit(node);
        return default;
    }

    static virtual void OnVisit(TNode node)
    {
    }
}
