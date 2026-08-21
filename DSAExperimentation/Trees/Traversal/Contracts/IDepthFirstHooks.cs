namespace DSAExperimentation.Trees;

public interface IDepthFirstHooks<TNode>
{
    static abstract void Enter(TNode node);

    static abstract void Exit(TNode node);
}
