using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.GoodSubsequenceQueries;

// LC 3901's own prepared-input witness for the composed strategy - the
// "graph already built" role LockGraph plays for OpenTheLock's ReduceGraph arm,
// or SegmentTree<int,GcdOperation> plays bare for MinimumStabilityFactorOfArray.
// A subsequence's gcd can only equal p exactly when every element it draws from
// is a multiple of p, so this tracks nums divided by p wherever p divides it (0 -
// GcdOperation's identity - everywhere else, so a non-multiple never contaminates
// a range's combined gcd) alongside a running count of how many positions are
// multiples at all: two independent SegmentTree<int,TOperation> instantiations
// answering "range gcd" and "range multiple-count" in O(log n) apiece, both point
// updated together on every query so CountGoodSubseqBySegmentTreeGcd never rescans
// the whole array except in the one case explained on HasGoodSubsequenceByRangeQuery.
internal sealed class GoodSubsequenceIndex
{
    public SegmentTree<int, GcdOperation> GcdTree { get; }
    public SegmentTree<int, SumOperation<int>> MultipleCountTree { get; }
    public int[] Divided { get; }
    public int P { get; }

    private GoodSubsequenceIndex(
        SegmentTree<int, GcdOperation> gcdTree,
        SegmentTree<int, SumOperation<int>> multipleCountTree,
        int[] divided,
        int p)
    {
        GcdTree = gcdTree;
        MultipleCountTree = multipleCountTree;
        Divided = divided;
        P = p;
    }

    public static GoodSubsequenceIndex Build(int[] nums, int p)
    {
        var divided = new int[nums.Length];
        var multiples = new int[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            divided[i] = DivideByP(nums[i], p);
            multiples[i] = divided[i] == 0 ? 0 : 1;
        }

        return new GoodSubsequenceIndex(
            new SegmentTree<int, GcdOperation>(divided),
            new SegmentTree<int, SumOperation<int>>(multiples),
            divided,
            p);
    }

    public void Apply(int index, int value)
    {
        var divided = DivideByP(value, P);

        Divided[index] = divided;
        GcdTree.Update(index, divided);
        MultipleCountTree.Update(index, divided == 0 ? 0 : 1);
    }

    private static int DivideByP(int value, int p) => value % p == 0 ? value / p : 0;
}
