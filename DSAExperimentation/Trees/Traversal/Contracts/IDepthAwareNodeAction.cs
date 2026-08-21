namespace DSAExperimentation.Trees;

public interface IDepthAwareNodeAction<TNode>
{
    static abstract void Invoke(TNode node, int depth);
}




