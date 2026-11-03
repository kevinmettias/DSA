using DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Tests.Dataset;

public sealed partial class DatasetInputTests
{
    [Fact]
    public void Split_TwoAssignments_ReturnsEachNameAndValue()
        => Assert.Equal([("nums", "[3,3]"), ("target", "6")], DatasetInput.Split("nums = [3,3], target = 6"));

    [Fact]
    public void Split_CommasInsideValues_SplitsOnlyBetweenAssignments()
    {
        var assignments = DatasetInput.Split("grid = [[1, 2], [3, 4]], s = 'a, b = c', k = 2");

        Assert.Equal([("grid", "[[1, 2], [3, 4]]"), ("s", "'a, b = c'"), ("k", "2")], assignments);
    }

    [Fact]
    public void Split_EscapedQuoteInsideString_StaysInsideTheValue()
        => Assert.Equal([("s", "'it\\'s, x = 1'"), ("n", "1")], DatasetInput.Split("s = 'it\\'s, x = 1', n = 1"));

    [Fact]
    public void Split_ValueThatIsNotAnAssignment_ReturnsNull()
        => Assert.Null(DatasetInput.Split("[1,2,3]"));
}
