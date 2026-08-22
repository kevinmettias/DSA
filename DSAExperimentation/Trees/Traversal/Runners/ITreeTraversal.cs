namespace DSAExperimentation.Trees;

public interface ITreeTraversal<TNode>
    where TNode : class
{
    static abstract void Traverse(TNode? root);
}
