namespace DSAExperimentation.Trees;

internal interface IChildEnumerationStrategy<TNode>
    where TNode : class
{
    static abstract IEnumerable<TNode> GetChildren(TNode node);
}
