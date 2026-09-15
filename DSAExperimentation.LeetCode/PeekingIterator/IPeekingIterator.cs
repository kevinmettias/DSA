namespace DSAExperimentation.LeetCode.PeekingIterator;

// The HasNext/Peek/Next contract every strategy above implements. Bespoke to
// this problem: no other LeetCode entry shares this shape, so it stays here
// rather than in DataStructures/.
internal interface IPeekingIterator
{
    bool HasNext();

    int Peek();

    int Next();
}
