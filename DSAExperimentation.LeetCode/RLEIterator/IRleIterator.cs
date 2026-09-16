namespace DSAExperimentation.LeetCode.RLEIterator;

// The next(elementCount) contract every strategy above implements. Bespoke to this
// problem: no other LeetCode entry shares this shape, so it stays here rather than
// in DataStructures/.
internal interface IRleIterator
{
    int Next(int elementCount);
}
