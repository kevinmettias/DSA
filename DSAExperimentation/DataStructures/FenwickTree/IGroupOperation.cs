namespace DSAExperimentation.DataStructures.FenwickTree;

// A strictly stronger law than SegmentTree.ICombineOperation<Element>'s associativity: Fenwick's
// RangeQuery derives a range's combined value from two prefix values via Invert (subtraction), so
// the operation must be invertible - an abelian group, not just a monoid - not merely
// associative. Standalone, not inheriting ICombineOperation<Element>: Fenwick and SegmentTree are
// different structure identities (different Representation - bit arithmetic vs. complete-binary-
// tree arithmetic), so per ARCHITECTURE.md §5 step 5/§11.1's domain-separation precedent this
// contract stands on its own even though Identity/Combine overlap in shape.
//
// Modeled as a static-abstract witness for the same reason ICombineOperation<Element> is (see its own
// doc comment) - a stateless pure function closed over at the generic-instantiation site, not a
// runtime object carrying closure-captured state.
//
// Min/Max deliberately never get an implementation here: neither has an inverse (given
// min(a,b)=3 and b=5, a is not recoverable), so only genuinely invertible operations (sum, xor,
// ...) belong in this domain.
internal interface IGroupOperation<Element>
{
    static abstract Element Identity { get; }

    static abstract Element Combine(Element left, Element right);

    static abstract Element Invert(Element value);
}
