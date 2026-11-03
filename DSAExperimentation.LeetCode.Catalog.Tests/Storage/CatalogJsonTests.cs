using DSAExperimentation.LeetCode.Catalog.Storage;

namespace DSAExperimentation.LeetCode.Catalog.Tests.Storage;

// CatalogJson takes explicit paths, so these exercise it in a folder of their own rather than in the
// checked-in snapshot.
public sealed partial class CatalogJsonTests : IDisposable
{
    private readonly DirectoryInfo _folder = Directory.CreateTempSubdirectory("catalog-json-tests-");

    public void Dispose() => _folder.Delete(recursive: true);

    [Fact]
    public void PathOf_Slug_NamesTheSlugsJsonFileInTheFolder()
    {
        var expected = Path.Combine("questions", "two-sum.json");

        var path = CatalogJson.PathOf("questions", "two-sum");

        Assert.Equal(expected, path);
    }

    [Fact]
    public void SlugsIn_FolderOfJsonFiles_ListsEachFilesSlugInOrdinalOrder()
    {
        foreach (var name in new[] { "two-sum.json", "3sum.json", "notes.txt" })
        {
            var path = PathIn(name);
            File.WriteAllText(path, "{}");
        }

        var slugs = CatalogJson.SlugsIn(_folder.FullName);

        Assert.Equal(["3sum", "two-sum"], slugs);
    }

    [Fact]
    public void SlugsIn_MissingFolder_ListsNone()
    {
        var missing = PathIn("absent");

        var slugs = CatalogJson.SlugsIn(missing);

        Assert.Empty(slugs);
    }

    [Fact]
    public void TryRead_MissingFile_ReportsNothingRead()
    {
        var missing = PathIn("absent.json");

        var isRead = CatalogJson.TryRead<int[]>(missing, out _);

        Assert.False(isRead);
    }

    [Fact]
    public void TryWrite_NewThenSameThenDifferentDocument_WritesOnlyWhenTheTextChanges()
    {
        var relative = Path.Combine("nested", "values.json");
        var path = PathIn(relative);

        var writes = (CatalogJson.TryWrite(path, new[] { 1, 2 }), CatalogJson.TryWrite(path, new[] { 1, 2 }), CatalogJson.TryWrite(path, new[] { 3 }));

        Assert.Equal((true, false, true), writes);
    }

    [Fact]
    public void TryRead_FileTryWriteProduced_ReadsTheSameDocumentBack()
    {
        var path = PathIn("values.json");
        CatalogJson.TryWrite(path, new[] { 4, 5, 6 });

        var isRead = CatalogJson.TryRead<int[]>(path, out var values);

        Assert.True(isRead);
        Assert.NotNull(values);
        Assert.Equal([4, 5, 6], values);
    }

    private string PathIn(string relative) => Path.Combine(_folder.FullName, relative);
}
