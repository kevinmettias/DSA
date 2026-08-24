namespace DSAExperimentation.Graph;

// The tree-only guard: ITreeTopology already promises unique ancestry, so every
// child is visited unconditionally. Stateless, so the constrained call through this
// should cost nothing beyond what a hardcoded "if (true)" would.
public readonly struct UnguardedVisit<TNode> : IVisitGuard<TNode>
    where TNode : class
{
    public bool ShouldVisit(TNode node) => true;
}
