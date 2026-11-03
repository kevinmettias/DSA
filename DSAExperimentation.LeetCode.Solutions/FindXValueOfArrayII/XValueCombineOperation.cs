using DSAExperimentation.DataStructures.ElementAlgebra;
using DSAExperimentation.Domain.Mathematics.Modular;

namespace DSAExperimentation.LeetCode.Solutions.FindXValueOfArrayII;

// The associative combine SegmentTree<Element,TOperation> needs: two adjacent
// ranges' nodes merge by composing them as functions of the incoming residue r -
// exactly the "left then right" order Build/Query already fix, so a left range's
// own output residue (r * left.Product) is what feeds the right range's Counts
// lookup. Identity is the empty range: Product = 1 mod k (multiplying by nothing
// changes nothing) and an all-zero Counts (no prefixes to land anywhere) - the same
// two-sided identity SumOperation<T>.Identity = T.Zero plays, verified by
// construction rather than a runtime check because TOperation.Combine's own
// associativity law is unenforced everywhere in this tier.
internal readonly struct XValueCombineOperation<TModulus> : ICombineOperation<XValueNode>
    where TModulus : struct, IModulus
{
    // k sizes this problem's arrays, and LeetCode bounds it to 5, so the long a modulus is read as
    // narrows to an int; the JIT folds the constant either way.
    private static int Modulus => (int)TModulus.Value;

    public static XValueNode Identity => new(1 % Modulus, new int[Modulus, Modulus]);

    public static XValueNode Combine(XValueNode left, XValueNode right)
    {
        var k = Modulus;
        var counts = new int[k, k];

        for (var r = 0; r < k; r++)
        {
            var shifted = r * left.Product % k;

            for (var x = 0; x < k; x++)
            {
                counts[r, x] = left.Counts[r, x] + right.Counts[shifted, x];
            }
        }

        return new XValueNode(left.Product * right.Product % k, counts);
    }

    // A single element's node: exactly one prefix (itself), landing on (r * value) %
    // k for every incoming residue r.
    public static XValueNode Leaf(int value)
    {
        var k = Modulus;
        var v = value % k;
        var counts = new int[k, k];

        for (var r = 0; r < k; r++)
        {
            counts[r, r * v % k] = 1;
        }

        return new XValueNode(v, counts);
    }
}
