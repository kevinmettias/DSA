using DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Tests.Dataset;

// ReadRowsAsync reads a local path the same way it reads a downloaded file, so a small JSONL file
// stands in for the dataset here without the network.
public sealed partial class LeetCodeDatasetTests : IDisposable
{
    private const string FirstRow = """{"task_id": "two-sum", "question_id": 1, "input_output": [{"input": "nums = [3,3], target = 6", "output": "[0, 1]"}]}""";
    private const string RepeatedRow = """{"task_id": "two-sum", "question_id": 1, "input_output": []}""";
    private const string OtherRow = """{"task_id": "climbing-stairs", "question_id": 70, "input_output": [{"input": "n = 2", "output": "2"}]}""";

    private readonly DirectoryInfo _folder = Directory.CreateTempSubdirectory("leetcode-dataset-tests-");

    public void Dispose() => _folder.Delete(recursive: true);

    [Fact]
    public async Task ReadRowsAsync_LocalJsonLines_KeepsTheFirstRowForEachSlug()
    {
        var path = Path.Combine(_folder.FullName, "dataset.jsonl");
        await File.WriteAllLinesAsync(path, [FirstRow, string.Empty, RepeatedRow, OtherRow]);

        var rows = await LeetCodeDataset.ReadRowsAsync([path], CancellationToken.None);

        Assert.Equal(["climbing-stairs", "two-sum"], rows.Keys.Order(StringComparer.Ordinal));
        Assert.Equal("nums = [3,3], target = 6", Assert.Single(rows["two-sum"].Cases).Input);
    }
}
