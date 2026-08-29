using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;
namespace DSAExperimentation.Tests.LeetCodeCoverage.TwoSumIIInputArrayIsSorted;
public sealed partial class TwoSumIIInputArrayIsSortedTests { [Fact] public void TwoSum_SortedExample_ReturnsOneBasedIndices()=>Assert.Equal([1,2],TwoSum([2,7,11,15],9)); private static int[] TwoSum(int[] nums,int target){for(var i=0;i<nums.Length;i++){var found=BinarySearch.Find<int,OffsetSequence>(new OffsetSequence(nums,i+1,nums.Length-i-1),target-nums[i]);if(found is not null)return [i+1,i+found.Value+2];}return [];} private readonly struct OffsetSequence(int[] nums,int start,int length):IRandomAccessSequence<int>{public int Length=>length;public int Get(int i)=>nums[start+i];} }
