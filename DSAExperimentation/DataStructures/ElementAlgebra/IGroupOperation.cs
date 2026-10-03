namespace DSAExperimentation.DataStructures.ElementAlgebra;

// An abelian group: a monoid whose Combine can be undone. FenwickTree needs exactly this and nothing
// weaker - it stores prefix values and answers a range query as Combine(prefix(right),
// Invert(prefix(left - 1))), combining prefixes in whatever order its bit arithmetic visits them.
//
// Min and Max deliberately never implement it: neither has an inverse (min(a, b) = 3 with b = 5
// does not recover a), so a Fenwick tree over either is a compile error, and the chain is where
// that refusal is explained.
internal interface IGroupOperation<Element> : ICombineOperation<Element>
{
    static abstract Element Invert(Element value);
}
