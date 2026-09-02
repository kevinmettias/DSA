using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LargestComponentSizeByCommonFactor;

// LeetCode 952. Largest Component Size by Common Factor: DisjointSet over value indices,
// unioned whenever two values share a prime factor (first-seen owner per factor tracked
// via HashMap<factor,index>) - the same AccountsMergeTests union-on-shared-key shape
// applied to prime factors instead of emails. Component sizes are then tallied with a
// second HashMap<root,count>.
public sealed partial class LargestComponentSizeByCommonFactorTests
{
    [Fact]
    public void LargestComponentSize_ClassicExample_AllFourValuesChainTogether()
    {
        int[] nums = [4, 6, 15, 35];

        var size = LargestComponentSize(nums);

        Assert.Equal(4, size);
    }

    [Fact]
    public void LargestComponentSize_TwoDisjointPairs_ReturnsSizeOfEitherPair()
    {
        int[] nums = [20, 50, 9, 63];

        var size = LargestComponentSize(nums);

        Assert.Equal(2, size);
    }

    private static int LargestComponentSize(int[] nums)
    {
        var components = new DisjointSet(nums.Length);
        UnionSharedFactors(nums, components);

        var sizeByRoot = new HashMap<int, int>();
        var largest = 1;

        for (var i = 0; i < nums.Length; i++)
        {
            var count = TallyComponent(components, sizeByRoot, i);
            largest = Math.Max(largest, count);
        }

        return largest;
    }

    private static void UnionSharedFactors(int[] nums, DisjointSet components)
    {
        var firstIndexWithFactor = new HashMap<int, int>();

        for (var i = 0; i < nums.Length; i++)
        {
            foreach (var factor in PrimeFactors(nums[i]))
            {
                if (firstIndexWithFactor.TryGetValue(factor, out var owner))
                {
                    components.Union(i, owner);
                }
                else
                {
                    firstIndexWithFactor.Set(factor, i);
                }
            }
        }
    }

    private static int TallyComponent(DisjointSet components, HashMap<int, int> sizeByRoot, int i)
    {
        var root = components.Find(i);
        sizeByRoot.TryGetValue(root, out var count);
        count++;
        sizeByRoot.Set(root, count);
        return count;
    }

    private static IEnumerable<int> PrimeFactors(int value)
    {
        for (var factor = 2; factor * factor <= value; factor++)
        {
            if (value % factor != 0)
            {
                continue;
            }

            yield return factor;

            while (value % factor == 0)
            {
                value /= factor;
            }
        }

        if (value > 1)
        {
            yield return value;
        }
    }
}
