namespace DSAExperimentation.Trees;

public interface IBinaryFoldTraversalStrategy<TNode>
    where TNode : class
{
    static abstract TResult Evaluate<TTopology, TOrder, TAlgebra, TResult>(TNode root)
        where TTopology : struct, IBinaryTreeTopology<TNode>
        where TOrder : struct, IBinaryChildOrder
        where TAlgebra : struct, IBinaryFoldAlgebra<TNode, TResult>;
}
