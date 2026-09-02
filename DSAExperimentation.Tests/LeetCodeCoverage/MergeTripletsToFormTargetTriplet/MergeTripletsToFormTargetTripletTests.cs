using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MergeTripletsToFormTargetTriplet;

// LeetCode 1899. Merge Triplets to Form Target Triplet: a "compatible" triplet
// (every coordinate <= target's) can only ever help the component-wise-max merge,
// never push a coordinate past target - so the whole problem reduces to "does some
// compatible triplet hit target[i] exactly, for every i in {0,1,2}," tracked with
// this repo's own Set<int> over the matched coordinate indices, the same
// membership-tracking role ContainsDuplicateTests already gives it.
public sealed partial class MergeTripletsToFormTargetTripletTests
{
    [Fact]
    public void CanFormTarget_CompatibleTripletsTogetherCoverEveryCoordinate_ReturnsTrue()
    {
        int[][] triplets = [[2, 5, 3], [1, 8, 4], [1, 7, 5]];
        int[] target = [2, 7, 5];

        var actual = CanFormTarget(triplets, target);
        Assert.True(actual);
    }

    [Fact]
    public void CanFormTarget_NoTripletReachesEveryCoordinate_ReturnsFalse()
    {
        int[][] triplets = [[1, 3, 4], [2, 2, 2]];
        int[] target = [3, 4, 5];

        var actual = CanFormTarget(triplets, target);
        Assert.False(actual);
    }

    [Fact]
    public void CanFormTarget_IncompatibleTripletIsSkippedButMatchStillFound_ReturnsTrue()
    {
        int[][] triplets = [[9, 9, 9], [3, 4, 5]];
        int[] target = [3, 4, 5];

        var actual = CanFormTarget(triplets, target);
        Assert.True(actual);
    }

    private static bool CanFormTarget(int[][] triplets, int[] target)
    {
        var matched = new Set<int>();

        foreach (var triplet in triplets)
        {
            if (triplet[0] > target[0] || triplet[1] > target[1] || triplet[2] > target[2])
            {
                continue;
            }

            for (var i = 0; i < 3; i++)
            {
                if (triplet[i] == target[i])
                {
                    matched.TryAdd(i);
                }
            }
        }

        return matched.Count == 3;
    }
}
