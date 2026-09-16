using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.GoodSubsequenceQueries;

// LC 3901's own prepared-input witness for the composed strategy - the
// "graph already built" role LockGraph plays for OpenTheLock's ReduceGraph arm,
// or SegmentTree<int,GcdOperation> plays bare for MinimumStabilityFactorOfArray.
// A subsequence's gcd can only equal `modulus` exactly when every element it draws
// from is a multiple of `modulus`, so this tracks nums divided by `modulus` wherever
// it divides the element (0 - GcdOperation's identity - everywhere else, so a
// non-multiple never contaminates a range's combined gcd) alongside a running count
// of how many positions are multiples at all: two SegmentTree<int,TOperation> instantiations
// answering "range gcd" and "range multiple-count" in O(log n) apiece, both point
// updated together on every query so CountGoodSubseqBySegmentTreeGcd never rescans
// the whole array except in the one case explained on HasGoodSubsequenceByRangeQuery.
internal sealed class GoodSubsequenceIndex(
    SegmentTree<int, GcdOperation> gcdTree,
    SegmentTree<int, SumOperation<int>> multipleCountTree,
    int[] divided,
    int modulus)
{
    // The three views GoodSubsequenceQueriesSolution reads by name, kept as
    // properties over the primary constructor's own values. The modulus has no
    // reader outside this type, so it stays a plain primary constructor parameter.
    public SegmentTree<int, GcdOperation> GcdTree => gcdTree;

    public SegmentTree<int, SumOperation<int>> MultipleCountTree => multipleCountTree;

    public int[] Divided => divided;

    public static GoodSubsequenceIndex Build(int[] nums, int modulus)
    {
        var divided = new int[nums.Length];
        var multiples = new int[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            divided[i] = DivideByP(nums[i], modulus);
            var isNotMultipleOfModulus = divided[i] == 0;
            multiples[i] = isNotMultipleOfModulus ? 0 : 1;
        }

        return new GoodSubsequenceIndex(
            new SegmentTree<int, GcdOperation>(divided),
            new SegmentTree<int, SumOperation<int>>(multiples),
            divided,
            modulus);
    }

    public void Apply(int index, int value)
    {
        var dividedValue = DivideByP(value, modulus);

        divided[index] = dividedValue;
        gcdTree.Update(index, dividedValue);
        multipleCountTree.Update(index, dividedValue == 0 ? 0 : 1);
    }

    private static int DivideByP(int value, int modulus)
    {
        var modulusDividesValue = value % modulus == 0;

        return modulusDividesValue ? QuotientOf(value, modulus) : 0;
    }

    private static int QuotientOf(int value, int modulus) => value / modulus;
}
