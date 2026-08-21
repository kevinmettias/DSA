namespace DSAExperimentation.Trees;

internal interface IFoldStrategy<TNode, TResult>
    where TNode : class
{
    static abstract TResult Empty { get; }

    static abstract TResult FoldNode(TNode node);
}
