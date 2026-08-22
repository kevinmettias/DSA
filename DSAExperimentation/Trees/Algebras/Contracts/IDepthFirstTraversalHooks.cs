namespace DSAExperimentation.Trees;

public interface IDepthFirstTraversalHooks<TNode, TSelf>
    : IFoldAlgebra<TNode, Unit>
    where TSelf : IDepthFirstTraversalHooks<TNode, TSelf>
{
    static Unit IFoldAlgebra<TNode, Unit>.Empty
        => default;

    static bool IFoldAlgebra<TNode, Unit>.CollectsChildResults
        => false;

    static void IFoldAlgebra<TNode, Unit>.Enter(TNode node, int depth)
        => TSelf.OnEnter(node, depth);

    static Unit IFoldAlgebra<TNode, Unit>.Combine(TNode node, IReadOnlyList<Unit> children)
    {
        TSelf.OnExit(node);
        return default;
    }

    static virtual void OnEnter(TNode node, int depth)
        => TSelf.OnEnter(node);

    static virtual void OnEnter(TNode node)
    {
    }

    static virtual void OnExit(TNode node)
    {
    }
}