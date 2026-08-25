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
}
