namespace DSAExperimentation.LeetCode.KthLargestElementInAStream;

// The Add contract every strategy above implements. Bespoke to this problem: no
// other LeetCode entry shares this shape, so it stays here rather than in
// DataStructures/.
internal interface IKthLargestStream
{
    int Add(int val);
}
