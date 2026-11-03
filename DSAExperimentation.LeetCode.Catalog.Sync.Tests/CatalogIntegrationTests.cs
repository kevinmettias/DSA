using DSAExperimentation.LeetCode.Catalog.Bounds;
using DSAExperimentation.LeetCode.Catalog.Questions;
using DSAExperimentation.LeetCode.Catalog.Storage;
using DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Tests;

// The seam between the sync tool and the catalog: the converter run against a question as the
// catalog stores it, and the cases it keeps read back through the catalog's own notation and bounds.
// Each side's unit tests prove it alone; these prove they agree on the contract between them - one
// input per judge parameter, LeetCode notation throughout, and the stated bounds.
public sealed partial class CatalogIntegrationTests
{
    private const string TwoSum = "two-sum";

    private static readonly LeetCodeQuestion StoredTwoSum = LeetCodeCatalog.Question(TwoSum);

    [Fact]
    public void Convert_CaseRepeatingAStoredExample_DropsItAsADuplicate()
    {
        var example = StoredTwoSum.Examples[0];
        var input = $"nums = {example.Inputs[0]}, target = {example.Inputs[1]}";

        var conversion = new DatasetCaseConverter(StoredTwoSum).Convert([new DatasetCaseDto { Input = input, Output = "[0, 1]" }]);

        Assert.Empty(conversion.Cases);
        Assert.Equal(1, conversion.Dropped[DatasetDropReason.Duplicate]);
    }

    [Fact]
    public void Convert_CaseInsideTheStoredBounds_ReadsBackThroughTheCatalogsNotation()
    {
        var conversion = new DatasetCaseConverter(StoredTwoSum)
            .Convert([new DatasetCaseDto { Input = "nums = [5, 1, 4], target = 9", Output = "[0, 2]" }]);

        var kept = Assert.Single(conversion.Cases);
        Assert.Equal([5, 1, 4], kept.Input<int[]>(0));
        Assert.Equal([0, 2], kept.Expected<int[]>());
    }

    [Fact]
    public void Convert_CaseOutsideTheStoredBounds_DropsWhatTheCatalogsBoundsReject()
    {
        var tooShort = new LeetCodeCase { Inputs = ["[5]", "9"] };
        var isRejectedByBounds = LeetCodeBounds.Of(StoredTwoSum.Constraints)
            .Any(bound => bound.Check(tooShort, StoredTwoSum.Signature.Parameters) == LeetCodeBoundCheck.Violated);

        var conversion = new DatasetCaseConverter(StoredTwoSum).Convert([new DatasetCaseDto { Input = "nums = [5], target = 9", Output = "[0, 0]" }]);

        Assert.Equal((true, 1), (isRejectedByBounds, conversion.Dropped[DatasetDropReason.OutOfBounds]));
    }

    [Fact]
    public void Read_StoredDatasetCases_EveryOneIsWhatTheConverterWouldKeep()
    {
        var stored = CaseFiles.Read(LeetCodeCaseSource.Dataset, TwoSum);
        var bounds = LeetCodeBounds.Of(StoredTwoSum.Constraints);

        Assert.All(stored, testCase => Assert.Equal(StoredTwoSum.Signature.Parameters.Count, testCase.Inputs.Count));
        Assert.DoesNotContain(stored, testCase => bounds.Any(bound => bound.Check(testCase, StoredTwoSum.Signature.Parameters) == LeetCodeBoundCheck.Violated));
    }
}
