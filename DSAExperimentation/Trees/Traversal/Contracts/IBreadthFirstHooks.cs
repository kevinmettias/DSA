namespace DSAExperimentation.Trees;

public interface IBreadthFirstHooks<TNode>
{
    static abstract void Discover(TNode node, int depth);

    static abstract void Visit(TNode node, int depth);
}




