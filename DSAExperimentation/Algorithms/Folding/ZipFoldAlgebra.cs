namespace DSAExperimentation.Algorithms.Folding;

// Runs two fold algebras together in a single traversal instead of two separate
// ones. This works with zero changes to TreeFold/RecursiveFoldEvaluation/
// IterativeFoldEvaluation because IFoldAlgebra<TNode, TResult> never assumed TResult
// was a scalar - a tuple is just another TResult, so "zip two algebras" is itself
// just another algebra, not a new traversal concept.
internal readonly struct ZipFoldAlgebra<TNode, TResultA, TResultB, TAlgebraA, TAlgebraB>
    : IFoldAlgebra<TNode, (TResultA A, TResultB B)>
    where TAlgebraA : struct, IFoldAlgebra<TNode, TResultA>
    where TAlgebraB : struct, IFoldAlgebra<TNode, TResultB>
{
    public static (TResultA A, TResultB B) Empty
        => (TAlgebraA.Empty, TAlgebraB.Empty);

    public static void Enter(TNode node, int depth)
    {
        TAlgebraA.Enter(node, depth);
        TAlgebraB.Enter(node, depth);
    }

    public static (TResultA A, TResultB B) Combine(TNode node, IReadOnlyList<(TResultA A, TResultB B)> children)
    {
        var childrenA = new TResultA[children.Count];
        var childrenB = new TResultB[children.Count];

        for (var i = 0; i < children.Count; i++)
        {
            childrenA[i] = children[i].A;
            childrenB[i] = children[i].B;
        }

        return (TAlgebraA.Combine(node, childrenA), TAlgebraB.Combine(node, childrenB));
    }
}
