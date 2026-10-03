namespace DSAExperimentation.DataStructures.ElementAlgebra;

// A monoid over Element: an associative Combine with an Identity that Combine leaves unchanged. It
// heads the one chain every range structure here draws its element algebra from, each structure
// demanding exactly the position its own arithmetic needs:
//
//   ICombineOperation           SegmentTree        merge two adjacent ranges' values
//   - IGroupOperation           FenwickTree        + Invert: a range is a difference of two prefixes
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
internal interface ICombineOperation<Element>
{
    static abstract Element Identity { get; }

    static abstract Element Combine(Element left, Element right);
}
