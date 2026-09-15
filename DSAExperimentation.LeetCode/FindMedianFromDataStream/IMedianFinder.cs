namespace DSAExperimentation.LeetCode.FindMedianFromDataStream;

// The AddNum/FindMedian contract every strategy above implements. Bespoke to this
// problem: no other LeetCode entry shares this shape, so it stays here rather than
// in DataStructures/.
internal interface IMedianFinder
{
    void AddNum(int num);

    double FindMedian();
}
