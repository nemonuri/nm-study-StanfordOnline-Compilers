using Nemonuri.LowLevel;

namespace Nemonuri.LowLevel.UnitTests;

public class PackedTableUnitTest
{
    [Theory]
    [MemberData(nameof(MemberData1))]
    public void TryGetValue(string key, bool expectedSuccess, int expectedValue)
    {
        // Arrange
        PackedTable<string, int> table = new(new([new("", 1), new("a", 1234), new("b", 5678)]));
        
        // Act
        bool actualSuccess = PackedTableTheory.Theorize<string, int, PackedTable<string, int>>(in table).TryGetValue(in key, out int actualValue);
        
        // Assert
        Assert.Equal(expectedSuccess, actualSuccess);
        Assert.Equal(expectedValue, actualValue);
    }

    public static TheoryData<string, bool, int> MemberData1 => new()
    {
        {"", true, 1},
        {"a", true, 1234},
        {"b", true, 5678},
        {"c", false, default}
    };
}
