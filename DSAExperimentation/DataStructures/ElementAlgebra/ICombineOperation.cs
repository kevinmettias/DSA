namespace DSAExperimentation.DataStructures.ElementAlgebra;

// A monoid over Element: an associative Combine with an Identity that Combine leaves unchanged. It
// heads the one chain every range structure here draws its element algebra from, each structure
// demanding exactly the position its own arithmetic needs:
//
//   ICombineOperation           SegmentTree        merge two adjacent ranges' values
//   - IGroupOperation           FenwickTree,       + Invert: a range is a difference of two prefixes
//                               PrefixSums
//     - IScaledGroupOperation   RangeFenwickTree   + Scale: a range update is a scaled prefix
//   IRangeUpdateOperation       LazySegmentTree    + an update action, kept structure-local
//
// Associativity is an unenforced precondition law, the same shape as BinarySearch's sortedness: an
// operation that is not associative still builds and queries without error, and silently returns a
// wrong combined value, because grouping ranges differently - as each tree's shape forces - changes
// the answer.
//
// The law belongs to the element type's algebra, as INumber<T>'s does, not to any one structure.
// That is why the chain lives here rather than in SegmentTree/, and why sharing it does not trip §5
// step 5: no structure reuses another structure's contract - each consumes this neutral one. A member
// only one structure needs never joins the chain; it goes in a structure-local refinement, which is
// what IRangeUpdateOperation is. ARCHITECTURE.md §11.4 has the full argument.
//
// A static-abstract witness rather than a runtime object like IComparer<T>: sum, min, max and gcd are
// as open-ended as any comparer, but each is a stateless pure function whose every input arrives
// through its member signatures, closed over at the instantiation site - the reason IHeapOrder and
// IPathHeuristic are witnesses too.
/// <summary>
/// An associative way to combine two elements, with an identity, that range structures take as a static witness.
/// </summary>
public interface ICombineOperation<Element>
{
    /// <summary>The element that, combined with any element on either side, gives back that element.</summary>
    static abstract Element Identity { get; }

    /// <summary>
    /// Combines two elements in order, associatively, so any grouping of a run of elements gives the same result.
    /// </summary>
    /// <param name="left">The first operand, such as the value of the earlier of two adjacent ranges.</param>
    /// <param name="right">The second operand, such as the value of the later range.</param>
    /// <returns>The two elements combined, <paramref name="left"/> first.</returns>
    static abstract Element Combine(Element left, Element right);
}
