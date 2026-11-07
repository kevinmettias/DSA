using System.Runtime.CompilerServices;

namespace DSAExperimentation.Domain.Mathematics.Algebra;

// The direct product of two abelian groups: pairs, combined and inverted component by component,
// with the pair of identities as the identity. It is again an abelian group, because each law holds
// of a pair exactly when it holds of both components. A structure written over one group can then
// carry two values through one walk: the range configuration of the Fenwick tree keeps a difference
// and its position-weighted term in each node this way (ARCHITECTURE.md §13.13).
//
// The components may be different groups over different types. A product of two monoids, or of two
// integer modules, is one too, and would be a type of its own: C# cannot implement a refinement of
// the chain only when both components do. The components' witnesses are type arguments rather than
// reached through anything else, so a pair of value-type elements compiles to the two operations
// side by side.
//
// Identity is always inlined. Its body, two identities and a tuple, is a little over what the JIT
// inlines without asking, and without profile data it judged the call unprofitable: the range
// Fenwick tree's constructor called it once per node and PrefixQuery once per query, where the
// point tree's Element.Zero is no call at all (ARCHITECTURE.md §13.13).
/// <summary>
/// The direct product of two abelian groups: pairs combined, inverted and started from the identity
/// one component at a time.
/// </summary>
/// <typeparam name="TFirstElement">The first component's type.</typeparam>
/// <typeparam name="TSecondElement">The second component's type.</typeparam>
/// <typeparam name="TFirst">The group the first components are combined in.</typeparam>
/// <typeparam name="TSecond">The group the second components are combined in.</typeparam>
public readonly struct DirectProduct<TFirstElement, TSecondElement, TFirst, TSecond>
    : IAbelianGroup<(TFirstElement First, TSecondElement Second)>
    where TFirst : struct, IAbelianGroup<TFirstElement>
    where TSecond : struct, IAbelianGroup<TSecondElement>
{
    /// <summary>The pair of the two groups' identities.</summary>
    public static (TFirstElement First, TSecondElement Second) Identity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (TFirst.Identity, TSecond.Identity);
    }

    /// <summary>Combines two pairs, each component in its own group.</summary>
    /// <param name="left">The first pair.</param>
    /// <param name="right">The second pair.</param>
    /// <returns>The first components combined, and the second components combined.</returns>
    public static (TFirstElement First, TSecondElement Second) Combine(
        (TFirstElement First, TSecondElement Second) left,
        (TFirstElement First, TSecondElement Second) right) =>
        (TFirst.Combine(left.First, right.First), TSecond.Combine(left.Second, right.Second));

    /// <inheritdoc/>
    public static (TFirstElement First, TSecondElement Second) Invert(
        (TFirstElement First, TSecondElement Second) value) =>
        (TFirst.Invert(value.First), TSecond.Invert(value.Second));
}
