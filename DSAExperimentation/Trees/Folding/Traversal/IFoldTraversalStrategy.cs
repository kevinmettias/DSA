namespace DSAExperimentation.Trees;

public interface IFoldTraversalStrategy<TNode>
    where TNode : class
{
    static abstract TResult Evaluate<TTopology, TOrder, TAlgebra, TResult>(TNode root)
        where TTopology : struct, ITreeTopology<TNode>
        where TOrder : struct, IChildOrder<TNode>
        where TAlgebra : struct, IFoldAlgebra<TNode, TResult>;
}
