namespace DSAExperimentation.Algorithms.Folding;

// Runs two fold algebras together in a single traversal instead of two separate
// ones. This works with zero changes to TreeFold/RecursiveFoldEvaluation/
// IterativeFoldEvaluation because IFoldAlgebra<TNode, TResult> never assumed TResult
// was a scalar - a tuple is just another TResult, so "zip two algebras" is itself
// just another algebra, not a new traversal concept. Each algebra rides along as a
// field, so two algebras that carry runtime values zip as easily as two stateless
// ones; the default value zips the two algebras' defaults.
//
// A plain struct, not a readonly one: calling an algebra's members through a readonly
// field of a generic struct type makes a defensive copy on every Combine.
internal struct ZipFoldAlgebra<TNode, TResultA, TResultB, TAlgebraA, TAlgebraB>(TAlgebraA first, TAlgebraB second)
    : IFoldAlgebra<TNode, (TResultA A, TResultB B)>
    where TAlgebraA : struct, IFoldAlgebra<TNode, TResultA>
    where TAlgebraB : struct, IFoldAlgebra<TNode, TResultB>
{
    private TAlgebraA _first = first;
    private TAlgebraB _second = second;

    public (TResultA A, TResultB B) Empty
        => (_first.Empty, _second.Empty);

    public (TResultA A, TResultB B) Combine(TNode node, IReadOnlyList<(TResultA A, TResultB B)> children)
    {
        var childrenA = new TResultA[children.Count];
        var childrenB = new TResultB[children.Count];

        for (var i = 0; i < children.Count; i++)
        {
            childrenA[i] = children[i].A;
            childrenB[i] = children[i].B;
        }

        return (_first.Combine(node, childrenA), _second.Combine(node, childrenB));
    }
}
