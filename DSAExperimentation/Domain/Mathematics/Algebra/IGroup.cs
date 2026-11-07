namespace DSAExperimentation.Domain.Mathematics.Algebra;

// A group: a monoid whose Combine can be undone, every element having an inverse that combines
// with it to the identity on either side. Combine need not commute. The permutations of a set,
// combined by doing one after the other, are the standard example (Domain/Mathematics/Groups):
// there Combine(a, b) and Combine(b, a) differ in general, so which operand acts first is part of
// what an instance states, and IMonoid's "left first" is read literally. A structure that regroups
// or reorders what it combines needs the commutative refinement, IAbelianGroup.
//
// Invert is declared here once, and IAbelianGroup adds only its law. An operation every group has
// is not declared again by the abelian ones, which would make two contracts for one operation
// (ARCHITECTURE.md §11.4). The constrained call a structure compiles resolves to the same struct
// method wherever the member is declared, and the tier-1 code of LC 2659's Fenwick arm is the same
// before and after the move (§11.4).
/// <summary>
/// An associative combine with an identity and an inverse for every element, not necessarily
/// commutative.
/// </summary>
public interface IGroup<Element> : IMonoid<Element>
{
    /// <summary>Finds the element that undoes another under Combine.</summary>
    /// <param name="value">The element to invert.</param>
    /// <returns>
    /// The element that, combined with <paramref name="value"/> on either side, gives the identity.
    /// </returns>
    static abstract Element Invert(Element value);
}
