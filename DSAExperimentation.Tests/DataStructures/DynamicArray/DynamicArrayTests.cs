using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.DataStructures.DynamicArray;

public sealed partial class DynamicArrayTests
{
    [Fact]
    public void Add_IncreasesCountAndAppendsValue()
    {
        var array = new DynamicArray<int>();

        array.Add(1);
        array.Add(2);

        Assert.Equal(2, array.Count);
        Assert.Equal(1, array.Get(0));
        Assert.Equal(2, array.Get(1));
    }

    [Fact]
    public void Add_BeyondInitialCapacity_GrowsAndRetainsAllValues()
    {
        var array = new DynamicArray<int>();

        for (var i = 0; i < 20; i++)
        {
            array.Add(i);
        }

        Assert.Equal(20, array.Count);

        for (var i = 0; i < 20; i++)
        {
            Assert.Equal(i, array.Get(i));
        }
    }

    [Fact]
    public void Set_OverwritesValueAtIndex()
    {
        var array = new DynamicArray<int>();
        array.Add(1);

        array.Set(0, 42);

        Assert.Equal(42, array.Get(0));
    }

    [Fact]
    public void Insert_AtMiddleIndex_ShiftsSubsequentValuesRight()
    {
        var array = new DynamicArray<int>();
        array.Add(1);
        array.Add(3);

        array.Insert(1, 2);

        Assert.Equal(3, array.Count);
        Assert.Equal(1, array.Get(0));
        Assert.Equal(2, array.Get(1));
        Assert.Equal(3, array.Get(2));
    }

    [Fact]
    public void Insert_AtEnd_AppendsValue()
    {
        var array = new DynamicArray<int>();
        array.Add(1);

        array.Insert(1, 2);

        Assert.Equal(2, array.Count);
        Assert.Equal(2, array.Get(1));
    }

    [Fact]
    public void RemoveAt_MiddleIndex_ShiftsSubsequentValuesLeft()
    {
        var array = new DynamicArray<int>();
        array.Add(1);
        array.Add(2);
        array.Add(3);

        array.RemoveAt(1);

        Assert.Equal(2, array.Count);
        Assert.Equal(1, array.Get(0));
        Assert.Equal(3, array.Get(1));
    }

    [Fact]
    public void Get_IndexOutOfRange_ThrowsArgumentOutOfRangeException()
    {
        var array = new DynamicArray<int>();
        array.Add(1);

        Assert.Throws<ArgumentOutOfRangeException>(() => array.Get(1));
    }

    [Fact]
    public void Insert_IndexBeyondCount_ThrowsArgumentOutOfRangeException()
    {
        var array = new DynamicArray<int>();

        Assert.Throws<ArgumentOutOfRangeException>(() => array.Insert(1, 0));
    }

    [Fact]
    public void TryGet_ValidIndex_ReturnsTrueAndValue()
    {
        var array = new DynamicArray<int>();
        array.Add(1);

        var found = array.TryGet(0, out var value);

        Assert.True(found);
        Assert.Equal(1, value);
    }

    [Fact]
    public void TryGet_IndexOutOfRange_ReturnsFalse()
    {
        var array = new DynamicArray<int>();
        array.Add(1);

        var found = array.TryGet(1, out _);

        Assert.False(found);
    }

    [Fact]
    public void TrySet_ValidIndex_ReturnsTrueAndOverwritesValue()
    {
        var array = new DynamicArray<int>();
        array.Add(1);

        var succeeded = array.TrySet(0, 42);

        Assert.True(succeeded);
        Assert.Equal(42, array.Get(0));
    }

    [Fact]
    public void TrySet_IndexOutOfRange_ReturnsFalse()
    {
        var array = new DynamicArray<int>();
        array.Add(1);

        var succeeded = array.TrySet(1, 42);

        Assert.False(succeeded);
    }

    [Fact]
    public void TryInsert_IndexBeyondCount_ReturnsFalse()
    {
        var array = new DynamicArray<int>();

        var succeeded = array.TryInsert(1, 0);

        Assert.False(succeeded);
    }

    [Fact]
    public void TryRemoveAt_ValidIndex_ReturnsTrueAndRemovesValue()
    {
        var array = new DynamicArray<int>();
        array.Add(1);
        array.Add(2);

        var succeeded = array.TryRemoveAt(0);

        Assert.True(succeeded);
        Assert.Equal(1, array.Count);
        Assert.Equal(2, array.Get(0));
    }

    [Fact]
    public void TryRemoveAt_IndexOutOfRange_ReturnsFalse()
    {
        var array = new DynamicArray<int>();
        array.Add(1);

        var succeeded = array.TryRemoveAt(1);

        Assert.False(succeeded);
    }

    [Fact]
    public void Count_NewArray_IsZero() => Assert.Equal(0, new DynamicArray<int>().Count);

    [Fact]
    public void Count_TracksAddsAndRemovals()
    {
        var array = new DynamicArray<int>();

        array.Add(1);
        array.Add(2);

        Assert.Equal(2, array.Count);

        array.RemoveAt(0);

        Assert.Equal(1, array.Count);
    }

    [Fact]
    public void Count_SurvivesGrowthPastTheInitialCapacity()
    {
        var array = new DynamicArray<int>();

        for (var i = 0; i < 100; i++)
        {
            array.Add(i);
        }

        Assert.Equal(100, array.Count);
    }
}
