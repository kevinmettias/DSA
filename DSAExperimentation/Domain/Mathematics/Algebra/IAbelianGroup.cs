namespace DSAExperimentation.Domain.Mathematics.Algebra;

// An abelian group: a monoid whose Combine can be undone. FenwickTree needs exactly this and nothing
// weaker - it stores prefix values and answers a range query as Combine(prefix(right),
// Invert(prefix(left - 1))), combining prefixes in whatever order its bit arithmetic visits them.
//
// Min and Max deliberately never implement it: neither has an inverse (min(a, b) = 3 with b = 5
// does not recover a), so a Fenwick tree over either is a compile error, and the chain is where
// that refusal is explained.
/// <summary>
/// A commutative combine operation with an inverse for every element, so a range can be answered from two prefixes.
/// </summary>
public interface IAbelianGroup<Element> : IMonoid<Element>
{
    /// <summary>Finds the element that undoes another under Combine.</summary>
    /// <param name="value">The element to invert.</param>
    /// <returns>The element that, combined with <paramref name="value"/> on either side, gives the identity.</returns>
    static abstract Element Invert(Element value);
}
