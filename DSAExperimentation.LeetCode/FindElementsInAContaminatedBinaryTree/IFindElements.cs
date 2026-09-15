namespace DSAExperimentation.LeetCode.FindElementsInAContaminatedBinaryTree;

// The Find contract every strategy above implements - LeetCode's own FindElements
// API, reduced to the one query it exposes. Bespoke to this problem, so it stays
// beside the solution rather than in DataStructures/.
internal interface IFindElements
{
    bool Find(int target);
}
