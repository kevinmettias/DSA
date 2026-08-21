namespace DSAExperimentation.Trees;

public interface IBreadthFirstFoldAlgebra<TNode, TState>
{
    static abstract TState Seed { get; }

    static abstract TState Accumulate(
        TState state,
        TNode node,
        int depth);
}




