namespace DSAExperimentation.Trees;

public interface IDepthFirstTraversalHooks<TNode, TSelf>
    : IDepthFirstFoldAlgebra<TNode, Unit>
    where TSelf : IDepthFirstTraversalHooks<TNode, TSelf>
{
    static Unit IDepthFirstFoldAlgebra<TNode, Unit>.Empty
        => default;

    static bool IDepthFirstFoldAlgebra<TNode, Unit>.CollectsChildResults
        => false;

    static void IDepthFirstFoldAlgebra<TNode, Unit>.Enter(TNode node)
        => TSelf.OnEnter(node);

    static Unit IDepthFirstFoldAlgebra<TNode, Unit>.Combine(TNode node, IReadOnlyList<Unit> children)
    {
        TSelf.OnExit(node);
        return default;
    }

    static virtual void OnEnter(TNode node)
    {
    }

    static virtual void OnExit(TNode node)
    {
    }
}
