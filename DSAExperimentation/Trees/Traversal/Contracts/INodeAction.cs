namespace DSAExperimentation.Trees;

public interface INodeAction<TNode>
{
    static abstract void Invoke(TNode node);
}




