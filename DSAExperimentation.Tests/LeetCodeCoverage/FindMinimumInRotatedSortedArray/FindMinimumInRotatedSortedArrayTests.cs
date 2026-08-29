using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;
namespace DSAExperimentation.Tests.LeetCodeCoverage.FindMinimumInRotatedSortedArray;
public sealed partial class FindMinimumInRotatedSortedArrayTests { [Theory] [InlineData(new[]{3,4,5,1,2},1)] [InlineData(new[]{4,5,6,7,0,1,2},0)] public void FindMin_Examples_ReturnsMinimum(int[] nums,int expected)=>Assert.Equal(expected,FindMin(nums)); private static int FindMin(int[] nums){var pivot=BinarySearch.LowerBound<int,PivotSequence>(new PivotSequence(nums),1);return nums[pivot];} private readonly struct PivotSequence(int[] nums):IRandomAccessSequence<int>{public int Length=>nums.Length;public int Get(int i)=>nums[i]<=nums[^1]?1:0;} }
