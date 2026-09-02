using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.DataStructures.Sequence;

public sealed partial class MatrixSequenceTests
{
    [Fact]
    public void Length_ReflectsRowsTimesColumns()
    {
        var sequence = new MatrixSequence<int>([[1, 2, 3], [4, 5, 6]]);

        Assert.Equal(6, sequence.Length);
    }

    [Fact]
    public void Get_ValidIndex_ReturnsElementAtRowMajorPosition()
    {
        var sequence = new MatrixSequence<int>([[1, 2, 3], [4, 5, 6]]);

        Assert.Equal(5, sequence.Get(4));
    }

    [Fact]
    public void Get_LastIndex_ReturnsBottomRightElement()
    {
        var sequence = new MatrixSequence<int>([[1, 2, 3], [4, 5, 6]]);

        Assert.Equal(6, sequence.Get(5));
    }
}
