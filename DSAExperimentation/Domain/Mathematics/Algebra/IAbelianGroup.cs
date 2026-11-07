namespace DSAExperimentation.Domain.Mathematics.Algebra;

// An abelian group: a group whose Combine commutes, Combine(a, b) = Combine(b, a). FenwickTree
// needs exactly this and nothing weaker - it stores prefix values and answers a range query as
// Combine(prefix(right), Invert(prefix(left - 1))), combining prefixes in whatever order its bit
// arithmetic visits them.
//
// It adds no member to IGroup, only the law, as IPrimeModulus adds only primality to IModulus: the
// inverse is every group's, and is declared on IGroup.
//
// Min and Max deliberately never implement it: neither has an inverse (min(a, b) = 3 with b = 5
// does not recover a), so a Fenwick tree over either is a compile error, and the chain is where
// that refusal is explained.
/// <summary>
/// A commutative combine operation with an inverse for every element, so a range can be answered from two prefixes.
/// </summary>
public interface IAbelianGroup<Element> : IGroup<Element>;
