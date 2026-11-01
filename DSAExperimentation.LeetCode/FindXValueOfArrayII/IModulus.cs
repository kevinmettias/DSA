namespace DSAExperimentation.LeetCode.FindXValueOfArrayII;

// LC 3525 bounds k to 1 <= k <= 5 - a genuinely closed set, not an arbitrary runtime
// modulus the way Domain.Modular.ModularArithmetic's 1e9+7 is a single fixed value.
// XValueCombineOperation<TModulus> needs k available at the generic-instantiation
// site (ICombineOperation<Element>'s own doc comment: Identity is a static-abstract
// property with no runtime channel to receive a value through), so each of the five
// legal values gets its own witness rather than k being a runtime field threaded
// through the segment tree's element type.
internal interface IModulus
{
    static abstract int Value { get; }
}
