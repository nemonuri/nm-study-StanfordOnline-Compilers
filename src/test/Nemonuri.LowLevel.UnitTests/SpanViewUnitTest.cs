using Nemonuri.LowLevel.Extensions;

namespace Nemonuri.LowLevel.UnitTests;

public class SpanViewUnitTest
{
    public record struct DummyStruct
    {
        public int First, Second, Third;

        public DummyStruct(int first, int second, int third)
        {
            First = first;
            Second = second;
            Third = third;
        }

        public static ref int GetFromName(ref DummyStruct self, string name)
        {
            if (name is nameof(First)) {return ref self.First;}
            else if (name is nameof(Second)) {return ref self.Second;}
            else if (name is nameof(Third)) {return ref self.Third;}

            throw new ArgumentOutOfRangeException(paramName: nameof(name));
        }
    }

    [ThreadStatic]
    private static string? _threadStaticString;

    [Theory]
    [MemberData(nameof(MemberData1))]
    public unsafe void ToArray(string name, int[] expectedResult)
    {
        // Arrange
        static ref int RefSelector(ref DummyStruct dummyStruct) => ref DummyStruct.GetFromName(ref dummyStruct, _threadStaticString!);

        Span<DummyStruct> dummyStructs = [new(11, 12, 13), new(14, 15, 16), new(21, 22, 23), new(51, 52, 53)];
        _threadStaticString = name;

        // Act
        int[] actualResult = dummyStructs.ToView(new RefSelectorHandle<DummyStruct, int>(&RefSelector)).ToArray();
        
        // Assert
        Assert.Equal(expectedResult, actualResult);
    }

    public static TheoryData<string, int[]> MemberData1 => new()
    {
        { nameof(DummyStruct.First), [11, 14, 21, 51] },
        { nameof(DummyStruct.Second), [12, 15, 22, 52] },
        { nameof(DummyStruct.Third), [13, 16, 23, 53] }
    };
}